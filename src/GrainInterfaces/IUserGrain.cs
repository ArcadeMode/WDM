using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans;

namespace GrainInterfaces
{
    public interface IUserGrain: IGrainWithGuidKey
    {
        Task<decimal> GetCredit();

        Task ModifyCredit(decimal amount);
    }
}