using System;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IOrderGrain: IGrainWithGuidKey
    {
        Task<bool> SetUser(IUserGrain userGrain);

        Task<bool> AddItem(Guid itemGuid);

        Task<bool> RemoveItem(Guid itemGuid);

        Task<bool> CancelOrder();

        [Transaction(TransactionOption.Create)]
        Task<bool> Checkout();
    }
}