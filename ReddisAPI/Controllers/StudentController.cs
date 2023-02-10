using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReddisAPI.Data;
using ReddisAPI.Models;
using ReddisAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReddisAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentDbContext _studentDbContext;
        private readonly ICacheService _cacheService;

        public StudentController(StudentDbContext studentDbContext , ICacheService cacheService)
        {
            _studentDbContext = studentDbContext;
            _cacheService = cacheService;
        }
        // GET: api/<StudentController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //Check from the cache
            var cacheData = _cacheService.GetData<IEnumerable<Student>>("students");

            if(cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }

            cacheData = await _studentDbContext.Students.ToListAsync();

            //Set expiry time
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);

            _cacheService.SetData<IEnumerable<Student>>("students" , cacheData , expiryTime);

            return Ok(cacheData);



        }

        // GET api/<StudentController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<StudentController>
        [HttpPost]
        public async Task<IActionResult> Post(Student student)
        {
            var addedobject = await _studentDbContext.Students.AddAsync(student);
            //Set expiry time
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);

            _cacheService.SetData<Student>($"student{student.Id}", addedobject.Entity, expiryTime);

            await _studentDbContext.SaveChangesAsync();

            return Ok(addedobject.Entity);

        }

        // PUT api/<StudentController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<StudentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exist = await _studentDbContext.Students.FirstOrDefaultAsync(u => u.Id == id);

            if(exist != null)
            {
                _studentDbContext.Remove(exist);
                _cacheService.RemoveData($"student{id}");
                await _studentDbContext.SaveChangesAsync();

                return NoContent();
            }

            return NotFound();


        }
    }
}
