using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace VardaanCab.Controllers
{
    
    public class TestApiController : ApiController
    {
        // GET: TestApi
        class Hero
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public IHttpActionResult GetHeroes()
        {
            var heroes = new List<Hero>
            {
                new Hero { Id=1,Name="Amit"},
                new Hero { Id=2,Name="Nirmal"},
                new Hero { Id=3,Name="Kamlesh"},
                new Hero { Id=4,Name="Roy"},
            };
            return Ok(heroes);
        }
    }
}