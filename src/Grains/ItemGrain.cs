using System;
using System.Collections.Generic;
using GrainInterfaces;
using Orleans;
using System.Threading.Tasks;
using Orleans.Providers;
using GrainInterfaces.States;

namespace Grains
{
    [StorageProvider(ProviderName="ItemStorage")]
    public class ItemGrain : Grain<ItemState>, IItemGrain
    {

        public override async Task OnActivateAsync() {
            await ReadStateAsync();
            State = State.Id != Guid.Empty ? State : new ItemState
            {
                Id = this.GetPrimaryKey(),
                Price = 0,
                Stock = 0
            };
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await WriteStateAsync();
            await base.OnActivateAsync();
        }
   
        public async Task<ItemState> GetItem()
        {
            return State;
        }

        public async Task<int> GetAvailability()
        {
            return State.Stock;
        }

        public async Task<bool> ModifyStock(int amount)
        {
            if (State.Stock + amount < 0)
            {
                return false;
            }
            State.Stock += amount;
            await WriteStateAsync();
            return true;
        }

        public async Task<decimal> ModifyPrice(decimal newPrice)
        {
            State.Price = newPrice;
            await WriteStateAsync();
            return State.Price;
        }

        public async Task<bool> Delete()
        {
            await ClearStateAsync();
            return true;
        }

    }

}
