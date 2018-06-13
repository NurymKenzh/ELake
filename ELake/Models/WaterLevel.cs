using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class WaterLevel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterLavelM")]
        public decimal WaterLavelM { get; set; }
    }

    public class WaterLevelIndexPageViewModel
    {
        public IEnumerable<WaterLevel> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
