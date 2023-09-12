using Microsoft.AspNetCore.Mvc;
using Medals.Models;

namespace Medals.Controllers
{
    [ApiController, Route("[controller]/country")]
    public class ApiController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public ApiController(DataContext db)
        {
            _dataContext = db;
        }
        // http get entire collection
        [HttpGet]
        public IEnumerable<Country> Get()
        {
            return _dataContext.Countries;
        }
        // http get specific member of collection
        [HttpGet("{id}")]
        public Country Get(int id)
        {
            return _dataContext.Countries.Find(id);
        }
        // http post member to collection
        [HttpPost]
        public async Task<ActionResult<Country>> Post([FromBody] Country country) {
            _dataContext.Add(country);
            await _dataContext.SaveChangesAsync();
            return country;
        }
        // http delete member from collection
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id){
            Country country = await _dataContext.Countries.FindAsync(id);
            if (country == null){
                return NotFound();
            }
            _dataContext.Remove(country);
            await _dataContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
