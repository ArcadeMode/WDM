<<<<<<< HEAD
=======
using System;
using System.Collections.Generic;
>>>>>>> origin/order_branch
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans;

namespace GrainInterfaces
{
    public interface IUserGrain: IGrainWithGuidKey
    {
        Task<bool> ModifyCredit(decimal amount);

        Task DeleteUser();

        Task<UserState> GetState();

        Task<decimal> GetCredit();

    }
}