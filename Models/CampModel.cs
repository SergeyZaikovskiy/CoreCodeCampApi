using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCampApi.Models
{
    public class CampModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Moniker { get; set; }

        public string Venue { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string CityTown { get; set; }
        public string StateProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public DateTime EventDate { get; set; } = DateTime.MinValue;

        [Range(1, 100)]
        public int Length { get; set; } = 1;

        public ICollection<TalkModel> Talks{ get; set; }
    }
}
