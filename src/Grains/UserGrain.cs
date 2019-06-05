using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Orleans;
using Orleans.Providers;

namespace Grains
{
    [StorageProvider(ProviderName="UserStorage")]
    public class UserGrain: Grain<UserState>, IUserGrain
    {
        public UserGrain()
        {
            State.UserBalance = 0;
            State.CompletedOrderGuids = new List<Guid>();
            State.ActiveOrderGuid = AddOrder().Result;
        }

        /// <summary>
        /// Adds or subtracts credit from the user's balance.
        /// </summary>
        /// <param name="amount">Double indicating the change in balance. Positive to add, negative to subtract</param>
        /// <returns>Boolean indicating if the change in balance could be made i.e. there was enough credit.</returns>
        public async Task<bool> ModifyCredit(double amount)
        {
            var balanceAfterChange = State.UserBalance + amount;
            if (!(balanceAfterChange > 0))
            {
                return false;
            }
            State.UserBalance += amount;
            await WriteStateAsync();
            return true;
        }

        /// <summary>
        /// Creates new OrderGrain for this user and sets it as the active order
        /// </summary>
        /// <returns>GUID of the newly created OrderGrain</returns>
        public async Task<Guid> AddOrder()
        {
            var orderGuid = Guid.NewGuid();
            GrainFactory.GetGrain<IOrderGrain>(orderGuid);
            State.ActiveOrderGuid = orderGuid;
            await WriteStateAsync();
            return orderGuid;
        }

        
        /// <summary>
        /// Cancels the currently active order for this user (currently just wipes the order state)
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public async Task<bool> CancelActiveOrder()
        {
            var grain = GrainFactory.GetGrain<IOrderGrain>(State.ActiveOrderGuid);
            await grain.CancelOrder();
            return true;
        }
    }
}