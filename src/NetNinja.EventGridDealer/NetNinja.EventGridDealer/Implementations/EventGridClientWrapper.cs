using System.Text.Json;
using Azure;
using Azure.Identity;
using Azure.Messaging.EventGrid;
using NetNinja.EventGridDealer.Configuration;
using NetNinja.EventGridDealer.Contracts;

namespace NetNinja.EventGridDealer.Implementations
{
    public class EventGridClientWrapper<T> : IEventGridClientWrapper<T>
    {
        public EventGridPublisherClient PublisherClient { get; set; }
        private readonly EventGridConfiguration _eventGridConfiguration;
        public EventGridClientWrapper(EventGridConfiguration config)
        {
            
            _eventGridConfiguration = config;
            
            PublisherClient = new EventGridPublisherClient(
                _eventGridConfiguration.TopicEndpoint,
                new AzureKeyCredential(_eventGridConfiguration.TopicKey)
            );
            
        }

        #region AuthRegion

        public void AuthBySasCredential(int hoursToExpire)
        {
            var sasToken = GetSasCredential(hoursToExpire);

            var sasCredential = new AzureSasCredential(sasToken);
            
            PublisherClient = new EventGridPublisherClient(
                _eventGridConfiguration.TopicEndpoint,
                sasCredential
            );
        }

        public void AuthByEntraId()
        {
            PublisherClient = new EventGridPublisherClient(
                _eventGridConfiguration.TopicEndpoint,
                new DefaultAzureCredential()
            );
        }

        #endregion

        #region Publish Methods

        public async Task PublishBasicEventAsync(string subject , string eventType , string dataVersion , T message)
        {
            EventGridEvent egEvent = new EventGridEvent(
                subject,
                eventType,
                dataVersion,
                JsonSerializer.Serialize(message)
            );

            await PublisherClient.SendEventAsync(egEvent);
        }
        
        public async Task PublishBasicEventAsync(string subject , string eventType , string dataVersion , List<T> message)
        {
            EventGridEvent egEvent = new EventGridEvent(
                subject,
                eventType,
                dataVersion,
                JsonSerializer.Serialize(message)
            );

            await PublisherClient.SendEventAsync(egEvent);
        }

        #endregion
        
        #region Private methods

        private string GetSasCredential(int hoursToExpire)
        {
            var builder = new EventGridSasBuilder(_eventGridConfiguration.TopicEndpoint, DateTimeOffset.Now.AddHours(hoursToExpire));

            var keyCredential = new AzureKeyCredential(_eventGridConfiguration.TopicKey);

            var sasToken = builder.GenerateSas(keyCredential);

            return sasToken;
        }

        #endregion
        
    }
};

