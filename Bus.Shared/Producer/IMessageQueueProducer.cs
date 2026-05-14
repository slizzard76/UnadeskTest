using Bus.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bus.Shared.Producer
{
    public interface IMessageQueueProducer
    {
        Task PublishJobAsync(ProcessingJob job, CancellationToken cancellationToken);
    }
}
