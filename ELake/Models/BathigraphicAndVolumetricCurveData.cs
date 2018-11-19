using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class BathigraphicAndVolumetricCurveData
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterLevel")]
        public decimal? WaterLevel { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeArea")]
        public decimal? LakeArea { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterMassVolume")]
        public decimal? WaterMassVolume { get; set; }
    }

    public class BathigraphicAndVolumetricCurveDataIndexPageViewModel
    {
        public IEnumerable<BathigraphicAndVolumetricCurveData> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
