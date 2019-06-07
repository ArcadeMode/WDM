using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Orleans;
using Orleans.Providers;
using Orleans.Transactions;
using Orleans.Transactions.Abstractions;

namespace Grains
{
    [StorageProvider(ProviderName="UserStorage")]
    public class UserGrain: Grain<UserState>, IUserGrain
    {

        public UserGrain([TransactionalState("balance")] ITransactionalState<Balance> balance)
        {
            State.UserBalance = balance;
            State.OrderGuids = new List<Guid>();
        }

        /// <summary>
        /// Gets the current credit from the user's balance.
        /// </summary>
        /// <returns>The current user's balance</returns>
        Task<decimal> IUserGrain.GetCredit()
        {
            return Task.FromResult(State.UserBalance.State.Value);
        }

        /// <summary>
        /// Adds or subtracts credit from the user's balance.
        /// </summary>
        /// <param name="amount">Double indicating the change in balance. Positive to add, negative to subtract</param>
        /// <returns>Boolean indicating if the change in balance could be made i.e. there was enough credit.</returns>
        public Task ModifyCredit(decimal amount)
        {
            var balanceAfterChange = State.UserBalance.State.Value + amount;
            if (!(balanceAfterChange > 0))
            {
                throw new Exception("Not enough credits!");
            }
            
            State.UserBalance.State.Value = balanceAfterChange;
            State.UserBalance.Save();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates new OrderGrain for this user and sets it as the active order
        /// </summary>
        /// <returns>GUID of the newly created OrderGrain</returns>
        public async Task<Guid> AddOrder()
        {
            var orderGuid = Guid.NewGuid();
            var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderGuid);
            await orderGrain.SetUser(this);

            State.OrderGuids.Add(orderGuid);
            await WriteStateAsync();

            return orderGuid;
        }

        public async Task<List<Guid>> GetOrders()
        {
            await ReadStateAsync();
            return State.OrderGuids;
        }

        
        /// <summary>
        /// Cancels the currently active order for this user (currently just wipes the order state)
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
//        public async Task<bool> CancelActiveOrder()
//        {
//            var grain = GrainFactory.GetGrain<IOrderGrain>(State.ActiveOrderGuid);
//            await grain.CancelOrder();
//            return true;
//        }
    }
}