using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class Layer
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GeoServerName")]
        public string GeoServerName { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FileNameWithPath")]
        public string FileNameWithPath { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "GeoServerStyle")]
        public string GeoServerStyle { get; set; }

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
                    name = NameRU;
                if (language == "kk")
                {
                    name = NameKK;
                }
                if (language == "ru")
                {
                    name = NameRU;
                }
                if (language == "en")
                {
                    name = NameEN;
                }
                return name;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Lake")]
        public bool Lake { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsWaterLevel { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesWaterLevel { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsWaterLevel { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsSurfaceFlow { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesSurfaceFlow { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsSurfaceFlow { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsPrecipitation { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesPrecipitation { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsPrecipitation { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsUndergroundFlow { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesUndergroundFlow { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsUndergroundFlow { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsSurfaceOutflow { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesSurfaceOutflow { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsSurfaceOutflow { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsEvaporation { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesEvaporation { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsEvaporation { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsUndergroundOutflow { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesUndergroundOutflow { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsUndergroundOutflow { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryMineralization { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryMineralization { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryMineralization { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryTotalHardness { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryTotalHardness { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryTotalHardness { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryDissOxygWater { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryDissOxygWater { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryDissOxygWater { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryPercentOxygWater { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryPercentOxygWater { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryPercentOxygWater { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistrypH { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistrypH { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistrypH { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryOrganicSubstances { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryOrganicSubstances { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryOrganicSubstances { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryCa { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryCa { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryCa { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryMg { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryMg { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryMg { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryNaK { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryNaK { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryNaK { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryCl { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryCl { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryCl { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryHCO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryHCO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryHCO { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistrySO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistrySO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistrySO { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryNH { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryNH { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryNH { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryNO2 { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryNO2 { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryNO2 { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryNO3 { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryNO3 { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryNO3 { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryPPO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryPPO { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryPPO { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryCu { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryCu { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryCu { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryZn { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryZn { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryZn { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryMn { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryMn { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryMn { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryPb { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryPb { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryPb { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryNi { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryNi { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryNi { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryCd { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryCd { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryCd { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryCo { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryCo { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryCo { get; set; }

        [NotMapped]
        public List<LayerInterval> LayerIntervalsHydrochemistryCIWP { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MinValues")]
        public decimal[] MinValuesHydrochemistryCIWP { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Colors")]
        public string[] ColorsHydrochemistryCIWP { get; set; }
    }
}
