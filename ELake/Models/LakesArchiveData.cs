using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class LakesArchiveData
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SurveyYear")]
        public int? SurveyYear { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeLength")]
        public decimal? LakeLength { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeShorelineLength")]
        public decimal? LakeShorelineLength { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeMirrorArea")]
        public decimal? LakeMirrorArea { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeAbsoluteHeight")]
        public decimal? LakeAbsoluteHeight { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeWidth")]
        public decimal? LakeWidth { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeMaxDepth")]
        public decimal? LakeMaxDepth { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeWaterMass")]
        public decimal? LakeWaterMass { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ArchivalInfoSource")]
        public string ArchivalInfoSource { get; set; }
    }

    public class LakesArchiveDataIndexPageViewModel
    {
        public IEnumerable<LakesArchiveData> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
