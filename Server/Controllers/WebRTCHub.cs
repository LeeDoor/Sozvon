using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Server.Hubs
{
    public class WebRTCHub : Hub
    {
        private static readonly ConcurrentDictionary<string, Room> _rooms = new();
        private static readonly ConcurrentDictionary<string, string> _userRooms = new();

        public class Room
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public HashSet<string> Users { get; set; }
            public int MaxUsers { get; set; }
        }

        public async Task<RoomInfo> CreateRoom(string roomName)
        {
            string roomId = GenerateRoomId();
            var room = new Room
            {
                Id = roomId,
                Name = string.IsNullOrEmpty(roomName) ? "Комната " + roomId : roomName,
                Users = new HashSet<string>(),
                MaxUsers = 30
            };

            _rooms[roomId] = room;

            return new RoomInfo
            {
                RoomId = roomId,
                RoomName = room.Name,
                UsersCount = 0
            };
        }

        public async Task<JoinResult> JoinRoom(string roomId, string userName)
        {
            if (!_rooms.TryGetValue(roomId, out Room room))
            {
                return new JoinResult { Success = false, Error = "Комната не найдена" };
            }

            if (room.Users.Count >= room.MaxUsers)
            {
                return new JoinResult { Success = false, Error = "Комната переполнена" };
            }

            if (_userRooms.TryGetValue(Context.ConnectionId, out string previousRoomId))
            {
                await LeaveRoom(previousRoomId);
            }

            room.Users.Add(Context.ConnectionId);
            _userRooms[Context.ConnectionId] = roomId;

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            var userInfo = new UserInfo
            {
                UserId = Context.ConnectionId,
                UserName = string.IsNullOrEmpty(userName) ? "User" + Context.ConnectionId.Substring(0, 8) : userName
            };

            await Clients.OthersInGroup(roomId).SendAsync("UserJoined", userInfo);

            var existingUsers = new List<UserInfo>();
            foreach (string userId in room.Users)
            {
                if (userId != Context.ConnectionId)
                {
                    existingUsers.Add(new UserInfo
                    {
                        UserId = userId,
                        UserName = "User" + userId.Substring(0, 8)
                    });
                }
            }

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
                room.Users.Remove(Context.ConnectionId);
                _userRooms.TryRemove(Context.ConnectionId, out string _);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                await Clients.OthersInGroup(roomId).SendAsync("UserLeft", Context.ConnectionId);

                if (room.Users.Count == 0)
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
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] result = new char[20];
            for (int i = 0; i < 20; i++)
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