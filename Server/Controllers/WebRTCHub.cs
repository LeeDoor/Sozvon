using Microsoft.AspNetCore.SignalR;
using Server.Models.Data;
using Server.Models.Data.Services;
using System.Collections.Concurrent;

namespace Server.Hubs
{
    public class WebRTCHub : Hub
    {
        private readonly ConferenceRoomService _roomService;
        public WebRTCHub(ConferenceRoomService roomService)
        {
            _roomService = roomService;
        }
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

            if (_roomService.getroo _userRooms.TryGetValue(Context.ConnectionId, out string previousRoomId))
            {
                await LeaveRoom(previousRoomId);
            }
            string? userName = Context.User?.Identity?.Name ?? Context.ConnectionId.Substring(0, 8);
            room.Users.Add(new UserInfo { 
                UserId = Context.ConnectionId, 
                UserName = userName 
            });
            _userRooms[Context.ConnectionId] = roomId;

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            
            var userInfo = new UserInfo
            {
                UserId = Context.ConnectionId,
                UserName = userName
            };

            await Clients.OthersInGroup(roomId).SendAsync("UserJoined", userInfo);

            var existingUsers = room.Users.Where(x => x.UserId != Context.ConnectionId).ToList();

            return new JoinResult
            {
                Success = true,
                RoomInfo = new RoomInfo
                {
                    RoomId = room.Id,
                    RoomName = room.Name,
                    UsersCount = room.Users.Count
                },
                ExistingUsers = existingUsers
            };
        }

        public async Task LeaveRoom(string roomId)
        {
            if (_rooms.TryGetValue(roomId, out Room room))
            {
                room.Users.RemoveWhere(x => x.UserId == Context.ConnectionId);
                _userRooms.TryRemove(Context.ConnectionId, out string _);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                await Clients.OthersInGroup(roomId).SendAsync("UserLeft", Context.ConnectionId);

                if (room.Users.Count <= 0)
                {
                    _rooms.TryRemove(roomId, out Room _);
                }
            }
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
            if (_userRooms.TryGetValue(Context.ConnectionId, out string roomId))
            {
                await Clients.OthersInGroup(roomId).SendAsync("UserMediaUpdated", new
                {
                    UserId = Context.ConnectionId,
                    HasCamera = hasCamera,
                    HasMicrophone = hasMicrophone
                });
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_userRooms.TryGetValue(Context.ConnectionId, out string roomId))
            {
                await LeaveRoom(roomId);
            }

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