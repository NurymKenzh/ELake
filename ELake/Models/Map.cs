using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class Map
    {
        public int Id { get; set; }

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

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Layers")]
        public int[] LayersId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Layers")]
        public Layer[] Layers { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IncludeWater")]
        public bool IncludeWater { get; set; }

        public Map()
        {
            LayersId = new Int32[0];
        }
    }
}
