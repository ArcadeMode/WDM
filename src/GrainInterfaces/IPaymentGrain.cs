using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.Enums;
using GrainInterfaces.States;

namespace GrainInterfaces
{
    public interface IPaymentGrain : IGrainWithGuidKey
    {
        Task<PaymentStatus> Pay(IUserGrain user, decimal amount);

        Task<PaymentStatus> Cancel();

        Task<PaymentStatus> Status();

    }
}
