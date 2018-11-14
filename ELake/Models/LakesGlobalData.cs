using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class LakesGlobalData
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Hylak_id")]
        public int Hylak_id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Lake_name_ENG")]
        public string Lake_name_ENG { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Lake_name_RU")]
        public string Lake_name_RU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Lake_name_KZ")]
        public string Lake_name_KZ { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Lake_name")]
        public string Lake_name
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    Lake_name = Lake_name_ENG;
                if (language == "kk")
                {
                    Lake_name = Lake_name_KZ;
                }
                if (language == "ru")
                {
                    Lake_name = Lake_name_RU;
                }
                return Lake_name;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Country_ENG")]
        public string Country_ENG { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Country_RU")]
        public string Country_RU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Country_KZ")]
        public string Country_KZ { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Country")]
        public string Country
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    Country = Country_ENG;
                if (language == "kk")
                {
                    Country = Country_KZ;
                }
                if (language == "ru")
                {
                    Country = Country_RU;
                }
                return Country;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Continent_ENG")]
        public string Continent_ENG { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Continent_RU")]
        public string Continent_RU { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Continent_KZ")]
        public string Continent_KZ { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Continent")]
        public string Continent
        {
            get
            {
                string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name,
                    Continent = Continent_ENG;
                if (language == "kk")
                {
                    Continent = Continent_KZ;
                }
                if (language == "ru")
                {
                    Continent = Continent_RU;
                }
                return Continent;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Lake_area")]
        public decimal? Lake_area { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Shore_len")]
        public decimal? Shore_len { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Shore_dev")]
        public decimal? Shore_dev { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Vol_total")]
        public decimal? Vol_total { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Depth_avg")]
        public decimal? Depth_avg { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Dis_avg")]
        public decimal? Dis_avg { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Res_time")]
        public decimal? Res_time { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Elevation")]
        public decimal? Elevation { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Slope_100")]
        public decimal? Slope_100 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Wshd_area")]
        public decimal? Wshd_area { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Pour_long")]
        public decimal? Pour_long { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Pour_lat")]
        public decimal? Pour_lat { get; set; }
    }
}
