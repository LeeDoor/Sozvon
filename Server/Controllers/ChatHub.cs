using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            if (ContainsBadWords(message)) await Clients.All.SendAsync("Receive", message, Context?.User?.Identity?.Name ?? "Гость");
            else await Clients.Caller.SendAsync("BadMessage");//напомнить лене обработать
        }
        private bool ContainsBadWords(string message)
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory, "banWords.txt");

            if (string.IsNullOrEmpty(message))
                return false;

            var banWords = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim().ToLower());

            foreach (var banWord in banWords)
            {
                if (message.ToLower().Contains(banWord))
                {
                    return true;
                }
            }

            return false;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вошел в чат");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
