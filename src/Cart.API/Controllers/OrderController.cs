using System;
using System.Threading.Tasks;
using Cart.API.Models;
using GrainInterfaces;
using GrainInterfaces.States;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Cart.API.Controllers
{
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IClusterClient _client;
        
        public OrderController(IClusterClient client)
        {
            _client = client;
        }
      
        [HttpPost("{userId}")]
        public async Task<ActionResult> Create(Guid userId)
        {
            var userGrain = _client.GetGrain<IUserGrain>(userId);
            var orderGrain = _client.GetGrain<IOrderGrain>(Guid.NewGuid());
            await orderGrain.SetUser(userGrain);
            return Ok(new MessageResult(orderGrain.GetPrimaryKey().ToString()));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(Guid id)
        {
            var grain = _client.GetGrain<IOrderGrain>(id);
            if ((await grain.GetOrder()).User == null)
            {
                return NotFound(new MessageResult("Order not found"));
            }
            if (await grain.DeleteOrder())
            {
                return Ok(new MessageResult("Order deleted"));
            }
            return BadRequest(new MessageResult("Order deletion failed"));
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrder(Guid id)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(id);
            if ((await orderGrain.GetOrder()).User == null)
            {
                return NotFound(new MessageResult("Order not found"));
            }

            return Ok(await orderGrain.GetOrder());
        }

        [HttpPost("/{orderId}/addItem/{itemId}")]
        public async Task<ActionResult> AddItem(Guid orderId, Guid itemId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if((await orderGrain.GetOrder()).User == null)
            {
                return NotFound(new MessageResult("Order not found"));
            }

            var itemGrain = _client.GetGrain<IItemGrain>(itemId);
            if((await itemGrain.GetItem()).Price > 0)
            {
                await orderGrain.AddItem(itemGrain);
                return Ok(orderGrain);
            }
            return NotFound(new MessageResult("Item not found"));
        }

        [HttpPost("/{orderId}/removeItem/{itemId}")]
        public async Task<ActionResult> RemoveItem(Guid orderId, Guid itemId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if ((await orderGrain.GetOrder()).User == null)
            {
                return NotFound(new MessageResult("Order not found"));
            }

            var itemGrain = _client.GetGrain<IItemGrain>(itemId);
            if ((await itemGrain.GetItem()).Price > 0 && await orderGrain.RemoveItem(itemGrain))
            {
                return Ok(orderGrain);
            }
            return NotFound(new MessageResult("Item not found"));
        }

        [HttpPost("/checkout/{id}")]
        public async Task<ActionResult> CheckoutOrder(Guid orderId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if ((await orderGrain.GetOrder()).User == null)
            {
                return NotFound(new MessageResult("Order not found"));
            }
            return Ok(await orderGrain.Checkout());
        }
    }
}