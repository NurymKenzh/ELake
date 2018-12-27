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

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NoDataPers")]
        public decimal NoDataPers { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NotWater")]
        public decimal NotWater { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SeasonalWaterArea")]
        public decimal SeasonalWaterArea { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PermanentWaterArea")]
        public decimal PermanentWaterArea { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MaximumWaterArea")]
        public decimal MaximumWaterArea
        {
            get
            {
                return SeasonalWaterArea + PermanentWaterArea;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SeasonalWaterAreaPer")]
        public decimal SeasonalWaterAreaPer
        {
            get
            {
                if (MaximumWaterArea != 0)
                {
                    return SeasonalWaterArea / MaximumWaterArea * 100;
                }
                return 0;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PermanentWaterAreaPer")]
        public decimal PermanentWaterAreaPer
        {
            get
            {
                if (MaximumWaterArea != 0)
                {
                    return PermanentWaterArea / MaximumWaterArea * 100;
                }
                return 0;
            }
        }
    }

    public class DynamicsLakeAreaIndexPageViewModel
    {
        public IEnumerable<DynamicsLakeArea> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
