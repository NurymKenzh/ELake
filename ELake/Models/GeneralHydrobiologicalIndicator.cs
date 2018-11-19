using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class GeneralHydrobiologicalIndicator
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FishId")]
        public int FishId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SurveyYear")]
        public int SurveyYear { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ZooplanktonBiomass")]
        public decimal? ZooplanktonBiomass { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "BenthosBiomass")]
        public decimal? BenthosBiomass { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SpeciesWealthIndex")]
        public decimal? SpeciesWealthIndex { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FishCatchLimit")]
        public decimal? FishCatchLimit { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "CurrentCommercialFishProductivity")]
        public decimal? CurrentCommercialFishProductivity { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PotentialFishProducts")]
        public decimal? PotentialFishProducts { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PotentialGrowingVolume")]
        public decimal? PotentialGrowingVolume { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "CurrentUsage")]
        public decimal? CurrentUsage { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "RecommendedUse")]
        public decimal? RecommendedUse { get; set; }
    }

    public class GeneralHydrobiologicalIndicatorIndexPageViewModel
    {
        public IEnumerable<GeneralHydrobiologicalIndicator> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
