
namespace Bus.Shared.Consumer
{
    public interface IMessageQueueConsumer
    {
        Task StartConsumingAsync(Func<ConsumeEventArgs, Task> messageHandler);
    }
}
