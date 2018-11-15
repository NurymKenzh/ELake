using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class WaterBalance
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SurfaceFlow")]
        public decimal? SurfaceFlow { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SurfaceOutflow")]
        public decimal? SurfaceOutflow { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "UndergroundFlow")]
        public decimal? UndergroundFlow { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "UndergroundOutflow")]
        public decimal? UndergroundOutflow { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Precipitation")]
        public decimal Precipitation { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Evaporation")]
        public decimal Evaporation { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterBalanceReceipt")]
        public decimal WaterBalanceReceipt
        {
            get
            {
                return SurfaceFlow ?? 0 + UndergroundFlow ?? 0 + Precipitation;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterBalanceExpenditure")]
        public decimal WaterBalanceExpenditure
        {
            get
            {
                return SurfaceOutflow ?? 0  + UndergroundOutflow ?? 0 + Evaporation;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Discrepancy")]
        public decimal Discrepancy
        {
            get
            {
                return WaterBalanceReceipt - WaterBalanceExpenditure;
            }
        }
    }

    public class WaterBalanceIndexPageViewModel
    {
        public IEnumerable<WaterBalance> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
