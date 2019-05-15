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
        public decimal? WaterBalanceWaterBalanceReceiptAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceExpenditureAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceDiscrepancyAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceFlowPerAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundFlowPerAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalancePrecipitationPerAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceOutflowPerAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundOutflowPerAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceEvaporationPerAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceReceiptPerAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceExpenditurePerAvg { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceReceiptMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceExpenditureMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceDiscrepancyMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceFlowPerMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundFlowPerMax { get; set; }
        [NotMapped]
        public decimal? WaterBalancePrecipitationPerMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceOutflowPerMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundOutflowPerMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceEvaporationPerMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceReceiptPerMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceExpenditurePerMax { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceReceiptMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceExpenditureMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceDiscrepancyMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceFlowPerMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundFlowPerMin { get; set; }
        [NotMapped]
        public decimal? WaterBalancePrecipitationPerMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceSurfaceOutflowPerMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceUndergroundOutflowPerMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceEvaporationPerMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceReceiptPerMin { get; set; }
        [NotMapped]
        public decimal? WaterBalanceWaterBalanceExpenditurePerMin { get; set; }


        [NotMapped]
        public decimal? WaterLevelWaterLavelMAvg { get; set; }
        [NotMapped]
        public decimal? WaterLevelWaterLavelMMax { get; set; }
        [NotMapped]
        public decimal? WaterLevelWaterLavelMMin { get; set; }



        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataWaterLevelAvg { get; set; }
        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataLakeAreaAvg { get; set; }
        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataWaterMassVolumeAvg { get; set; }
        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataWaterLevelMax { get; set; }
        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataLakeAreaMax { get; set; }
        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataWaterMassVolumeMax { get; set; }
        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataWaterLevelMin { get; set; }
        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataLakeAreaMin { get; set; }
        [NotMapped]
        public decimal? BathigraphicAndVolumetricCurveDataWaterMassVolumeMin { get; set; }


        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorMineralizationAvg { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorTotalHardnessAvg { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorDissOxygWaterAvg { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorPercentOxygWaterAvg { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorpHAvg { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorOrganicSubstancesAvg { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorMineralizationMax { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorTotalHardnessMax { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorDissOxygWaterMax { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorPercentOxygWaterMax { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorpHMax { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorOrganicSubstancesMax { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorMineralizationMin { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorTotalHardnessMin { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorDissOxygWaterMin { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorPercentOxygWaterMin { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorpHMin { get; set; }
        [NotMapped]
        public decimal? GeneralHydrochemicalIndicatorOrganicSubstancesMin { get; set; }


        [NotMapped]
        public decimal? ToxicologicalIndicatorNH4Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNO2Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNO3Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorPPO4Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCuAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorZnAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorMnAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorPbAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNiAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCdAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCoAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNH4Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNO2Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNO3Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVPPO4Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCuAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVZnAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVMnAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVPbAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNiAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCdAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCoAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNH4Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNO2Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNO3Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoPPO4Avg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCuAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoZnAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoMnAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoPbAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNiAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCdAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCoAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVAvg { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNH4Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNO2Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNO3Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorPPO4Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCuMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorZnMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorMnMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorPbMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNiMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCdMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCoMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNH4Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNO2Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNO3Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVPPO4Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCuMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVZnMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVMnMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVPbMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNiMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCdMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCoMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNH4Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNO2Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNO3Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoPPO4Max { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCuMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoZnMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoMnMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoPbMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNiMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCdMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCoMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVMax { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNH4Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNO2Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNO3Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorPPO4Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCuMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorZnMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorMnMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorPbMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorNiMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCdMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorCoMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNH4Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNO2Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNO3Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVPPO4Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCuMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVZnMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVMnMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVPbMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVNiMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCdMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorIZVCoMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNH4Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNO2Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNO3Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoPPO4Min { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCuMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoZnMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoMnMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoPbMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoNiMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCdMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVkoCoMin { get; set; }
        [NotMapped]
        public decimal? ToxicologicalIndicatorKIZVMin { get; set; }


        [NotMapped]
        public decimal? IonsaltWaterCompositionCaMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCaMgAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgMgAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKMgAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClMgAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOMgAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOMgAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCationsSumMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionAnionsSumMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionIonsSumMgEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCaPerEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgPerEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKPerEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClPerEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOPerEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOPerEqAvg { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCaMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCaMgMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgMgMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKMgMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClMgMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOMgMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOMgMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCationsSumMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionAnionsSumMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionIonsSumMgEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCaPerEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgPerEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKPerEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClPerEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOPerEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOPerEqMax { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCaMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCaMgMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgMgMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKMgMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClMgMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOMgMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOMgMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCationsSumMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionAnionsSumMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionIonsSumMgEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionCaPerEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionMgPerEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionNaKPerEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionClPerEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionHCOPerEqMin { get; set; }
        [NotMapped]
        public decimal? IonsaltWaterCompositionSOPerEqMin { get; set; }


        [NotMapped]
        public decimal? TransitionNoСhange { get; set; }
        [NotMapped]
        public decimal? TransitionPermanent { get; set; }
        [NotMapped]
        public decimal? TransitionNewPermanent { get; set; }
        [NotMapped]
        public decimal? TransitionLostPermanent { get; set; }
        [NotMapped]
        public decimal? TransitionSeasonal { get; set; }
        [NotMapped]
        public decimal? TransitionNewSeasonal { get; set; }
        [NotMapped]
        public decimal? TransitionLostSeasonal { get; set; }
        [NotMapped]
        public decimal? TransitionSeasonalToPermanent { get; set; }
        [NotMapped]
        public decimal? TransitionPermanentToDeasonal { get; set; }
        [NotMapped]
        public decimal? TransitionEphemeralPermanent { get; set; }
        [NotMapped]
        public decimal? TransitionEphemeralSeasonal { get; set; }
        [NotMapped]
        public decimal? TransitionMaximumWater { get; set; }
        [NotMapped]
        public decimal? TransitionPermanentPer { get; set; }
        [NotMapped]
        public decimal? TransitionSeasonalPer { get; set; }


        [NotMapped]
        public decimal? SeasonalitNoData { get; set; }
        [NotMapped]
        public decimal? SeasonalitJanuary { get; set; }
        [NotMapped]
        public decimal? SeasonalitFebruary { get; set; }
        [NotMapped]
        public decimal? SeasonalitMarch { get; set; }
        [NotMapped]
        public decimal? SeasonalitApril { get; set; }
        [NotMapped]
        public decimal? SeasonalitMay { get; set; }
        [NotMapped]
        public decimal? SeasonalitJune { get; set; }
        [NotMapped]
        public decimal? SeasonalitJuly { get; set; }
        [NotMapped]
        public decimal? SeasonalitAugust { get; set; }
        [NotMapped]
        public decimal? SeasonalitSeptember { get; set; }
        [NotMapped]
        public decimal? SeasonalitOctober { get; set; }
        [NotMapped]
        public decimal? SeasonalitNovember { get; set; }
        [NotMapped]
        public decimal? SeasonalitDecember { get; set; }


        [NotMapped]
        public decimal? DynamicsLakeAreaNoDataPersAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaNotWaterAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaMaximumWaterAreaAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaPerAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaPerAvg { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaNoDataPersMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaNotWaterMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaMaximumWaterAreaMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaPerMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaPerMax { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaNoDataPersMin { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaNotWaterMin { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaMin { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaMin { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaMaximumWaterAreaMin { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaSeasonalWaterAreaPerMin { get; set; }
        [NotMapped]
        public decimal? DynamicsLakeAreaPermanentWaterAreaPerMin { get; set; }
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
