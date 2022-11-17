using System;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCampApi.Models
{
    public class TalkModel
    {
        public int TalkId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(4000)]
        public string Abstract { get; set; }

        [Range(200, 500)]
        public int Level { get; set; }

        public SpeakerModel Speaker{ get; set; }
    }
}
