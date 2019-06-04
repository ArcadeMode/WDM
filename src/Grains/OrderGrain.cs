using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public class OrderGrain: Grain<OrderState>, IOrderGrain
    {

        public async Task<Cart> GetOrder()
        {
            await ReadStateAsync();
            
            if (State.Value != null) return State.Value;

            State.Value = new Order
            {
                Id =  Guid.NewGuid(),
                Items = new List<Items>()
            };

            await WriteStateAsync();
            
            return State.Value;
        }

        public Task<bool> AddItem(Guid itemGuid)
        {
            await ReadStateAync();

            if (State.Value == null)
            {
                State = new OrderState();
            }
        
            State.Value.Items.Add(itemGuid)

            await WriteStateAsync();

            return true;
        }

        public Task<bool> RemoveItem(Guid itemGuid)
        {
            await ReadStateAync();

            if (State.Value == null)
            {
                State = new OrderState();
            }
        
            State.Value.Items.Remove(itemGuid)

            await WriteStateAsync();

            return true;
        }

        public async Task<bool> CancelOrder()
        {
            await ClearStateAsync();

            return true;
        }

        public Task<bool> Checkout()
        {
            throw new NotImplementedException();
        }
    }
}