using System;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Cart.API.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly IClusterClient _client;
        
        public UserController(IClusterClient client)
        {
            _client = client;
        }
      
        /// <summary>
        /// Creates an User and returns its GUID
        /// </summary>
        /// <returns>GUID of the newly created User</returns>
        [HttpPost("create")]
        public Guid CreateUser()
        {
       
            var grain = _client.GetGrain<IUserGrain>(Guid.NewGuid());
            return grain.GetGrainIdentity().PrimaryKey;
        }
        
        /// <summary>
        /// Clears the persisted state of a user.
        /// </summary>
        /// <param name="id">GUID of user to delete</param>
        /// <returns></returns>
        [HttpDelete("remove/{id}")]
        public async Task RemoveUser(Guid id)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            await grain.DeleteUser();
            //TODO: How to check if this was successful?
        }
        
        /// <summary>
        /// Find and return details about the user
        /// </summary>
        /// <returns>GUID and balance of the user</returns>
        [HttpGet("find/{id}")]
        public UserState FindUser(Guid id)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            return grain.GetState().Result;
        }
        
        /// <summary>
        /// Returns the credit of the specified user
        /// </summary>
        /// <returns>Decimal with user credit</returns>
        [HttpGet("credit/{id}")]
        public decimal GetUserCredit(Guid id)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            return grain.GetState().Result.UserBalance;
        }
        

        /// <summary>
        /// Subtracts balance from the current user
        /// </summary>
        /// <param name="id">GUID of the user to subtract balance from</param>
        /// <param name="amount">Positive decimal to subtract</param>
        /// <returns>Boolean indicating if action was successful</returns>
        [HttpPost("credit/subtract/{id}/{amount}")]
        public async Task<bool> SubtractBalance(Guid id, decimal amount)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            return await grain.ModifyCredit(-1*amount);
        }
        
        /// <summary>
        /// Adds balance from the current user
        /// </summary>
        /// <param name="id">GUID of the user to ad balance to</param>
        /// <param name="amount">Positive decimal to subtract</param>
        /// <returns>Boolean indicating if action was successful</returns>
        [HttpPost("credit/add/{id}/{amount}")]
        public async Task<bool> AddBalance(Guid id, decimal amount)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            return await grain.ModifyCredit(amount);
        }
    }
}
