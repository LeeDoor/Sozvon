using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    public class WebRTCHub : Hub
    {
        public async Task JoinCall(string userId)
        {
            await Clients.Others.SendAsync("UserJoined", userId);
        }

        public async Task SendOffer(string targetUserId, string offer)
        {
            await Clients.User(targetUserId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }

        public async Task SendAnswer(string targetUserId, string answer)
        {
            await Clients.User(targetUserId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        public async Task SendIceCandidate(string targetUserId, string candidate)
        {
            await Clients.User(targetUserId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        public async Task LeaveCall()
        {
            await Clients.Others.SendAsync("UserLeft", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.Others.SendAsync("UserLeft", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}