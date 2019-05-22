using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public class UserGrain: Grain, IUserGrain
    {
        private double userBalance;
        
        public UserGrain()
        {
            userBalance = 0;
        }

        /// <summary>
        /// Adds or subtracts credit from the user's balance.
        /// </summary>
        /// <param name="amount">Double indicating the change in balance. Positive to add, negative to substract</param>
        /// <returns>Boolean indicating if the change in balance could be made i.e. there was enough credit.</returns>
        public async Task<bool> ModifyCredit(double amount)
        {
            var balanceAfterChange = userBalance + amount;
            if (!(balanceAfterChange > 0))
            {
                return false;
            }
            userBalance += amount;
            return true;

        }

        public Task<Guid> AddOrder()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrder(Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}