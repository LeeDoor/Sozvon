using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace WebRtcApp.Hubs
{
    public class WebRtcHub : Hub
    {
        private static readonly ConcurrentDictionary<string, bool> _connectedUsers = new ConcurrentDictionary<string, bool>();

        public override async Task OnConnectedAsync()
        {
            _connectedUsers[Context.ConnectionId] = false;
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectedUsers.TryRemove(Context.ConnectionId, out _);
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendOffer(string targetUserId, string offer)
        {
            await Clients.Client(targetUserId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }

        public async Task SendAnswer(string targetUserId, string answer)
        {
            await Clients.Client(targetUserId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        public async Task SendIceCandidate(bool isInitiator, string targetUserId, string candidate)
        {
            //          If this request is being sent by initiator, all receivers should get that they are NOT initiators
            //                                                                 vvv
            await Clients.Client(targetUserId).SendAsync("ReceiveIceCandidate", !isInitiator, Context.ConnectionId, candidate);
        }

        // New method to get all connected users
        public List<string> GetConnectedUsers()
        {
            return _connectedUsers.Keys.Where(id => id != Context.ConnectionId).ToList();
        }

        // Notify all users when someone starts sharing
        public async Task NotifyStartedSharing()
        {
            _connectedUsers[Context.ConnectionId] = true;
            await Clients.All.SendAsync("UserStartedSharing", Context.ConnectionId);
        }
        public async Task NotifyStoppedSharing()
        {
            _connectedUsers[Context.ConnectionId] = false;
            await Clients.All.SendAsync("UserStoppedSharing", Context.ConnectionId);
        }
    }
}