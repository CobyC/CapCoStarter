using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapCoStarter.Api.Data;

namespace CapCoStarter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestDataEntitiesController : ControllerBase
    {
        private readonly AppDataDbContext _context;

        public TestDataEntitiesController(AppDataDbContext context)
        {
            _context = context;
        }

        // GET: api/TestDataEntities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestDataEntity>>> GetTestDataEntity()
        {
            return await _context.TestDataEntity.ToListAsync();
        }

        // GET: api/TestDataEntities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TestDataEntity>> GetTestDataEntity(int id)
        {
            var testDataEntity = await _context.TestDataEntity.FindAsync(id);

            if (testDataEntity == null)
            {
                return NotFound();
            }

            return testDataEntity;
        }

        // PUT: api/TestDataEntities/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTestDataEntity(int id, TestDataEntity testDataEntity)
        {
            if (id != testDataEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(testDataEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestDataEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TestDataEntities
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TestDataEntity>> PostTestDataEntity(TestDataEntity testDataEntity)
        {
            _context.TestDataEntity.Add(testDataEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestDataEntity", new { id = testDataEntity.Id }, testDataEntity);
        }

        // DELETE: api/TestDataEntities/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TestDataEntity>> DeleteTestDataEntity(int id)
        {
            var testDataEntity = await _context.TestDataEntity.FindAsync(id);
            if (testDataEntity == null)
            {
                return NotFound();
            }

            _context.TestDataEntity.Remove(testDataEntity);
            await _context.SaveChangesAsync();

            return testDataEntity;
        }

        private bool TestDataEntityExists(int id)
        {
            return _context.TestDataEntity.Any(e => e.Id == id);
        }
    }
}
