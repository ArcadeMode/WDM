using System;
using System.Collections.Generic;
using GrainInterfaces;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans.Providers;

namespace Grains
{
    [StorageProvider(ProviderName="ItemStorage")]
    public class ItemGrain : Grain<ItemState>, IItemGrain
    {

        public async Task<Item> CreateItem() {

            await ReadStateAsync();

            State.Value = new Item
            {
                Id =  Guid.NewGuid(),
                Price = 0,
                Stock = 0
            };

            await WriteStateAsync();
            
            return State.Value;
        }
   
        public async Task<Item> GetItem()
        {
            await ReadStateAsync();
            
            if (State.Value != null) return State.Value;
            
            //Todo: What should be returned when not an item?
            return null;
        }

        public async Task<Item> GetAvailability()
        {
            await ReadStateAsync();

            return State.Value.Stock;
        }

        public async Task<Item> ModifyStock(int amount)
        {
            await ReadStateAsync();

            State.Value.Stock = State.Value.Stock + amount;

            return State.Value.Stock
        }

        public async Task<Item> ModifyPrice(decimal newPrice)
        {
            await ReadStateAsync();

            State.Value.Price = newPrice;

            return State.Value.Price
        }
        
    }

}
