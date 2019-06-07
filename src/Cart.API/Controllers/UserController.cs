using System;
using System.Threading.Tasks;
using Cart.API.Models;
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
        [HttpPost("")]
        public ActionResult CreateUser()
        {
            var grain = _client.GetGrain<IUserGrain>(Guid.NewGuid());
            return Ok(new MessageResult(grain.GetGrainIdentity().PrimaryKey.ToString()));
        }
        
        /// <summary>
        /// Clears the persisted state of a user.
        /// </summary>
        /// <param name="id">GUID of user to delete</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveUser(Guid id)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            await grain.DeleteUser();
            return Ok($"User {id} deleted");
            //TODO: How to check if this was successful?
        }
        
        /// <summary>
        /// Find and return details about the user
        /// </summary>
        /// <returns>GUID and balance of the user</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> FindUser(Guid id)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            var grainState = await grain.GetState();
            if (grainState.Id == Guid.Empty)
            {
                return NotFound(new MessageResult("User not found"));
            }
            return Ok(grainState);
        }
        
        /// <summary>
        /// Returns the credit of the specified user
        /// </summary>
        /// <returns>Decimal with user credit</returns>
        [HttpGet("{id}/credit")]
        public async Task<ActionResult> GetUserCredit(Guid id)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            return Ok(new MessageResult((await grain.GetState()).Balance.ToString()));
        }
        

        /// <summary>
        /// Subtracts balance from the current user
        /// </summary>
        /// <param name="id">GUID of the user to subtract balance from</param>
        /// <param name="amount">Positive decimal to subtract</param>
        /// <returns>Boolean indicating if action was successful</returns>
        [HttpPut("{id}/credit/subtract/{amount}")]
        public async Task<ActionResult> SubtractBalance(Guid id, decimal amount)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            if(await grain.ModifyCredit(-1 * amount))
            {
                return Ok(await grain.GetState());
            }
            return BadRequest(new MessageResult("Insufficient credit"));
        }
        
        /// <summary>
        /// Adds balance from the current user
        /// </summary>
        /// <param name="id">GUID of the user to ad balance to</param>
        /// <param name="amount">Positive decimal to subtract</param>
        /// <returns>Boolean indicating if action was successful</returns>
        [HttpPut("{id}/credit/add/{amount}")]
        public async Task<ActionResult> AddBalance(Guid id, decimal amount)
        {
            var grain = _client.GetGrain<IUserGrain>(id);
            if (await grain.ModifyCredit(amount))
            {
                return Ok(await grain.GetState());
            }
            return BadRequest(new MessageResult("Insufficient credit"));
        }
    }
}
