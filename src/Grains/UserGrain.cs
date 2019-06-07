using System;
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

        /// <summary>
        /// Gets the current credit from the user's balance.
        /// </summary>
        /// <returns>The current user's balance</returns>
        Task<decimal> IUserGrain.GetCredit()
        {
            return Task.FromResult(State.Balance);
        }

        /// <summary>
        /// Adds or subtracts credit from the user's balance.
        /// </summary>
        /// <param name="amount">Double indicating the change in balance. Positive to add, negative to subtract</param>
        /// <returns>Boolean indicating if the change in balance could be made i.e. there was enough credit.</returns>
        public Task ModifyCredit(decimal amount)
        {
            var balanceAfterChange = State.Balance + amount;
            if (!(balanceAfterChange > 0))
            {
                throw new Exception("Not enough credits!");
            }
            
            State.Balance = balanceAfterChange;
            return Task.CompletedTask;
        }
    }
}