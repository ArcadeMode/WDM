using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainInterfaces;
using GrainInterfaces.States;
using Microsoft.AspNetCore.Mvc;
using Orleans;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cart.API.Controllers
{
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IClusterClient _client;

        public UserController(IClusterClient client)
        {
            _client = client;
        }

        [HttpPost("create/{id}")]
        public async Task<Guid> CreateUser(Guid id)
        {
            var grain = _client.GetGrain<IUserGrain>(id);

        }

        [HttpDelete("remove/{id}")]

        
        [HttpGet("find/{id}")]


        [HttpGet("credit/{id}")]


        [HttpPost("credit/substract/{id}/{amount}")]


        [HttpPost("credit/add/{id}/{amount}")]
    }
}
