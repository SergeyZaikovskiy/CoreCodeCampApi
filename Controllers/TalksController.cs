using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCampApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace CoreCodeCampApi.Controllers
{
    [Route("api/camps/{moniker}/[controller]")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await campRepository.GetTalksByMonikerAsync(moniker, true);

                if (talks == null) return NotFound();

                return mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talks");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id);

                if (talk == null) return NotFound($"Could not found talk with id equal {id}");

                return mapper.Map<TalkModel>(talk);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talk");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(string moniker, int id)
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound("Failed to find talk");
                campRepository.Delete(talk);

                if (await campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Falied to delete talk");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete talk");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, [FromBody] TalkModel talkModel )
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound("Couldn't find the talk");

                mapper.Map(talkModel, talk);

                if (talkModel.Speaker != null)
                {
                    var speaker = await campRepository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                    
                    if (speaker != null)
                    {
                        talk.Speaker = speaker;
                    }
                }

                if (await campRepository.SaveChangesAsync())
                {
                    return mapper.Map<TalkModel>(talk);
                }
                else
                {
                    return BadRequest("Failed to update DB");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Failed to update talk");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, [FromBody]TalkModel talkModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            try
            {
                var camp = await campRepository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Camp hasn't been existed yet");

                var talk = mapper.Map<Talk>(talkModel);
                talk.Camp = camp;

                if (talkModel.Speaker == null) return BadRequest("Speaker ID is required");
                var speaker = await campRepository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                if(speaker == null) return NotFound("Speaker could not found");
                talk.Speaker = speaker;

                campRepository.Add(talk);

                if (await campRepository.SaveChangesAsync())
                {
                    var url = linkGenerator.GetPathByAction(
                        HttpContext,
                        "Get",
                        values: new {moniker, id = talk.TalkId});

                    return Created(url, mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("Failed to save new Talk");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "DB connection failure");
            }
        }
    }
}
