using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class ZonalStatWater
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Type")]
        public string Type { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int? Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Month")]
        public int? Month { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "RasterValue")]
        public int RasterValue { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "RasterValueStat")]
        public decimal RasterValueStat { get; set; }
    }
}
