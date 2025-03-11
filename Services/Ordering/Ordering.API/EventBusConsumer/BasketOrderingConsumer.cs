using AutoMapper;
using EventBus.Messages.Common;
using MassTransit;
using MediatR;
using Ordering.Application.Commands;

namespace Ordering.API.EventBusConsumer
{
    public class BasketOrderingConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BasketOrderingConsumer> _logger;
        private readonly IMapper _mapper;
        public BasketOrderingConsumer(IMediator mediator, ILogger<BasketOrderingConsumer> logger, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            using var scope = _logger.BeginScope("Consuming Basket Checkout event for {correlationId}", context.Message.CorelationId);
            var cmd = _mapper.Map<CheckoutOrderCommand>(context.Message);
            await _mediator.Send(cmd);
            _logger.LogInformation("Basket Checkout event completed");

        }
    }
}
