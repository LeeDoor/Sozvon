using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Models.Data;
using Server.Models.Data.Services;
using System.Collections.Concurrent;

namespace Server.Hubs
{
    public class WebRTCHub : Hub
    {
        private readonly ConferenceRoomService _roomService;
        private readonly UserService userService;
        public async Task<RoomInfo> CreateRoom(string roomName)
        {
            ConferenceRoom room = await _roomService.CreateRoomAsync(roomName);
            return new RoomInfo
            {
                RoomId = room.Link,
                RoomName = room.Name,
                UsersCount = room.Users.Count
            };
        }
        public async Task<JoinResult> JoinRoom(string roomId)
        {
            ConferenceRoom? room = await _roomService.GetRoomByLinkWithUsersAsync(roomId);
            if (room is null)
            {
                return new JoinResult { Success = false, Error = "Комната не найдена" };
            }
            string? userId = Context.User?.Identity?.Name;
            if (userId is null) return new JoinResult { Success = false };
            ConferenceRoom? previousRoom = await _roomService.GetRoomByUserLoginAsync(userId);
            if(previousRoom is not null)
            {          
                await LeaveRoom(previousRoom.Id);
            }

            string? userName = Context.User?.Identity?.Name ?? Context.ConnectionId.Substring(0, 8);
            var userInfo = new UserInfo
            {
                UserId = Context.ConnectionId,
                UserName = userName
            };

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // Уведомляем всех в комнате о новом пользователе
            await Clients.OthersInGroup(roomId).SendAsync("UserJoined", userInfo);

            // Уведомляем всех в комнате о новом сообщении в чате
            await Clients.Group(roomId).SendAsync("ReceiveChatMessage", "Система", $"{userName} присоединился к комнате", false);

            List<User> existingUsers = room.Users.Where(x => x.Login != userName).ToList();

            return new JoinResult
            {
                Success = true,
                RoomInfo = new RoomInfo
                {
                    RoomId = room.Link,
                    RoomName = room.Name,
                    UsersCount = room.Users.Count
                },
                ExistingUsers = existingUsers.Select(user => new UserInfo()
                {
                    UserId = user.Login,
                    UserName = user.Name
                }).ToList()
            };
        }

        public async Task LeaveRoom(ConferenceRoomId roomId)
        {
            string userName = Context.User?.Identity?.Name;
            ConferenceRoom? room = await _roomService.GetRoomByIdAsync(roomId);
            if (room is not null)
            {
                await _roomService.RemoveUserFromRoomAsync(roomId);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Link);
                await Clients.OthersInGroup(room.Link).SendAsync("UserLeft", userName);

                // Уведомляем о выходе пользователя в чате
                await Clients.Group(room.Link)
                    .SendAsync("ReceiveChatMessage", "Система", $"{userName} покинул комнату", false);

                if (room.Users.Count <= 0)
                {
                    // REMOVE ROOM DO NOT FORGET
                }
            }
        }

        // Новый метод для отправки сообщений в чат комнаты
        public async Task SendMessageToRoom(string roomId, string message)
        {
            await Clients.Group(roomId).SendAsync("ReceiveChatMessage", Context.User.Identity.Name, message, true);
        }

        public async Task SendSignal(string targetUserId, object signal)
        {
            Console.WriteLine($"Sending signal from {Context.ConnectionId} to {targetUserId}: {signal}");
            await Clients.Client(targetUserId).SendAsync("ReceiveSignal", new
            {
                SenderId = Context.ConnectionId,
                Signal = signal
            });
        }

        public async Task UpdateMediaState(bool hasCamera, bool hasMicrophone)
        {
            string userName = Context.User?.Identity?.Name;
            ConferenceRoom? room = await _roomService.GetRoomByUserLoginAsync(userName);
            {
                await Clients.OthersInGroup(room.Link).SendAsync("UserMediaUpdated", new
                {
                    UserId = Context.ConnectionId,
                    HasCamera = hasCamera,
                    HasMicrophone = hasMicrophone
                });
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userName = Context.User?.Identity?.Name;
            ConferenceRoom? room = await _roomService.GetRoomByUserLoginAsync(userName);
            await LeaveRoom(room.Id);

            await base.OnDisconnectedAsync(exception);
        }

        private string GenerateRoomId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const uint idSize = 5;
            Random random = new Random();
            char[] result = new char[idSize];
            for (int i = 0; i < idSize; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }
    }

    public class RoomInfo
    {
        public string RoomId { get; set; }
        public string RoomName { get; set; }
        public int UsersCount { get; set; }
    }

    public class JoinResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public RoomInfo RoomInfo { get; set; }
        public List<UserInfo> ExistingUsers { get; set; }
    }

    public class UserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}