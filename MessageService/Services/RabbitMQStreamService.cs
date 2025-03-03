using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;
using System.Text;
using System.Text.Json;

namespace MessageService.Services
{
    public class RabbitMQStreamService : IAsyncDisposable
    {
        private readonly StreamSystem _streamSystem;
        private readonly Producer _producer;
        private const string StreamName = "hello-stream";

        // Для упрощения в конструкторе выполняется синхронная инициализация (в реальном коде рекомендуется использовать асинхронное создание)
        public RabbitMQStreamService()
        {
            // Создаём подключение к RabbitMQ Stream с настройками по умолчанию (localhost, порт по умолчанию и т.д.)
            _streamSystem = StreamSystem.Create(new StreamSystemConfig()).GetAwaiter().GetResult();

            // Объявляем стрим (операция идемпотентна)
            _streamSystem.CreateStream(new StreamSpec(StreamName)
            {
                MaxLengthBytes = 5_000_000_000 // Ограничение на 5 ГБ
            }).GetAwaiter().GetResult();

            // Создаём производителя для стрима
            _producer = Producer.Create(new ProducerConfig(_streamSystem, StreamName)).GetAwaiter().GetResult();
        }

        // Асинхронный метод публикации сообщения
        public async Task PublishMessageAsync<T>(T message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            var msg = new Message(messageBytes);
            await _producer.Send(msg);
        }

        public async ValueTask DisposeAsync()
        {
            await _streamSystem.DisposeAsync();
        }
    }
}