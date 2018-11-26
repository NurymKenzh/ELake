using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELake.Models
{
    public class IonsaltWaterComposition
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakeId")]
        public int LakeId { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Year")]
        [Range(1900, 2015)]
        public int Year { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "LakePart")]
        public LakePart LakePart { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "CaMgEq")]
        public decimal? CaMgEq { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MgMgEq")]
        public decimal? MgMgEq { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NaKMgEq")]
        public decimal? NaKMgEq { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ClMgEq")]
        public decimal? ClMgEq { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HCOMgEq")]
        public decimal? HCOMgEq { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SOMgEq")]
        public decimal? SOMgEq { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "CaMg")]
        public decimal? CaMg
        {
            get
            {
                return CaMgEq * 20.04M;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MgMg")]
        public decimal? MgMg
        {
            get
            {
                return MgMgEq * 12.16M;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NaKMg")]
        public decimal? NaKMg
        {
            get
            {
                return NaKMgEq * 23;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ClMg")]
        public decimal? ClMg
        {
            get
            {
                return ClMgEq * 35.45M;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HCOMg")]
        public decimal? HCOMg
        {
            get
            {
                return HCOMgEq * 61.02M;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SOMg")]
        public decimal? SOMg
        {
            get
            {
                return SOMgEq * 48.03M;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "CationsSumMgEq")]
        public decimal CationsSumMgEq
        {
            get
            {
                return (CaMgEq ?? 0) + (MgMgEq ?? 0) + (NaKMgEq ?? 0);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AnionsSumMgEq")]
        public decimal AnionsSumMgEq
        {
            get
            {
                return (ClMgEq ?? 0) + (HCOMgEq ?? 0) + (SOMgEq ?? 0);
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IonsSumMgEq")]
        public decimal IonsSumMgEq
        {
            get
            {
                return CationsSumMgEq + AnionsSumMgEq;
            }
        }

        //[Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "IonsSumPer")]
        //public decimal IonsSumPer
        //{
        //    get
        //    {
        //        return 100;
        //    }
        //}

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "CaPerEq")]
        public decimal? CaPerEq
        {
            get
            {
                return CaMgEq / IonsSumMgEq * 100;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "MgPerEq")]
        public decimal? MgPerEq
        {
            get
            {
                return MgMgEq / IonsSumMgEq * 100;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NaKPerEq")]
        public decimal? NaKPerEq
        {
            get
            {
                return NaKMgEq / IonsSumMgEq * 100;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "ClPerEq")]
        public decimal? ClPerEq
        {
            get
            {
                return ClMgEq / IonsSumMgEq * 100;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "HCOPerEq")]
        public decimal? HCOPerEq
        {
            get
            {
                return HCOMgEq / IonsSumMgEq * 100;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "SOPerEq")]
        public decimal? SOPerEq
        {
            get
            {
                return SOMgEq / IonsSumMgEq * 100;
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterClass")]
        public string WaterClass
        {
            get
            {
                decimal max1 = Math.Max(ClPerEq ?? 0, Math.Max(SOPerEq ?? 0, HCOPerEq ?? 0)),
                    max2 = Math.Min(ClPerEq ?? 0, Math.Max(SOPerEq ?? 0, HCOPerEq ?? 0)),
                    max3 = Math.Min(ClPerEq ?? 0, Math.Min(SOPerEq ?? 0, HCOPerEq ?? 0));
                string wclass1 = "",
                    wclass2 = "",
                    wclass3 = "";
                if (max1 == (HCOPerEq ?? 0))
                {
                    wclass1 = Resources.Controllers.SharedResources.HydrocarbonateCarbonateClass;
                }
                else if (max1 == (SOPerEq ?? 0))
                {
                    wclass1 = Resources.Controllers.SharedResources.SulphateClass;
                }
                else if (max1 == (ClPerEq ?? 0))
                {
                    wclass1 = Resources.Controllers.SharedResources.ChlorideClass;
                }
                if (max2 == (HCOPerEq ?? 0))
                {
                    wclass2 = Resources.Controllers.SharedResources.HydrocarbonateCarbonateClass;
                }
                else if (max2 == (SOPerEq ?? 0))
                {
                    wclass2 = Resources.Controllers.SharedResources.SulphateClass;
                }
                else if (max2 == (ClPerEq ?? 0))
                {
                    wclass2 = Resources.Controllers.SharedResources.ChlorideClass;
                }
                if (max3 == (HCOPerEq ?? 0))
                {
                    wclass3 = Resources.Controllers.SharedResources.HydrocarbonateCarbonateClass;
                }
                else if (max3 == (SOPerEq ?? 0))
                {
                    wclass3 = Resources.Controllers.SharedResources.SulphateClass;
                }
                else if (max3 == (ClPerEq ?? 0))
                {
                    wclass3 = Resources.Controllers.SharedResources.ChlorideClass;
                }

                if (max1 - max2 > 5)
                {
                    return wclass1;
                }
                else if (max2 - max3 > 5)
                {
                    return wclass1 + wclass2;
                }
                else
                {
                    return wclass1 + wclass2 + wclass3;
                }
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Group")]
        public string Group
        {
            get
            {
                decimal max1 = Math.Max(CaPerEq ?? 0, Math.Max(MgPerEq ?? 0, NaKPerEq ?? 0)),
                    max2 = Math.Min(CaPerEq ?? 0, Math.Max(MgPerEq ?? 0, NaKPerEq ?? 0)),
                    max3 = Math.Min(CaPerEq ?? 0, Math.Min(MgPerEq ?? 0, NaKPerEq ?? 0));
                string wgroup1 = "",
                    wgroup2 = "",
                    wgroup3 = "";
                if (max1 == (CaPerEq ?? 0))
                {
                    wgroup1 = Resources.Controllers.SharedResources.CalciumGroup;
                }
                else if (max1 == (MgPerEq ?? 0))
                {
                    wgroup1 = Resources.Controllers.SharedResources.MagnesiumGroup;
                }
                else if (max1 == (NaKPerEq ?? 0))
                {
                    wgroup1 = Resources.Controllers.SharedResources.SodiumGroup;
                }
                if (max2 == (CaPerEq ?? 0))
                {
                    wgroup2 = Resources.Controllers.SharedResources.CalciumGroup;
                }
                else if (max2 == (MgPerEq ?? 0))
                {
                    wgroup2 = Resources.Controllers.SharedResources.MagnesiumGroup;
                }
                else if (max2 == (NaKPerEq ?? 0))
                {
                    wgroup2 = Resources.Controllers.SharedResources.SodiumGroup;
                }
                if (max3 == (CaPerEq ?? 0))
                {
                    wgroup3 = Resources.Controllers.SharedResources.CalciumGroup;
                }
                else if (max3 == (MgPerEq ?? 0))
                {
                    wgroup3 = Resources.Controllers.SharedResources.MagnesiumGroup;
                }
                else if (max3 == (NaKPerEq ?? 0))
                {
                    wgroup3 = Resources.Controllers.SharedResources.SodiumGroup;
                }

                if (max1 - max2 > 5)
                {
                    return "(" + wgroup1 + ")";
                }
                else if (max2 - max3 > 5)
                {
                    return "(" + wgroup1 + wgroup2 + ")";
                }
                else
                {
                    return "(" + wgroup1 + wgroup2 + wgroup3 + ")";
                }
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "WaterType")]
        public string WaterType
        {
            get
            {
                if ((HCOMgEq ?? 0) > (CaMgEq ?? 0) + (MgMgEq ?? 0))
                {
                    return Resources.Controllers.SharedResources.Type1;
                }
                else if ((HCOMgEq ?? 0) < (CaMgEq ?? 0) + (MgMgEq ?? 0) && (CaMgEq ?? 0) + (MgMgEq ?? 0) < (HCOMgEq ?? 0) + (SOMgEq ?? 0))
                {
                    return Resources.Controllers.SharedResources.Type2;
                }
                else if ((HCOMgEq ?? 0) + (SOMgEq ?? 0) < (CaMgEq ?? 0) + (MgMgEq ?? 0) || (ClMgEq ?? 0) > (NaKMgEq ?? 0))
                {
                    return Resources.Controllers.SharedResources.Type3;
                }
                else if ((HCOMgEq ?? 0) == 0)
                {
                    return Resources.Controllers.SharedResources.Type4;
                }
                else
                {
                    return null;
                }
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "AlekinWaterCompositionClass")]
        public string AlekinWaterCompositionClass
        {
            get
            {
                return $"{WaterClass}{Group}{WaterType}";
            }
        }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "KurlovFormula")]
        public string KurlovFormula
        {
            get
            {
                decimal m = ((CaMg ?? 0) + (MgMg ?? 0) + (NaKMg ?? 0) + (ClMg ?? 0) + (HCOMg ?? 0) + (SOMg ?? 0)) / 1000;
                string anions = "",
                    Clf = "",
                    HCOf = "",
                    SOf = "",
                    cations = "",
                    Caf = "",
                    Mgf = "",
                    NaKf = "";
                if (ClPerEq > 5)
                {
                    Clf = $"Cl{String.Format("{0:0}", ClPerEq)}";
                }
                if (HCOPerEq > 5)
                {
                    HCOf = $"HCO₃{String.Format("{0:0}", HCOPerEq)}";
                }
                if (SOPerEq > 5)
                {
                    SOf = $"SO₄{String.Format("{0:0}", SOPerEq)}";
                }
                if (CaPerEq > 5)
                {
                    Caf = $"Ca{String.Format("{0:0}", CaPerEq)}";
                }
                if (MgPerEq > 5)
                {
                    Mgf = $"MG{String.Format("{0:0}", MgPerEq)}";
                }
                if (NaKPerEq > 5)
                {
                    NaKf = $"Na+K{String.Format("{0:0}", NaKPerEq)}";
                }
                // 1 Cl
                if ((ClPerEq ?? 0) >= (HCOPerEq ?? 0) && (ClPerEq ?? 0) >= (SOPerEq ?? 0))
                {
                    anions = string.IsNullOrEmpty(Clf) ? "" : Clf;
                    // 2 HCO
                    if ((HCOPerEq ?? 0) >= (SOPerEq ?? 0))
                    {
                        anions += string.IsNullOrEmpty(HCOf) ? "" : HCOf;
                    }
                    // 2 SO
                    else
                    {
                        anions += string.IsNullOrEmpty(SOf) ? "" : SOf;
                    }
                }
                // 1 HCO
                if ((HCOPerEq ?? 0) >= (ClPerEq ?? 0) && (HCOPerEq ?? 0) >= (SOPerEq ?? 0))
                {
                    anions = string.IsNullOrEmpty(HCOf) ? "" : HCOf;
                    // 2 Cl
                    if ((ClPerEq ?? 0) >= (SOPerEq ?? 0))
                    {
                        anions += string.IsNullOrEmpty(Clf) ? "" : Clf;
                    }
                    // 2 SO
                    else
                    {
                        anions += string.IsNullOrEmpty(SOf) ? "" : SOf;
                    }
                }
                // 1 SO
                if ((SOPerEq ?? 0) >= (ClPerEq ?? 0) && (SOPerEq ?? 0) >= (HCOPerEq ?? 0))
                {
                    anions = string.IsNullOrEmpty(SOf) ? "" : SOf;
                    // 2 Cl
                    if ((ClPerEq ?? 0) >= (HCOPerEq ?? 0))
                    {
                        anions += string.IsNullOrEmpty(Clf) ? "" : Clf;
                    }
                    // 2 HCO
                    else
                    {
                        anions += string.IsNullOrEmpty(HCOf) ? "" : HCOf;
                    }
                }
                // 1 Ca
                if ((CaPerEq ?? 0) >= (MgPerEq ?? 0) && (CaPerEq ?? 0) >= (NaKPerEq ?? 0))
                {
                    cations = string.IsNullOrEmpty(Caf) ? "" : Caf;
                    // 2 Mg
                    if ((MgPerEq ?? 0) >= (NaKPerEq ?? 0))
                    {
                        cations += string.IsNullOrEmpty(Mgf) ? "" : Mgf;
                    }
                    // 2 NaK
                    else
                    {
                        cations += string.IsNullOrEmpty(NaKf) ? "" : NaKf;
                    }
                }
                // 1 Mg
                if ((MgPerEq ?? 0) >= (CaPerEq ?? 0) && (MgPerEq ?? 0) >= (NaKPerEq ?? 0))
                {
                    cations = string.IsNullOrEmpty(Mgf) ? "" : Mgf;
                    // 2 Ca
                    if ((CaPerEq ?? 0) >= (NaKPerEq ?? 0))
                    {
                        cations += string.IsNullOrEmpty(Caf) ? "" : Caf;
                    }
                    // 2 NaK
                    else
                    {
                        cations += string.IsNullOrEmpty(NaKf) ? "" : NaKf;
                    }
                }
                // 1 NaK
                if ((NaKPerEq ?? 0) >= (CaPerEq ?? 0) && (NaKPerEq ?? 0) >= (MgPerEq ?? 0))
                {
                    cations = string.IsNullOrEmpty(NaKf) ? "" : NaKf;
                    // 2 Ca
                    if ((CaPerEq ?? 0) >= (MgPerEq ?? 0))
                    {
                        cations += string.IsNullOrEmpty(Caf) ? "" : Caf;
                    }
                    // 2 Mg
                    else
                    {
                        cations += string.IsNullOrEmpty(Mgf) ? "" : Mgf;
                    }
                }

                return $"M{String.Format("{0:0.##}", m)}({anions})/({cations})";
            }
        }
    }

    public class IonsaltWaterCompositionIndexPageViewModel
    {
        public IEnumerable<IonsaltWaterComposition> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
