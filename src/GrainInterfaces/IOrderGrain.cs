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

        Task<bool> AddItem(IItemGrain item);

        Task<bool> RemoveItem(IItemGrain itemGuid);

        Task<bool> DeleteOrder();

        Task<bool> Checkout();
    }
}