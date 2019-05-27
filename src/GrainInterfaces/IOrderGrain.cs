using System;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IOrderGrain: IGrainWithGuidKey
    {
        Task<bool> AddItem(Guid itemGuid);

        Task<Guid> RemoveItem(Guid itemGuid);

        Task<bool> CancelOrder();

        Task<bool> Checkout();
    }
}