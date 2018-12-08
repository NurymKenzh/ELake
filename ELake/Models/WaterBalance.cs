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
                return (SurfaceFlow ?? 0) + (UndergroundFlow ?? 0) + Precipitation;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterBalanceExpenditure")]
        public decimal WaterBalanceExpenditure
        {
            get
            {
                return (SurfaceOutflow ?? 0)  + (UndergroundOutflow ?? 0) + Evaporation;
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

        [Display(Name = "SurfaceFlowPer")]
        public decimal SurfaceFlowPer
        {
            get
            {
                if(WaterBalanceReceipt!=0)
                {
                    return (SurfaceFlow ?? 0) / WaterBalanceReceipt * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Display(Name = "UndergroundFlowPer")]
        public decimal UndergroundFlowPer
        {
            get
            {
                if(WaterBalanceReceipt!=0)
                {
                    return (UndergroundFlow ?? 0) / WaterBalanceReceipt * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Display(Name = "PrecipitationPer")]
        public decimal PrecipitationPer
        {
            get
            {
                if (WaterBalanceReceipt != 0)
                {
                    return Precipitation / WaterBalanceReceipt * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Display(Name = "SurfaceOutflowPer")]
        public decimal SurfaceOutflowPer
        {
            get
            {
                if (WaterBalanceExpenditure != 0)
                {
                    return (SurfaceOutflow ?? 0) / WaterBalanceExpenditure * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Display(Name = "UndergroundOutflowPer")]
        public decimal UndergroundOutflowPer
        {
            get
            {
                if (WaterBalanceExpenditure != 0)
                {
                    return (UndergroundOutflow ?? 0) / WaterBalanceExpenditure * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Display(Name = "EvaporationPer")]
        public decimal EvaporationPer
        {
            get
            {
                if (WaterBalanceExpenditure != 0)
                {
                    return Evaporation / WaterBalanceExpenditure * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Display(Name = "WaterBalanceReceiptPer")]
        public decimal WaterBalanceReceiptPer
        {
            get
            {
                if (WaterBalanceReceipt != 0)
                {
                    return WaterBalanceReceipt / WaterBalanceReceipt * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Display(Name = "WaterBalanceExpenditurePer")]
        public decimal WaterBalanceExpenditurePer
        {
            get
            {
                if (WaterBalanceExpenditure != 0)
                {
                    return WaterBalanceExpenditure / WaterBalanceExpenditure * 100;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    public class WaterBalanceIndexPageViewModel
    {
        public IEnumerable<WaterBalance> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
