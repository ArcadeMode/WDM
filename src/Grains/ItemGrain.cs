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

        public override async Task OnActivateAsync() {

            State = State ?? new ItemState
            {
                Id = this.GetPrimaryKey(),
                Price = 0,
                Stock = 0
            };
            await base.OnActivateAsync();
        }
   
        public async Task<ItemState> GetItem()
        {
            await ReadStateAsync();
            return State;
        }

        public async Task<int> GetAvailability()
        {
            await ReadStateAsync();
            return State.Stock;
        }

        public async Task<int> ModifyStock(int amount)
        {
            await ReadStateAsync();
            State.Stock += amount;
            return State.Stock;
        }

        public async Task<decimal> ModifyPrice(decimal newPrice)
        {
            State.Price = newPrice;
            return State.Price;
        }
        
    }

}
