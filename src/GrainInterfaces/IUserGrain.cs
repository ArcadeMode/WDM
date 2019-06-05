using System;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IUserGrain: IGrainWithGuidKey
    {
        [Transaction(TransactionOption.CreateOrJoin)]
        Task<int> GetCredit();

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<bool> ModifyCredit(decimal amount);

        Task<Guid> AddOrder();

        Task<bool> CancelActiveOrder();
    }
}