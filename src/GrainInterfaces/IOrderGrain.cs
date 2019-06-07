using System;
using System.Threading.Tasks;
using GrainInterfaces.States;
using Orleans;

namespace GrainInterfaces
{
    public interface IOrderGrain: IGrainWithGuidKey
    {
        Task<OrderState> GetOrder();
        
        Task<bool> SetUser(IUserGrain userGrain);

        Task<bool> AddItem(Guid itemGuid);

        Task<bool> RemoveItem(Guid itemGuid);

        Task<bool> CancelOrder();

        Task<bool> Checkout();
    }
}