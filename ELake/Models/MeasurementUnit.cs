using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class MeasurementUnit
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Name")]
        public string Name { get; set; }
    }

    public class MeasurementUnitIndexPageViewModel
    {
        public IEnumerable<MeasurementUnit> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
