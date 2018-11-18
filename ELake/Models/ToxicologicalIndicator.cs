using ELake.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class ToxicologicalIndicator
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        [Range(1900, 2015)]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakePart")]
        public LakePart LakePart { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NH4")]
        public decimal? NH4 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NO2")]
        public decimal? NO2 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NO3")]
        public decimal? NO3 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "PPO4")]
        public decimal? PPO4 { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Cu")]
        public decimal? Cu { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Zn")]
        public decimal? Zn { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Mn")]
        public decimal? Mn { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Pb")]
        public decimal? Pb { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Ni")]
        public decimal? Ni { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Cd")]
        public decimal? Cd { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Co")]
        public decimal? Co { get; set; }

        private NutrientsHeavyMetalsStandard NutrientsHeavyMetalsStandard
        {
            get
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseNpgsql("Host=localhost;Database=ELake;Username=postgres;Password=postgres");
                NutrientsHeavyMetalsStandard nutrientsHeavyMetalsStandard = null;
                using (var _context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    nutrientsHeavyMetalsStandard = _context.NutrientsHeavyMetalsStandard.FirstOrDefault();
                }
                return nutrientsHeavyMetalsStandard;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVNH4")]
        public decimal? IZVNH4
        {
            get
            {
                return NH4 / (NutrientsHeavyMetalsStandard?.MPCNH4);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVNO2")]
        public decimal? IZVNO2
        {
            get
            {
                return NO2 / (NutrientsHeavyMetalsStandard?.MPCNO2);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVNO3")]
        public decimal? IZVNO3
        {
            get
            {
                return NO3 / (NutrientsHeavyMetalsStandard?.MPCNO3);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVPPO4")]
        public decimal? IZVPPO4
        {
            get
            {
                return PPO4 / (NutrientsHeavyMetalsStandard?.MPCPPO4);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVCu")]
        public decimal? IZVCu
        {
            get
            {
                return Cu / (NutrientsHeavyMetalsStandard?.MPCCu);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVZn")]
        public decimal? IZVZn
        {
            get
            {
                return Zn / (NutrientsHeavyMetalsStandard?.MPCZn);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVMn")]
        public decimal? IZVMn
        {
            get
            {
                return Mn / (NutrientsHeavyMetalsStandard?.MPCMn);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVPb")]
        public decimal? IZVPb
        {
            get
            {
                return Pb / (NutrientsHeavyMetalsStandard?.MPCPb);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVNi")]
        public decimal? IZVNi
        {
            get
            {
                return Ni / (NutrientsHeavyMetalsStandard?.MPCNi);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVCd")]
        public decimal? IZVCd
        {
            get
            {
                return Cd / (NutrientsHeavyMetalsStandard?.MPCCd);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IZVCo")]
        public decimal? IZVCo
        {
            get
            {
                return Co / (NutrientsHeavyMetalsStandard?.MPCCo);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoNH4")]
        public decimal? KIZVkoNH4
        {
            get
            {
                if ((NH4 ?? 0) > 1)
                {
                    return NH4 * NutrientsHeavyMetalsStandard?.HazardClassNH4;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoNO2")]
        public decimal? KIZVkoNO2
        {
            get
            {
                if ((NO2 ?? 0) > 1)
                {
                    return NO2 * NutrientsHeavyMetalsStandard?.HazardClassNO2;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoNO3")]
        public decimal? KIZVkoNO3
        {
            get
            {
                if ((NO3 ?? 0) > 1)
                {
                    return NO3 * NutrientsHeavyMetalsStandard?.HazardClassNO3;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoPPO4")]
        public decimal? KIZVkoPPO4
        {
            get
            {
                if ((PPO4 ?? 0) > 1)
                {
                    return PPO4 * NutrientsHeavyMetalsStandard?.HazardClassPPO4;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoCu")]
        public decimal? KIZVkoCu
        {
            get
            {
                if ((Cu ?? 0) > 1)
                {
                    return Cu * NutrientsHeavyMetalsStandard?.HazardClassCu;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoZn")]
        public decimal? KIZVkoZn
        {
            get
            {
                if ((Zn ?? 0) > 1)
                {
                    return Zn * NutrientsHeavyMetalsStandard?.HazardClassZn;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoMn")]
        public decimal? KIZVkoMn
        {
            get
            {
                if ((Mn ?? 0) > 1)
                {
                    return Mn * NutrientsHeavyMetalsStandard?.HazardClassMn;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoPb")]
        public decimal? KIZVkoPb
        {
            get
            {
                if ((Pb ?? 0) > 1)
                {
                    return Pb * NutrientsHeavyMetalsStandard?.HazardClassPb;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoNi")]
        public decimal? KIZVkoNi
        {
            get
            {
                if ((Ni ?? 0) > 1)
                {
                    return Ni * NutrientsHeavyMetalsStandard?.HazardClassNi;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoCd")]
        public decimal? KIZVkoCd
        {
            get
            {
                if ((Cd ?? 0) > 1)
                {
                    return Cd * NutrientsHeavyMetalsStandard?.HazardClassCd;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVkoCo")]
        public decimal? KIZVkoCo
        {
            get
            {
                if ((Co ?? 0) > 1)
                {
                    return Co * NutrientsHeavyMetalsStandard?.HazardClassCo;
                }
                return null;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KIZVs")]
        public string KIZVs
        {
            get
            {
                decimal KIZV = (KIZVkoNH4 ?? 0) +
                    (KIZVkoNO2 ?? 0) +
                    (KIZVkoNO3 ?? 0) +
                    (KIZVkoPPO4 ?? 0) +
                    (KIZVkoCu ?? 0) +
                    (KIZVkoZn ?? 0) +
                    (KIZVkoMn ?? 0) +
                    (KIZVkoPb ?? 0) +
                    (KIZVkoNi ?? 0) +
                    (KIZVkoCd ?? 0) +
                    (KIZVkoCo ?? 0);
                if(KIZV<=2)
                {
                    return Resources.Controllers.SharedResources.Regulatory;
                }
                else if (KIZV < 6)
                {
                    return Resources.Controllers.SharedResources.ModeratePollution;
                }
                else if (KIZV < 10)
                {
                    return Resources.Controllers.SharedResources.HighPollution;
                }
                else
                {
                    return Resources.Controllers.SharedResources.ExtremelyHighLevelsPollution;
                }
            }
        }
    }

    public class ToxicologicalIndicatorIndexPageViewModel
    {
        public IEnumerable<ToxicologicalIndicator> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
