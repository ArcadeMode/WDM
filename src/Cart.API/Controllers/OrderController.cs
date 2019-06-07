using System;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Cart.API.Controllers
{
    [Route("api/order")]
    public class OrderController : Controller
    {
        private readonly IClusterClient _client;
        
        public OrderController(IClusterClient client)
        {
            _client = client;
        }
      
        [HttpPost("create/{id}")]
        public async Task<ActionResult> Create(Guid userId)
        {
            var userGrain = _client.GetGrain<IUserGrain>(userId);
            var orderGrain = _client.GetGrain<IOrderGrain>(Guid.NewGuid());
            await orderGrain.SetUser(userGrain);
            return Ok(orderGrain.GetPrimaryKey());
        }

        [HttpPost("remove/{id}")]
        public async Task CancelOrder(Guid id)
        {
            //TODO change to actual deletion of order? I mean, /remove/ right?
            var grain = _client.GetGrain<IOrderGrain>(id);
            await grain.CancelOrder();
        }
        
        [HttpGet("find/{id}")]
        public async Task<ActionResult> GetOrder(Guid id)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(id);
            if ((await orderGrain.GetOrder()).User == null)
            {
                return NotFound("Order not found");
            }

            return Ok(await orderGrain.GetOrder());
        }

        [HttpPost("/addItem/{orderId}/{itemId}")]
        public async Task<ActionResult> AddItem(Guid orderId, Guid itemId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if((await orderGrain.GetOrder()).User == null)
            {
                return NotFound("Order not found");
            }

            var itemGrain = _client.GetGrain<IItemGrain>(itemId);
            if((await itemGrain.GetItem()).Price > 0)
            {
                await orderGrain.AddItem(itemGrain);
                return Ok(orderGrain);
            }
            return NotFound("Item was not found");
        }

        [HttpPost("/removeItem/{orderId}/{itemId}")]
        public async Task<ActionResult> RemoveItem(Guid orderId, Guid itemId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if ((await orderGrain.GetOrder()).User == null)
            {
                return NotFound("Order not found");
            }

            var itemGrain = _client.GetGrain<IItemGrain>(itemId);
            if ((await itemGrain.GetItem()).Price > 0 && await orderGrain.RemoveItem(itemGrain))
            {
                return Ok(orderGrain);
            }
            return NotFound("Item not found");
        }

        [HttpPost("/checkout/{id}")]
        public async Task<ActionResult> CheckoutOrder(Guid orderId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            if ((await orderGrain.GetOrder()).User == null)
            {
                return NotFound("Order not found");
            }
            return Ok(await orderGrain.Checkout());
        }
    }
}