using System;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans;

namespace GrainInterfaces
{
    public interface IOrderGrain: IGrainWithGuidKey
    {
        Task<bool> SetUser(IUserGrain userGrain);

        Task<Order> GetOrder();

        Task<bool> AddItem(Guid itemGuid);

        Task<bool> RemoveItem(Guid itemGuid);

        Task<bool> CancelOrder();

        [Transaction(TransactionOption.Create)]
        Task<bool> Checkout();
    }
}