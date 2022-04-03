﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SamplesController : ControllerBase
    {
        // GET: api/<SamplesController>
        [HttpGet]
        public string Get()
        {
            return "Hello from the user side";
        }

        // GET api/<SamplesController>/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "Hello from the admin side";
        }

        // POST api/<SamplesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SamplesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SamplesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
