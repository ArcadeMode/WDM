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
        public async Task<bool> SetUser(IUserGrain userGrain) {
            State.Value.User = userGrain;

            await WriteStateAsync();

            return true;
        }

        public async Task<Order> GetOrder()
        {
            await ReadStateAsync();
            
            if (State.Value != null) return State.Value;

            State.Value = new Order
            {
                Id =  Guid.NewGuid(),
                Items = new Dictionary<Guid, int>()
            };

            await WriteStateAsync();
            
            return State.Value;
        }

        public async Task<bool> AddItem(Guid itemGuid)
        {
            await ReadStateAsync();

            if (State.Value == null)
            {
                State = new OrderState();
            }

            State.Value.Items.TryGetValue(itemGuid, out var currentCount); 
            State.Value.Items[itemGuid] = currentCount + 1;

            await WriteStateAsync();

            return true;
        }

        public async Task<bool> RemoveItem(Guid itemGuid)
        {
            await ReadStateAsync();

            if (State.Value == null)
            {
                State = new OrderState();
            }

            State.Value.Items.Remove(itemGuid);

            await WriteStateAsync();

            return true;
        }

        public async Task<bool> CancelOrder()
        {
            await ClearStateAsync();
            return true;
        }

        public async Task<bool> Checkout()
        {
            //List of all items in the order
            var orderItems = State.Value.Items;
            
            //Tasks to do after checks
            List<Task> tasks = new List<Task>();
            
            decimal totalSum = 0;

            //For each item in order
            foreach(KeyValuePair<Guid, int> kvp in orderItems )
            {
                var itemGrain = GrainFactory.GetGrain<IItemGrain>(kvp.Key);
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
            
            //Get the current credit of the user
            var credit = await State.Value.User.GetCredit();
            
            //Check if user's credit is enough to pay for all ordered products
            if(credit >= totalSum)
            {
                //Add subtracting the credit of the user to list of tasks to perform
                tasks.Add(State.Value.User.ModifyCredit(-1 * totalSum));
            } else
            {
                throw new Exception($"Balance of user {State.Value.User.GetPrimaryKey()} is lower than {totalSum}.");
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