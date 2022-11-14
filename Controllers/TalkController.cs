using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCampApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CoreCodeCampApi.Controllers
{
    [Route("api/{moniker}/[controller]")]
    public class TalkController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;

        public TalkController(ICampRepository campRepository, IMapper mapper)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await campRepository.GetTalksByMonikerAsync(moniker);

                if (talks == null) return NotFound();

                return mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB connection failure");
            }
        }

        [HttpGet("id:int")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id);

                if (talk == null) return NotFound();

                return mapper.Map<TalkModel>(talk);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB connection failure");
            }
        }

        [HttpDelete("id:int")]
        public async Task<ActionResult> Delete(string moniker,  int id)
        {

        }

        [HttpPut("id:int")]
        public async Task<ActionResult> Put(string moniker, int id)
        {

        }

        [HttpPost]
        public async Task<ActionResult> Post(string moniker)
        {

        }
    }
}
