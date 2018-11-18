using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class GeneralHydrochemicalIndicatorStandard
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Indicator")]
        public string Indicator { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Value")]
        public decimal? Value { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LowerLimit")]
        public decimal? LowerLimit { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "UpperLimit")]
        public decimal? UpperLimit { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasurementUnit")]
        public int MeasurementUnitId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MeasurementUnit")]
        public MeasurementUnit MeasurementUnit { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "RegulatoryDocument")]
        public int RegulatoryDocumentId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "RegulatoryDocument")]
        public RegulatoryDocument RegulatoryDocument { get; set; }
    }

    public class GeneralHydrochemicalIndicatorStandardIndexPageViewModel
    {
        public IEnumerable<GeneralHydrochemicalIndicatorStandard> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
