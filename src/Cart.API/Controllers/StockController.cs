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
        public async Task<GrainInterfaces.States.Item> Get(Guid id)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            //TODO: Check if item is there?
            return await grain.GetAvailability();
        }

        [HttpGet("subtract/{id}/{amount}")]
        public async Task<GrainInterfaces.States.Item> SubtractStock(Guid id, int amount)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            //TODO: Check if item is there?
            return await grain.modifyStock(-1*amount);
        }

        [HttpGet("add/{id}/{amount}")]
        public async Task<GrainInterfaces.States.Item> AddStock(Guid id, int amount)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            //TODO: Check if item is there?
            return await grain.modifyStock(amount);
        }

        [HttpPost("item/create")]
        public async Task<GrainInterfaces.States.Item> CreateItem()
        {
            var item = _client.CreateItem()
            return await grain.GetItem().Id
        }
     
    }
}
