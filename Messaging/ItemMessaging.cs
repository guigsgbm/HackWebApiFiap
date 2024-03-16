using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using HackWebApi.Domain;
using static HackWebApi.Messaging.ItemMessaging;

namespace HackWebApi.Messaging;

public class ItemMessaging
{
    public class ItemMessagingConfig
    {
        public string? ConnectionString { get; set; }
        public string? QueueName { get; set; }
    }

    private readonly IQueueClient _queueClient;
    private readonly ItemMessagingConfig _itemMessagingConfig;
    private readonly ILogger<ItemMessaging> _logger;

    public ItemMessaging(IOptions<ItemMessagingConfig> itemMessagingConfig, ILogger<ItemMessaging> logger)
    {
        _itemMessagingConfig = itemMessagingConfig.Value;
        _queueClient = new QueueClient(
            "",
            "");
        _logger = logger;
    }

    public async Task SendMessageAsync(requestToMessage item)
    {
        string messageJson = JsonConvert.SerializeObject(item);
        var message = new Message(Encoding.UTF8.GetBytes(messageJson));

        await _queueClient.SendAsync(message);
    }

    public void StartMessageProcessing()
    {
        var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
        {
            MaxConcurrentCalls = 1,
            AutoComplete = false
        };

        _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
    }

    public void StopMessageProcessing()
    {
        _queueClient.CloseAsync().Wait();
    }

    private async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
        _logger.LogInformation($"Received message: {Encoding.UTF8.GetString(message.Body)}");

        var bodyJson = Encoding.UTF8.GetString(message.Body);
        var item = JsonConvert.DeserializeObject<VideoRequest>(bodyJson);

        await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        _logger.LogError($"Message handler encountered an exception: {exceptionReceivedEventArgs.Exception}");

        return Task.CompletedTask;
    }

}
