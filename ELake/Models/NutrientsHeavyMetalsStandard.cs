using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class NutrientsHeavyMetalsStandard
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "RegulatoryDocument")]
        public int RegulatoryDocumentId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "RegulatoryDocument")]
        public RegulatoryDocument RegulatoryDocument { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCNH4")]
        public decimal MPCNH4 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCNO2")]
        public decimal MPCNO2 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCNO3")]
        public decimal MPCNO3 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCPPO4")]
        public decimal MPCPPO4 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCCu")]
        public decimal MPCCu { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCZn")]
        public decimal MPCZn { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCMn")]
        public decimal MPCMn { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCPb")]
        public decimal MPCPb { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCNi")]
        public decimal MPCNi { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCCd")]
        public decimal MPCCd { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MPCCo")]
        public decimal MPCCo { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassNH4")]
        public int HazardClassNH4 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassNO2")]
        public int HazardClassNO2 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassNO3")]
        public int HazardClassNO3 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassPPO4")]
        public int HazardClassPPO4 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassCu")]
        public int HazardClassCu { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassZn")]
        public int HazardClassZn { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassMn")]
        public int HazardClassMn { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassPb")]
        public int HazardClassPb { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassNi")]
        public int HazardClassNi { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassCd")]
        public int HazardClassCd { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HazardClassCo")]
        public int HazardClassCo { get; set; }
    }

    public class NutrientsHeavyMetalsStandardIndexPageViewModel
    {
        public IEnumerable<NutrientsHeavyMetalsStandard> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
