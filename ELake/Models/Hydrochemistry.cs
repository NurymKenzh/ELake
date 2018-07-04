using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class Hydrochemistry
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Mineralization")]
        public decimal? Mineralization { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "TotalHardness")]
        public decimal? TotalHardness { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DissOxygWater")]
        public decimal? DissOxygWater { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PercentOxygWater")]
        public decimal? PercentOxygWater { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "pH")]
        public decimal? pH { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "OrganicSubstances")]
        public decimal? OrganicSubstances { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Ca")]
        public decimal? Ca { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Mg")]
        public decimal? Mg { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NaK")]
        public decimal? NaK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Cl")]
        public decimal? Cl { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HCO")]
        public decimal? HCO { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SO")]
        public decimal? SO { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NH")]
        public decimal? NH { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NO2")]
        public decimal? NO2 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NO3")]
        public decimal? NO3 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PPO")]
        public decimal? PPO { get; set; }

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

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "CIWP")]
        public decimal? CIWP { get; set; }
    }

    public class HydrochemistryIndexPageViewModel
    {
        public IEnumerable<Hydrochemistry> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
