using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class Lake
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameKK")]
        public string NameKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameRU")]
        public string NameRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameEN")]
        public string NameEN { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    name = NameEN;
                if (language == "kk")
                {
                    name = NameKK;
                }
                if (language == "ru")
                {
                    name = NameRU;
                }
                return name;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "VHBKK")]
        public string VHBKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeSystemId")]
        public int? LakeSystemId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "VHBRU")]
        public string VHBRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "VHBEN")]
        public string VHBEN { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "VHB")]
        public string VHB
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    vhb = VHBEN;
                if (language == "kk")
                {
                    vhb = VHBKK;
                }
                if (language == "ru")
                {
                    vhb = VHBRU;
                }
                return vhb;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "VHU")]
        public string VHU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Area2015")]
        public decimal Area2015 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeShorelineLength2015")]
        public decimal LakeShorelineLength2015 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Longitude")]
        public string Longitude { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Latitude")]
        public string Latitude { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ShoreIrregularityRatio")]
        public decimal ShoreIrregularityRatio
        {
            get
            {
                return LakeShorelineLength2015 / (decimal)(2 * Math.PI) / (decimal) Math.Sqrt((double)(Area2015 / (decimal) Math.PI));
            }
        }

        [NotMapped]
        public decimal? LakesArchiveDataLakeLength { get; set; }
        [NotMapped]
        public decimal? LakesArchiveDataLakeShorelineLength { get; set; }
        [NotMapped]
        public decimal? LakesArchiveDataLakeMirrorArea { get; set; }
        [NotMapped]
        public decimal? LakesArchiveDataLakeAbsoluteHeight { get; set; }
        [NotMapped]
        public decimal? LakesArchiveDataLakeWidth { get; set; }
        [NotMapped]
        public decimal? LakesArchiveDataLakeMaxDepth { get; set; }
        [NotMapped]
        public decimal? LakesArchiveDataLakeWaterMass { get; set; }

        [NotMapped]
        public decimal? LakesGlobalDataLake_area { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataShore_len { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataShore_dev { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataVol_total { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataDepth_avg { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataDis_avg { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataRes_time { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataElevation { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataSlope_100 { get; set; }
        [NotMapped]
        public decimal? LakesGlobalDataWshd_area { get; set; }

        [NotMapped]
        public decimal? ToxicKIZV { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceFlowAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceOutflowAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundFlowAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundOutflowAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalancePrecipitationAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceEvaporationAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceFlowMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceOutflowMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundFlowMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundOutflowMax { get; set; }
        [NotMapped]
        public decimal? WaterBalancePrecipitationMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceEvaporationMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceFlowMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceOutflowMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundFlowMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundOutflowMin { get; set; }
        [NotMapped]
        public decimal? WaterBalancePrecipitationMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceEvaporationMin { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaMin { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaMin { get; set; }
    }

    public class LakeIndexPageViewModel
    {
        public IEnumerable<Lake> Items { get; set; }
        public Pager Pager { get; set; }
    }

    // not in DB (bufer)
    public class LakeB
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
