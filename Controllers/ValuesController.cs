using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using hwwebapi.Core;

namespace hwwebapi.Controllers {

    [Route("api/[controller]")]
    public class ValuesController : Controller {

        private readonly IRepository<int, string> repository;

        public ValuesController(IRepository<int, string> repository) {
            if (repository == null) {
                throw new ArgumentNullException("repository");
            }

            this.repository = repository;
        }

        // GET api/values
        [HttpGet]
        [Authorize]
        public IActionResult GetAll() {
            return Ok(this.repository.GetAll());
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetValue")]
        public IActionResult Get(int id) {
            string value;

            if (!this.repository.TryGet(id, out value)) {
                return NotFound();
            }

            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Create([FromBody] string value) {
            if (String.IsNullOrWhiteSpace(value)) {
                return BadRequest();
            }

            var createdId = this.repository.Create(value);

            return CreatedAtRoute("GetValue", new { id = createdId }, value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value) {
            if (String.IsNullOrWhiteSpace(value)) {
                return BadRequest();
            }

            if (!this.repository.TryUpdate(id, value)) {
                return NotFound();
            }

            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            if (!this.repository.Delete(id)) {
                return NotFound();
            }

            return Ok();
        }

    }

}
