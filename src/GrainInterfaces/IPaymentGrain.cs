using System;
using System.Collections.Generic;
using System.Text;
using Orleans;
using System.Threading.Tasks;
using GrainInterfaces.States;

namespace GrainInterfaces
{
    public interface IPaymentGrain : IGrainWithGuidKey
    {
        Task<int> Pay();

        Task<int> Cancel();

        Task<int> Status();

    }
}
