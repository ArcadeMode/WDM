using System.Collections.Generic;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;

namespace GrainInterfaces
{
    public interface IItemGrain : IGrainWithGuidKey
    {
        Task<Item> GetItem();
        
        Task<int> GetAvailability();
        
        Task ModifyStock(int amount);
    }
}
