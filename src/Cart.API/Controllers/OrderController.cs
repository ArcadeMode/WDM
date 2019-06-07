using System;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Cart.API.Controllers
{
    [Route("api/orders")]
    public class OrderController : Controller
    {
        private readonly IClusterClient _client;
        
        public OrderController(IClusterClient client)
        {
            _client = client;
        }
      
        [HttpPost("create/{id}")]
        public async Task<Guid> Get(Guid userId)
        {
            var userGrain = _client.GetGrain<IUserGrain>(userId);
            var orderGrain = _client.GetGrain<IOrderGrain>(Guid.NewGuid());
            await orderGrain.SetUser(userGrain);
            return orderGrain.GetPrimaryKey();
        }

        [HttpPost("/remove/{id}")]
        public async Task CancelOrder(Guid id)
        {
            var grain = _client.GetGrain<IOrderGrain>(id);
            await grain.CancelOrder();
        }
        
        [HttpGet("/find/{id}")]
        public async Task<OrderState> GetOrder(Guid id)
        {
            var grain = _client.GetGrain<IOrderGrain>(id);
            return await grain.GetOrder();
        }

        [HttpPost("/addItem/{orderId}/{itemId}")]
        public async Task AddItem(Guid orderId, Guid itemId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            //TODO: Check if item exists?
            await orderGrain.AddItem(itemId);
        }

        [HttpPost("/removeItem/{orderId}/{itemId}")]
        public async Task RemoveItem(Guid orderId, Guid itemId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            //TODO: Check if item exists?
            await orderGrain.RemoveItem(itemId);
        }

        [HttpPost("/checkout/{id}")]
        public async Task<bool> CheckoutOrder(Guid orderId)
        {
            var orderGrain = _client.GetGrain<IOrderGrain>(orderId);
            return await orderGrain.Checkout();
        }
    }
}