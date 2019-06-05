using System;
using System.Threading.Tasks;
using GrainInterfaces;
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
      
        [HttpGet("create/{id}")]
        public async Task<GrainInterfaces.States.Order> Get(Guid id)
        {
            var grain = _client.GetGrain<IOrderGrain>(id);
            return await grain.GetOrder();
        }

        [HttpGet("/remove/{id}")]
        public async Task CancelOrder(Guid id)
        {
            var grain = _client.GetGrain<IOrderGrain>(id);
            await grain.CancelOrder();
        }
        
        [HttpGet("/find/{id}")]
        public async Task<GrainInterfaces.States.Order> GetOrder(Guid id)
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