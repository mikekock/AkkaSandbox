using System;
using System.Threading.Tasks;
using System.Web.Http;
using AkkaStats.Core;
using AkkaStats.Core.Messages;

namespace AkkaStats.Api.Controllers
{
    public class ValuesController : ApiController
    {

        private readonly IStatsActor _statsActor;

        public ValuesController(IStatsActor statsActor)
        {
            _statsActor = statsActor;
        }

        /// <summary>
        /// Get An Records Array
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/stats", Name = "GetAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            var results = await _statsActor.GetAll();
            if (results == null) return NotFound();
            // Used to make a simple link from the json object to the GetById Route
            foreach (var result in results)
            {
                result.Url = String.Format("{0}{1}", "http://localhost:49781", Url.Route("GetById", new { id = result.Id }));
            }
            return Ok(results);
        }

        /// <summary>
        /// Add an entity and get the record back
        /// </summary>
        [HttpGet]
        [Route("api/stats/add/{name}", Name="Add")]
        public async Task<IHttpActionResult> Add(string name)
        {
            var newPlayer = new PlayerMessage() { Id = Guid.NewGuid(), Name = name };
            await _statsActor.AddPlayer(newPlayer);

            return Ok(newPlayer);
        }

        /// <summary>
        /// Get an entity by its identity
        /// </summary>
        [HttpGet]
        [Route("api/stats/{id}", Name = "GetById")]
        public async Task<IHttpActionResult> GetById(string id)
        {    
            var result = await _statsActor.GetById(id);
            return Ok(result);
        }



    }
}
