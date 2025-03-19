using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Core.Entities;
using MediatR;
using System.Net;
using Basket.Application.Commands;
using EventBus.Messages.Events;

namespace Basket.API.Controllers.V2
{
    [Asp.Versioning.ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BasketControllerV2 : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BasketController> _logger;

        public BasketControllerV2(IMediator mediator, IPublishEndpoint publishEndpoint, ILogger<BasketController> logger)
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }


        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckoutV2 basketCheckout)
        {
            var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
            var basket = await _mediator.Send(query);
            if (basket == null)
            {
                return BadRequest();
            }
            var eventMsg = BasketMapper.Mapper.Map<BasketCheckoutEventV2>(basketCheckout);
            eventMsg.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMsg);
            _logger.LogInformation($"Basket Published for {basket.Username} with V2 endpoint");
            var deleteCmd = new DeleteBasketByUserNameCommand(basketCheckout.UserName);
            await _mediator.Send(deleteCmd);
            return Accepted();
        }
    }
}
