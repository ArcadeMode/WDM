using System.Collections.Generic;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;

namespace GrainInterfaces
{
    public interface IItemGrain : IGrainWithGuidKey
    {
        Task<ItemState> GetItem();
        
        Task<int> GetAvailability();
        
        Task<bool> ModifyStock(int amount);

        Task<decimal> ModifyPrice(decimal newPrice);

        Task<bool> Delete();

    }
}
