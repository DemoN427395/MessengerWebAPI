using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using MessageService.Services;

namespace MessageService.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly RabbitMQStreamService _rabbitService;

        public ChatHub(ILogger<ChatHub> logger, RabbitMQStreamService rabbitService)
        {
            _logger = logger;
            _rabbitService = rabbitService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
            _logger.LogInformation("User {UserId} ({Email}) connected", userId, email);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation("User {UserId} disconnected", userId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(string chatId)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User not authenticated");

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            _logger.LogInformation("User {UserId} joined chat {ChatId}", userId, chatId);
        }

        public async Task SendMessage(string chatId, string message)
        {
            var userId = Context.UserIdentifier;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            var messageData = new
            {
                UserId = userId,
                UserName = userName,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            // Асинхронная публикация сообщения в RabbitMQ Stream
            await _rabbitService.PublishMessageAsync(chatId, messageData);

            // Рассылка сообщения всем участникам группы (чата)
            await Clients.Group(chatId)
                .SendAsync("ReceiveMessage", messageData);
        }
    }
}
