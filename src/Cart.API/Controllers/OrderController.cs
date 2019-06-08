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

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(Guid orderId)
        {
            var grain = _client.GetGrain<IOrderGrain>(orderId);
            if ((await grain.GetOrder()).UserId == Guid.Empty)
            {
                return NotFound(new MessageResult("Order not found"));
            }
            if (await grain.DeleteOrder())
            {
                return Ok(new MessageResult("Order deleted"));
            }
            return BadRequest(new MessageResult("Order deletion failed"));
        }
        
        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrder(Guid orderId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if ((await orderGrain.GetOrder()).UserId == Guid.Empty)
            {
                return NotFound(new MessageResult("Order not found"));
            }

            return Ok(await orderGrain.GetOrder());
        }

        [HttpPut("{orderId}/addItem/{itemId}")]
        public async Task<ActionResult> AddItem(Guid orderId, Guid itemId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if((await orderGrain.GetOrder()).UserId == Guid.Empty)
            {
                return NotFound(new MessageResult("Order not found"));
            }
            var itemGrain = _client.GetGrain<IItemGrain>(itemId);
            if((await itemGrain.GetItem()).Price == 0)
            {
                return NotFound(new MessageResult("Item not found"));
            }
            await orderGrain.AddItem(itemGrain);
            return Ok(await orderGrain.GetOrder());
        }

        [HttpPut("{orderId}/removeItem/{itemId}")]
        public async Task<ActionResult> RemoveItem(Guid orderId, Guid itemId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if ((await orderGrain.GetOrder()).UserId == Guid.Empty)
            {
                return NotFound(new MessageResult("Order not found"));
            }
            var itemGrain = _client.GetGrain<IItemGrain>(itemId);
            if ((await itemGrain.GetItem()).Price == 0)
            {
                return NotFound(new MessageResult("Item not found"));
            }
            if (await orderGrain.RemoveItem(itemGrain))
            {
                return Ok(await orderGrain.GetOrder());
            }
            return BadRequest(new MessageResult("Item not present in order"));
        }

        [HttpPost("{orderId}/checkout")]
        public async Task<ActionResult> CheckoutOrder(Guid orderId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if ((await orderGrain.GetOrder()).UserId == Guid.Empty)
            {
                return NotFound(new MessageResult("Order not found"));
            }
            if(await orderGrain.Checkout())
            {
                return Ok(new MessageResult("Checkout succesfull"));
            }
            return BadRequest(new MessageResult("Checkout failed"));
        }
    }
}