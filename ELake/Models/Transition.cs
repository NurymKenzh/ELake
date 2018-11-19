using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class Transition
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NoСhange")]
        public decimal NoСhange { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Permanent")]
        public decimal Permanent { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NewPermanent")]
        public decimal NewPermanent { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LostPermanent")]
        public decimal LostPermanent { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Seasonal")]
        public decimal Seasonal { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NewSeasonal")]
        public decimal NewSeasonal { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LostSeasonal")]
        public decimal LostSeasonal { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SeasonalToPermanent")]
        public decimal SeasonalToPermanent { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PermanentToDeasonal")]
        public decimal PermanentToDeasonal { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "EphemeralPermanent")]
        public decimal EphemeralPermanent { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "EphemeralSeasonal")]
        public decimal EphemeralSeasonal { get; set; }
    }

    public class TransitionIndexPageViewModel
    {
        public IEnumerable<Transition> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
