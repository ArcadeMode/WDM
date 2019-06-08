using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cart.API.Models
{
    [Serializable]
    public class MessageResult
    {
        public string Message;

        public MessageResult(string message)
        {
            Message = message;
        }
    }
}
