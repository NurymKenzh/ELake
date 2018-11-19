using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class FishKind
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FishId")]
        public int FishId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FamilyId")]
        public int FamilyId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FishNameKK")]
        public string FishNameKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FishNameRU")]
        public string FishNameRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FishNameLA")]
        public string FishNameLA { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FamilyNameKK")]
        public string FamilyNameKK { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FamilyNameRU")]
        public string FamilyNameRU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "FamilyNameLA")]
        public string FamilyNameLA { get; set; }
    }

    public class FishKindIndexPageViewModel
    {
        public IEnumerable<FishKind> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
