using System;
using System.Threading.Tasks;
using Cart.API.Models;
using GrainInterfaces;
using GrainInterfaces.States;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Cart.API.Controllers
{
    [Route("api/stock")]
    public class StockController : Controller
    {
        private readonly IClusterClient _client;
        
        public StockController(IClusterClient client)
        {
            _client = client;
        }

        [HttpPost("item")]
        public async Task<ActionResult> CreateItem()
        {
            var grain = _client.GetGrain<IItemGrain>(Guid.NewGuid());
            return Ok(await grain.GetItem());
        }

        [HttpGet("item/{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            var item = await grain.GetItem();
            if (item.Price == 0)
            {
                return NotFound(new MessageResult("Item not found"));
            }
            return Ok(item);
        }

        [HttpDelete("item/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            var item = await grain.GetItem();
            if(item.Price == 0)
            {
                return NotFound(new MessageResult("Item not found"));
            }
            if(await grain.Delete())
            {
                return Ok(new MessageResult("Item deleted"));

            }
            return BadRequest(new MessageResult("Item not deleted"));

        }

        [HttpPut("item/{id}/subtract/{amount}")]
        public async Task<ActionResult> SubtractStock(Guid id, int amount)
        {
            return await ModifyStock(id, -amount);
        }

        [HttpPut("item/{id}/add/{amount}")]
        public async Task<ActionResult> AddStock(Guid id, int amount)
        {
            return await ModifyStock(id, amount);
        }

        private async Task<ActionResult> ModifyStock(Guid id, int amount)
        {
            var grain = _client.GetGrain<IItemGrain>(id);
            var item = await grain.GetItem();
            if (item.Price == 0)
            {
                return NotFound(new MessageResult("Item not found"));
            }
            if (await grain.ModifyStock(amount))
            {
                return Ok(await grain.GetItem()); //return new reference to item state to get updated values
            }
            return BadRequest(new MessageResult("Insufficient stock"));
        }
     
    }
}
