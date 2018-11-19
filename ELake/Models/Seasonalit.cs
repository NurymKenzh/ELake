using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class Seasonalit
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NoData")]
        public decimal NoData { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "January")]
        public decimal January { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "February")]
        public decimal February { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "March")]
        public decimal March { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "April")]
        public decimal April { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "May")]
        public decimal May { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "June")]
        public decimal June { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "July")]
        public decimal July { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "August")]
        public decimal August { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "September")]
        public decimal September { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "October")]
        public decimal October { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "November")]
        public decimal November { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "December")]
        public decimal December { get; set; }
    }

    public class SeasonalitIndexPageViewModel
    {
        public IEnumerable<Seasonalit> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
