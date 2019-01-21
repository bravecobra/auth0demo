using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace pharmacyAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> _logger;
        private static ConcurrentDictionary<int, string> data = new ConcurrentDictionary<int, string>();

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
            var user = User;
            if (data.Any())
            {
                return;
            }

            data.TryAdd(1, "value1");
            data.TryAdd(2, "value2");
            data.TryAdd(3, "value3");
        }

        [HttpGet]
        [Authorize(Policy = "Guest")]
        public IActionResult Get()
        {
            foreach (var dataValue in data.Values)
                _logger.LogDebug(dataValue);
            return Ok(data.Values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Authorize(Policy = "Guest")]
        public string Get(int id)
        {
            data.TryGetValue(id, out var value);
            _logger.LogDebug(value);
            return value;
        }

        // POST api/values
        [HttpPost]
        [Authorize(Policy = "Administrator")]
        public void Post([FromBody] Payload payload)
        {
            data.TryAdd(payload.id, payload.Value);
            _logger.LogDebug($"Added value {payload.Value}");
        }

        // PUT api/values/5
        [HttpPut]
        [Authorize(Policy = "Administrator")]
        public void Put([FromBody] Payload payload)
        {
            data.AddOrUpdate(payload.id, payload.Value, (k, v) => payload.Value);
            _logger.LogDebug($"Added/Updated value {payload.Value}");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "Administrator")]
        public void Delete(int id)
        {
            data.Remove(id, out var value);
            _logger.LogDebug($"Deleted value with id {id}");
        }
    }
}
