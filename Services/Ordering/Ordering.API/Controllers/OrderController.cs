﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;
using System.Net;

namespace Ordering.API.Controllers
{
    public class OrderController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;
        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }
        [HttpGet("{userName}", Name = "GetOrdersByUserName")]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrdersByUserName(string userName)
        {
            var query = new GetOrderListQuery(userName);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }

        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand checkoutOrderCommand)
        {
            var result = await _mediator.Send(checkoutOrderCommand);
            return Ok(result);
        }

        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateOrderCommand updateOrderCommand)
        {
            var result = await _mediator.Send(updateOrderCommand);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> DeleteOrder(int id)
        {
            var cmd = new DeleteOrderCommand() { Id = id };
            await _mediator.Send(cmd);
            return NoContent();
        }
    }
}
