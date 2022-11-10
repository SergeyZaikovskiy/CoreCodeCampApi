using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCampApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCampApi.Controllers
{
    [Route("api/[controller]")]
    public class CapmsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;

        public CapmsController(ICampRepository campRepository, IMapper mapper)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetCamps(bool includeTalks = false)
        {
            try
            {
                var allCamps = await campRepository.GetAllCampsAsync(includeTalks);
                return mapper.Map<CampModel[]>(allCamps);
            } 
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,"DB connection failure");
            }
        }

        [HttpGet("{moniker}")]       
        public async Task<ActionResult<CampModel>> GetCamps(string moniker, bool includeTalks = false)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker, includeTalks);

                if (camp == null) return NotFound();

                return mapper.Map<CampModel>(camp);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB connection failure");
            }
        }

        [HttpGet("{dateTime}")]
        public async Task<ActionResult<CampModel>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var camp = await campRepository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!camp.Any()) return NotFound();

                return mapper.Map<CampModel>(camp);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB connection failure");
            }
        }
    }
}
