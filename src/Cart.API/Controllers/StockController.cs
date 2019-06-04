using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Stock.API.Controllers
{
    [Route("api/stock")]
    public class ItemController : Controller
    {
        private readonly IClusterClient _client;
        
        public ItemController(IClusterClient client)
        {
            _client = client;
        }
      
        [HttpGet("availability/{id}")]
        public async Task<GrainInterfaces.States.Item> Get(Guid id)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            return await grain.GetAvailability();
        }

        [HttpGet("subtract/{id}/{amount}")]
        public async Task<GrainInterfaces.States.Item> SubtractStock(Guid id, int amount)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            return await grain.modifyStock(-1*amount);
        }

        [HttpGet("add/{id}/{amount}")]
        public async Task<GrainInterfaces.States.Item> AddStock(Guid id, int amount)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            return await grain.modifyStock(amount);
        }

        /stock/add/{item_id}/{number}
        
        [HttpGet("{id}/product")]
        public async Task<List<Product>> GetProduct(Guid id)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            return await grain.GetProducts(-1* amount);
        }

        [HttpPost("{id}/product")]
        public async Task AddProduct(Guid id, [FromBody]Product product)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            await grain.AddProduct(product);
        }
     
    }
}
