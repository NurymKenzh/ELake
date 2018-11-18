using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class GeneralHydrochemicalIndicator
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        [Range(1900, 2015)]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakePart")]
        public LakePart LakePart { get; set; }

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

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MineralizationType")]
        public string MineralizationType
        {
            get
            {
                if(Mineralization!=null)
                {
                    if(Mineralization<1000)
                    {
                        return Resources.Controllers.SharedResources.Fresh;
                    }
                    else if (Mineralization < 25000)
                    {
                        return Resources.Controllers.SharedResources.Brackish;
                    }
                    else if (Mineralization < 50000)
                    {
                        return Resources.Controllers.SharedResources.MarineSalinity;
                    }
                    else
                    {
                        return Resources.Controllers.SharedResources.Brine;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterHardness")]
        public string WaterHardness
        {
            get
            {
                if(TotalHardness != null)
                {
                    if(TotalHardness < 1.5M)
                    {
                        return Resources.Controllers.SharedResources.VerySoft;
                    }
                    else if (TotalHardness < 3)
                    {
                        return Resources.Controllers.SharedResources.Soft;
                    }
                    else if (TotalHardness < 6)
                    {
                        return Resources.Controllers.SharedResources.ModeratelyTough;
                    }
                    else if (TotalHardness < 9)
                    {
                        return Resources.Controllers.SharedResources.Tough;
                    }
                    else
                    {
                        return Resources.Controllers.SharedResources.VeryTough;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DissolvedOxygenInWaterDegree")]
        public string DissolvedOxygenInWaterDegree
        {
            get
            {
                if(DissOxygWater != null)
                {
                    if(DissOxygWater < 1)
                    {
                        return Resources.Controllers.SharedResources.ExtremelyHighLevelsPollution;
                    }
                    else if (DissOxygWater < 3)
                    {
                        return Resources.Controllers.SharedResources.HighPollution;
                    }
                    else if (DissOxygWater < 4)
                    {
                        return Resources.Controllers.SharedResources.ModeratePollution;
                    }
                    else
                    {
                        return Resources.Controllers.SharedResources.Regulatory;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AcidityOfWaterDegree")]
        public string AcidityOfWaterDegree
        {
            get
            {
                if(pH != null)
                {
                    if(pH < 3)
                    {
                        return Resources.Controllers.SharedResources.StronglyAcidicWater;
                    }
                    else if (pH < 5)
                    {
                        return Resources.Controllers.SharedResources.SourWater;
                    }
                    else if (pH < 6.5M)
                    {
                        return Resources.Controllers.SharedResources.SubacidWaters;
                    }
                    else if (pH < 7.5M)
                    {
                        return Resources.Controllers.SharedResources.NeutralWaters;
                    }
                    else if (pH < 8.5M)
                    {
                        return Resources.Controllers.SharedResources.WeakAlkalineWater;
                    }
                    else if (pH < 9.5M)
                    {
                        return Resources.Controllers.SharedResources.AlkalineWater;
                    }
                    else
                    {
                        return Resources.Controllers.SharedResources.StronglyAlkalineWater;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public class GeneralHydrochemicalIndicatorIndexPageViewModel
    {
        public IEnumerable<GeneralHydrochemicalIndicator> Items { get; set; }
        public Pager Pager { get; set; }
    }

    public enum LakePart
    {
        FullyPart = 0,
        FreshPart = 1,
        SaltyPart = 2
    }
}
