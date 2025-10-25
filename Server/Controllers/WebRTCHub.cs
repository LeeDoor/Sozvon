using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Server.Hubs
{
    public class WebRTCHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _rooms = new();

        public async Task JoinRoom(string roomId)
        {
            // Удаляем из предыдущих комнат
            var previousRoom = _rooms.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (previousRoom != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, previousRoom);
                await Clients.OthersInGroup(previousRoom).SendAsync("UserLeft", Context.ConnectionId);
            }

            // Добавляем в новую комнату
            _rooms[roomId] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // Получаем список пользователей в комнате
            var usersInRoom = _rooms.Where(x => x.Key == roomId && x.Value != Context.ConnectionId)
                                   .Select(x => x.Value)
                                   .ToList();

            await Clients.Caller.SendAsync("ExistingUsers", usersInRoom);
            await Clients.OthersInGroup(roomId).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task SendSignal(string targetUserId, object signal)
        {
            await Clients.Client(targetUserId).SendAsync("ReceiveSignal", new
            {
                SenderId = Context.ConnectionId,
                Signal = signal
            });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Удаляем при отключении
            var room = _rooms.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (room != null)
            {
                _rooms.TryRemove(room, out _);
                await Clients.OthersInGroup(room).SendAsync("UserLeft", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}