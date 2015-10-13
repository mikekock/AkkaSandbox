using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AkkaStats.Core.Actors;
using AkkaStats.Core.Messages;

namespace AkkaStats.Api.Controllers
{
    public class StatsController : ApiController
    {

        private readonly IStatsActor _statsActor;

        public StatsController(IStatsActor statsActor)
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
        [Route("api/stats/add/{name}", Name = "Add")]
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


        /// <summary>
        /// Delete all existing records
        /// </summary>
        [HttpGet]
        [Route("api/stats/delete", Name = "Delete")]
        public async Task<IHttpActionResult> Delete()
        {
            await _statsActor.DeleteAllPlayers();
            return Ok(true);
        }


        /// <summary>
        /// Bulk in many players
        /// </summary>
        [HttpGet]
        [Route("api/stats/bulk", Name = "Bulk")]
        public async Task<IHttpActionResult> Bulk()
        {
            List<PlayerMessage> listOfPlayers = new List<PlayerMessage>()
            {
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Barry Bonds" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Barry Zito" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Brandon Belt" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Brandon Crawford" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Brian Wilson" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Bruce Bochey" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Buster Posey" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Cody Ross" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Edger Renteria" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Hunter Pence" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Javier Lopez" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Jake Peavy" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Jeremy Affeldt" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Joe Panik" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Kevin Mitchell" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Madision Bumgardner" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Marco Scutaro" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Matt Cain" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Matt Duffy" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Matt Williams" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Pablo Sandoval" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Ryan Vogelsong" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Sergio Romo" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Tim Lincecum" },
                 new PlayerMessage { Id = Guid.NewGuid(), Name = "Will Clark" }         
            };

            await _statsActor.BulkPlayers(listOfPlayers);

            return Ok(listOfPlayers);
        }


    }
}
