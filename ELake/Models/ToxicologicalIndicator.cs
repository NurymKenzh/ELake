using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class ToxicologicalIndicator
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        [Range(1900, 2015)]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NH4")]
        public decimal? NH4 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NO2")]
        public decimal? NO2 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NO3")]
        public decimal? NO3 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PPO4")]
        public decimal? PPO4 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Cu")]
        public decimal? Cu { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Zn")]
        public decimal? Zn { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Mn")]
        public decimal? Mn { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Pb")]
        public decimal? Pb { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Ni")]
        public decimal? Ni { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Cd")]
        public decimal? Cd { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Co")]
        public decimal? Co { get; set; }

        //[Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVNH4")]
        //public decimal? IZVNH4
        //{
        //    get
        //    {
        //        return NH4 / 
        //    }
        //}
    }
}
