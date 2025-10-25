using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using Server.Models.Data;
using System.Collections.Concurrent;

namespace SignalRApp
{
    public class VideoHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();
        private static readonly ConcurrentDictionary<string, string> _userRooms = new();

        public override async Task OnConnectedAsync()
        {
            _userConnections[Context.ConnectionId] = Context.ConnectionId;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // При отключении пользователя уведомляем других участников комнаты
            if (_userRooms.TryRemove(Context.ConnectionId, out var roomId))
            {
                await Clients.Group(roomId).SendAsync("UserDisconnected", Context.ConnectionId);
            }

            _userConnections.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }

        // Создание или присоединение к комнате
        public async Task JoinRoom(string roomId)
        {
            _userRooms[Context.ConnectionId] = roomId;
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // Уведомляем других участников о новом пользователе
            await Clients.OthersInGroup(roomId).SendAsync("UserJoined", Context.ConnectionId);
        }

        // Отправка видео-предложения WebRTC
        public async Task SendOffer(string targetConnectionId, string offer)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }

        // Отправка видео-ответа WebRTC
        public async Task SendAnswer(string targetConnectionId, string answer)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        // Обмен ICE-кандидатами
        public async Task SendIceCandidate(string targetConnectionId, string candidate)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        // Получение видео потока для трансляции (если нужно через сервер)
        [HubMethodName("ReceiveVideoStream")]
        public async Task ReceiveVideoStream(byte[] videoData)
        {
            if (_userRooms.TryGetValue(Context.ConnectionId, out var roomId))
            {
                // Пересылаем видео другим участникам комнаты
                await Clients.OthersInGroup(roomId).SendAsync("ReceiveVideoData", Context.ConnectionId, videoData);
            }
        }
    }
}
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("Receive", message);
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


namespace Server.Controllers
{
    public class AuthController : Controller
    {
        private List<User> _users = new();
        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        public IActionResult RegisterPost([Required] User user)
        {
            _users.Add(user);

            return RedirectToAction("Login", new UserCredential { 
                Login = user.Login, Password = user.Password });
        }
        [HttpGet]
        public IActionResult Login() => View();
        
        [HttpPost]
        public IActionResult LoginPost()
        {
            Console.WriteLine("LoginPost has been activated");
            return RedirectToAction("Login");
        }
    }
}
