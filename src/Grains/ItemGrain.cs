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

        public override Task OnActivateAsync() {

            if (State.Value == null) {

                State.Value = new Item
                {
                    Id = Guid.NewGuid(),
                    Price = 0,
                    Stock = 0
                };
            }

            return Task.CompletedTask;
        }
   
        public async Task<Item> GetItem()
        {
            await ReadStateAsync();
            return State.Value;
        }

        public async Task<int> GetAvailability()
        {
            await ReadStateAsync();
            return State.Value.Stock;
        }

        public async Task<int> ModifyStock(int amount)
        {
            await ReadStateAsync();
            State.Value.Stock += amount;
            return State.Value.Stock;
        }

        public async Task<decimal> ModifyPrice(decimal newPrice)
        {
            State.Value.Price = newPrice;
            return State.Value.Price;
        }
        
    }

}
