using MessageService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace MessageService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly RabbitMQStreamService _rabbitService;

        public MessageController(RabbitMQStreamService rabbitService)
        {
            _rabbitService = rabbitService;
        }

        [HttpGet("history/{chatId}")]
        public IActionResult GetHistory(string chatId)
        {
            // В RabbitMQ Stream нет встроенного механизма хранения истории сообщений.
            // История должна храниться в базе данных (например, Postgres).
            // Здесь можно реализовать логику получения истории из БД.
            return Ok("История сообщений не реализована");
        }
    }
}