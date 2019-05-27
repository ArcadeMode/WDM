using System;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IUserGrain: IGrainWithGuidKey
    {
        Task<bool> ModifyCredit(double amount);

        Task<Guid> AddOrder();

        Task<bool> CancelActiveOrder();
    }
}