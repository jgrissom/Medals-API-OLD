using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medals_API.Models;

namespace Medals_API.Controllers
{
    [ApiController, Route("[controller]/country")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private DataContext _dataContext;

        public ApiController(ILogger<ApiController> logger, DataContext db)
        {
            _dataContext = db;
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<Country> Get()
        {
            return _dataContext.Countries;
        }
        [HttpGet("{id}")]
        public Country Get(int id)
        {
            return _dataContext.Countries.Find(id);
        }
    }
}
