using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CabinsController : ControllerBase
    {
        private readonly CabinService _cabinService;
        public CabinsController(CabinService cabinService)
        {
            _cabinService = cabinService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cabin>>> Get() =>
            await _cabinService.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Cabin>> Get(string id)
        {
            var cabin = await _cabinService.GetByIdAsync(id);
            if (cabin == null) return NotFound();
            return cabin;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Cabin cabin)
        {
            await _cabinService.CreateAsync(cabin);
            return CreatedAtAction(nameof(Get), new { id = cabin.Id }, cabin);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, Cabin cabin)
        {
            var exist = await _cabinService.GetByIdAsync(id);
            if (exist == null) return NotFound();
            await _cabinService.UpdateAsync(id, cabin);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var exist = await _cabinService.GetByIdAsync(id);
            if (exist == null) return NotFound();
            await _cabinService.DeleteAsync(id);
            return NoContent();
        }
    }
}
