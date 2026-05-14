using System;
using System.Collections.Generic;
using System.Text;

namespace Bus.Shared.Consumer
{
    public interface IMessageQueueConsumer
    {
        Task StartConsumingAsync(Func<ConsumeEventArgs, Task> messageHandler);
    }
}
