using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
