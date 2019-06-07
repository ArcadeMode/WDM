using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.Enums;

namespace GrainInterfaces
{
    public interface IPaymentGrain : IGrainWithGuidKey
    {
        Task<PaymentStatus> Pay();

        Task<PaymentStatus> Cancel();

        Task<PaymentStatus> Status();

    }
}
