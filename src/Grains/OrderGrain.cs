using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Orleans;
using Orleans.Providers;

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
            //List of all items in the order
            var orderItems = State.Items;
            //Tasks to do after checks
            List<Task> tasks = new List<Task>();
            
            decimal totalSum = 0;

            //For each item in order
            foreach(KeyValuePair<Guid, int> kvp in orderItems )
            {
                var itemKey = kvp.Key;
                var itemGrain = GrainFactory.GetGrain<IItemGrain>(itemKey);
                var item = await itemGrain.GetItem();
                
                //Check if stock of item is at least as much as the ordered amount
                if(item.Stock >= kvp.Value) {
                    //add its price times the amount to the total sum of the order
                   totalSum += item.Price*kvp.Value;
                   
                   //Add subtracting the stock of current item to list of tasks to perform
                   tasks.Add(itemGrain.ModifyStock(-1*kvp.Value));
                } else
                {
                    throw new Exception($"Stock of item {kvp.Key} is lower than {kvp.Value}.");
                }
             }

            var userGrain = GrainFactory.GetGrain<IUserGrain>(State.UserId);

            //Get the current credit of the user
            var credit = await userGrain.GetCredit();
            
            //Check if user's credit is enough to pay for all ordered products
            if(credit >= totalSum)
            {
                //Add subtracting the credit of the user to list of tasks to perform
                tasks.Add(userGrain.ModifyCredit(-1 * totalSum));
            } else
            {
                throw new Exception($"Balance of user {userGrain.GetPrimaryKey()} is lower than {totalSum}.");
            }
            
            //Create a new payment grain and set it to "paid"
            var paymentGrain = GrainFactory.GetGrain<IPaymentGrain>(Guid.NewGuid());
            tasks.Add(paymentGrain.Pay());

            //Complete all tasks
            await Task.WhenAll(tasks);

            return true;
        }
    }
}