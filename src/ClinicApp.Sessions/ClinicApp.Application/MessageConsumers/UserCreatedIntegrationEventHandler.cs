using ClinicApp.Shared.IntegrationEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Application.MessageConsumers;
public class UserCreatedIntegrationEventHandler : IConsumer<UserCreatedIntegrationEvent>
{
    public Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        return Task.CompletedTask;
    }
}
