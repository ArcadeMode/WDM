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
        public override async Task OnActivateAsync()
        {
            if (State.Id == Guid.Empty)
            {
                State.Id = this.GetPrimaryKey();
                State.Balance = 0;
            }
            await base.OnActivateAsync();
        }

        /// <summary>
        /// Gets the current credit from the user's balance.
        /// </summary>
        /// <returns>The current user's balance</returns>
        public async Task<decimal> GetCredit()
        {
            return State.Balance;
        }

        /// <summary>
        /// Adds or subtracts credit from the user's balance.
        /// </summary>
        /// <param name="amount">Decimal indicating the change in balance. Positive to add, negative to subtract</param>
        /// <returns>Boolean indicating if the change in balance could be made i.e. there was enough credit.</returns>
        public async Task<bool> ModifyCredit(decimal amount)
        {
            var balanceAfterChange = State.Balance + amount;
            if (balanceAfterChange < 0)
            {
                return false;
                //throw new Exception("Not enough credits!");
            }
            State.Balance += amount;
            await WriteStateAsync();
            return true;
        }
        
        /// <summary>
        /// Deletes the user by wiping its persisted state.
        /// AzureTableGrainStorage has a bool property DeleteStateOnClear that determines the database behaviour of this method
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public async Task DeleteUser()
        {
            await ClearStateAsync();
            //TODO: How to check if this was successful?
        }

        public async Task<UserState> GetState()
        {
            return State;
        }
    }
}