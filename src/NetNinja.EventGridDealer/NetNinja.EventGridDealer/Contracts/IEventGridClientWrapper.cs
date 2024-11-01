namespace NetNinja.EventGridDealer.Contracts
{
    public interface IEventGridClientWrapper<T>
    {
        void AuthBySasCredential(int hoursToExpire);
        void AuthByEntraId();
        Task PublishBasicEventAsync(string subject, string eventType, string dataVersion, T message);
        Task PublishBasicEventAsync(string subject, string eventType, string dataVersion, List<T> message);
    }
};

