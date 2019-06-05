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
    public class StockController : Controller
    {
        private readonly IClusterClient _client;
        
        public StockController(IClusterClient client)
        {
            _client = client;
        }
      
        [HttpGet("availability/{id}")]
        public async Task<int> Get(Guid id)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            //TODO: Check if item is there?
            return await grain.GetAvailability();
        }

        [HttpGet("subtract/{id}/{amount}")]
        public async Task<int> SubtractStock(Guid id, int amount)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            //TODO: Check if item is there?
            return await grain.ModifyStock(-1*amount);
        }

        [HttpGet("add/{id}/{amount}")]
        public async Task<int> AddStock(Guid id, int amount)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            //TODO: Check if item is there?
            return await grain.ModifyStock(amount);
        }

        [HttpPost("item/create")]
        public async Task<Item> CreateItem(decimal price, int stock)
        {
            var guid = Guid.NewGuid();
            var grain = _client.GetGrain<IItemGrain>(guid);
            return await grain.GetItem();
        }
     
    }
}
