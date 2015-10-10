using System;
using System.Threading.Tasks;
using System.Web.Http;
using AkkaStats.Core;
using AkkaStats.Core.Messages;

namespace AkkaStats.Api.Controllers
{
    public class ValuesController : ApiController
    {

        private StatsActorSystemService _statsActorSystemService;

        public ValuesController()
        {
            _statsActorSystemService = new StatsActorSystemService();
        }

        // GET api/values
        [HttpGet]
        [Route("api/stats/add/{name}")]
        public async Task<IHttpActionResult> Add(string name)
        {
            var newPlayer = new PlayerMessage() { Id = Guid.NewGuid(), Name = name };
            _statsActorSystemService.AddPlayer(newPlayer);

            return Ok(newPlayer);
        }

        [HttpGet]
        [Route("api/stats/{id}")]
        public async Task<IHttpActionResult> Get(string id)
        {
          
            var result = await _statsActorSystemService.GetById(Guid.Parse(id));

            return Ok(result);
        }
    
    }
}
