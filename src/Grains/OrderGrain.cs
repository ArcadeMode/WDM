using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Orleans;
using Orleans.Providers;
using GrainInterfaces.Enums;

namespace Grains
{
    [StorageProvider(ProviderName="OrderStorage")]
    public class OrderGrain: Grain<OrderState>, IOrderGrain
    {

        public override async Task OnActivateAsync()
        {
            await ReadStateAsync();

            State = State.Id != Guid.Empty ? State : new OrderState
            {
                Id = this.GetPrimaryKey(),
                UserId = Guid.Empty,
                PaymentId = Guid.Empty,
                Items = new Dictionary<Guid, int>()
            };
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await WriteStateAsync();
            await base.OnDeactivateAsync();
        }

        public async Task<bool> SetUser(IUserGrain userGrain) {
            State.UserId = userGrain.GetPrimaryKey();
            return true;
        }

        public async Task<OrderState> GetOrder()
        {            
            return State;
        }

        public async Task<bool> AddItem(IItemGrain item)
        {
            var itemKey = item.GetPrimaryKey();
            if(State.Items.TryGetValue(itemKey, out var currentCount))
            {
                State.Items[itemKey] = currentCount + 1;
            } else
            {
                State.Items.Add(itemKey, 1);
            }
            return true;
        }

        public async Task<bool> RemoveItem(IItemGrain item)
        {
            return State.Items.Remove(item.GetPrimaryKey());
        }

        public async Task<bool> DeleteOrder()
        {
            await ClearStateAsync(); 
            //TODO: dont clear, thats like destroying the order, do cancel on payment instead
            return true;
        }

        public async Task<bool> Checkout()
        {
            var paymentGrain = GrainFactory.GetGrain<IPaymentGrain>(State.PaymentId == Guid.Empty ? Guid.NewGuid() : State.PaymentId);
            State.PaymentId = paymentGrain.GetPrimaryKey();
            if(await paymentGrain.Status() != PaymentStatus.Pending)
            {
                return false; //order already processed
            }

            decimal totalSum = 0;
            var orderItems = State.Items;
            var processedItems = new List<KeyValuePair<Guid, int>>();
            bool doRollback = false;
            foreach (KeyValuePair<Guid, int> kvp in orderItems )
            {
                var itemKey = kvp.Key;
                var itemCount = kvp.Value;
                var itemGrain = GrainFactory.GetGrain<IItemGrain>(itemKey);
                var itemState = await itemGrain.GetItem();
                if(itemState.Price == 0)
                {
                    await RemoveItem(itemGrain);
                    continue; //skip item if its considered deleted and remove it from the order
                }
                if(await itemGrain.ModifyStock(-1 * itemCount)) { //0 price indicates deleted item
                    totalSum += itemState.Price * itemCount;
                    processedItems.Add(kvp);
                } else
                {   //insufficient stock
                    doRollback = true;
                    break;
                }
            }
            if(doRollback)
            {
                await RollBackStockChanges(processedItems);
                return false;
            }

            var userGrain = GrainFactory.GetGrain<IUserGrain>(State.UserId);
            if(await paymentGrain.Pay(userGrain, totalSum) == PaymentStatus.Paid)
            {
                //payment succesfull & stock subtracted succesfull
                return true;
            } 
            //insufficient credits
            await RollBackStockChanges(processedItems);
            return false;
        }

        private async Task<bool> RollBackStockChanges(List<KeyValuePair<Guid, int>> processedItems)
        {
            bool res = true;
            foreach (KeyValuePair<Guid, int> kvp in processedItems)
            {
                var itemKey = kvp.Key;
                var itemCount = kvp.Value;
                var itemGrain = GrainFactory.GetGrain<IItemGrain>(itemKey);
                res = res && await itemGrain.ModifyStock(itemCount);
            }
            return res;
        }
    }
}