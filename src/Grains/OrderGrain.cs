using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public class OrderGrain: Grain<OrderState>, IOrderGrain
    {
        public Task<bool> SetUser(IUserGrain userGrain) {
            State.Value.User = userGrain;

            await WriteStateAsync();

            return true;
        }

        public async Task<Cart> GetOrder()
        {
            await ReadStateAsync();
            
            if (State.Value != null) return State.Value;

            State.Value = new Order
            {
                Id =  Guid.NewGuid(),
                Items = new Dictionary<Guid, int>();
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

            State.Value.Items.TryGetValue(itemGuid, out var currentCount); 
            State.Value.Items[itemGuid] = currentCount + 1;

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
            var orderItems = State.Value.Items;
            var totalSum = 0

            foreach( KeyValuePair<Guid, int> kvp in openWith )
            {
                itemGrain = GrainFactory.GetGrain<IItemGrain>(kvp.Key);
                item = itemGrain.GetItem();
                if(item.Stock < kvp.Value) {
                   totalSum += item.Price;
                } else {
                    throw new Exception($"Stock of item {kvp.Key} is lower than {kvp.Value}.")
               }
             }

            if(!State.Value.User.GetCredit() < totalSum) {
               await State.Value.User.ModifyCredit(-1*totalSum)
            } else {
                throw new Exception($"Balance of user {State.Value.User.GetPrimaryKey()} is lower than {totalSum}.")
            }

            payment = GrainFactory.GetGrain<IPaymentGrain>();
            await payment.Pay();

            return true;
        }
    }
}