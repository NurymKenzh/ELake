using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class DynamicsLakeArea
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NoData")]
        public decimal NoData { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NoWater")]
        public decimal NoWater { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SeasonalWaterArea")]
        public decimal SeasonalWaterArea { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PermanentWaterArea")]
        public decimal PermanentWaterArea { get; set; }
    }

    public class DynamicsLakeAreaIndexPageViewModel
    {
        public IEnumerable<DynamicsLakeArea> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
