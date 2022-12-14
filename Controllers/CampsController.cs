using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCampApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCodeCampApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        // api/camps?includeTalks=false
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
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

        // api/camps/moiker/atl2018?includeTalks=false where alt2018 is moniker      
        [HttpGet("{moniker}")]       
        public async Task<ActionResult<CampModel>> Get(string moniker, bool includeTalks = false)
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

        [HttpGet("search")]
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

        // api/camps
        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel campModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            try
            {
                var existingCamp = await campRepository.GetCampAsync(campModel.Moniker);

                if (existingCamp != null)
                {
                    return BadRequest("Moniker is already used");
                }

                var location = linkGenerator.GetPathByAction(
                "Get",
                "Camps",
                new { moniker = campModel.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                var camp = mapper.Map<Camp>(campModel);
                campRepository.Add(camp);

                if (await campRepository.SaveChangesAsync())
                {
                    return Created($"/api/camps/{camp.Moniker}", mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"DB connection failure {e.Message}");
            }

            return BadRequest("Camp not added");
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel campModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            try
            {
                var existingCamp = await campRepository.GetCampAsync(campModel.Moniker);

                if (existingCamp == null)  return NotFound($"Could not find camp with moniker of {moniker}");

                mapper.Map(campModel, existingCamp);

                if (await campRepository.SaveChangesAsync())
                {
                    return mapper.Map<CampModel>(existingCamp);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB connection failure");
            }

            return BadRequest("Camp not updated");
        }

        [HttpDelete("{moniker}")]
        public async Task<ActionResult<CampModel>> Delete(string moniker)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker);

                if (camp == null) return NotFound($"Camp with moniker {moniker} has't existed yet");

                campRepository.Delete(camp);

                if (await campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Falied to delete camp");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB connection failure");
            }
        }
    }
}
