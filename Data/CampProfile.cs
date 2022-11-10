using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCampApi.Models;

namespace CoreCodeCampApi.Data
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            this.CreateMap<Camp, CampModel>()
                .ForMember(c => c.Venue, o => o.MapFrom(m => m.Location.VenueName));
        }
    }
}
