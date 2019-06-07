using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans;

namespace GrainInterfaces
{
    public interface IUserGrain: IGrainWithGuidKey
    {
        [Transaction(TransactionOption.RequiresNew)]
        Task<decimal> GetCredit();

        [Transaction(TransactionOption.RequiresNew)]
        Task ModifyCredit(decimal amount);

        Task<Guid> AddOrder();

        Task<List<Guid>> GetOrders();

//        Task<bool> CancelActiveOrder();
    }
}