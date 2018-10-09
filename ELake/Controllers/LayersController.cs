using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELake.Data;
using ELake.Models;
using Microsoft.Extensions.Localization;

namespace ELake.Controllers
{
    public class LayersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly GDALController _GDAL;

        public LayersController(ApplicationDbContext context,
            IStringLocalizer<SharedResources> sharedLocalizer,
            GDALController GDAL)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
            _GDAL = GDAL;
        }

        // GET: Layers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Layer.ToListAsync());
        }

        public static int Max(params int[] values)
        {
            return Enumerable.Max(values);
        }

        public static int Min(params int[] values)
        {
            return Enumerable.Min(values);
        }

        // GET: Layers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layer
                .SingleOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }

            ViewBag.MinYear = Min(_context.WaterLevel.Min(w => w.Year),
                _context.SurfaceFlow.Min(w => w.Year),
                _context.Precipitation.Min(w => w.Year),
                _context.UndergroundFlow.Min(w => w.Year),
                _context.SurfaceOutflow.Min(w => w.Year),
                _context.Evaporation.Min(w => w.Year),
                _context.UndergroundOutflow.Min(w => w.Year),
                _context.Hydrochemistry.Min(w => w.Year));
            ViewBag.MaxYear = Max(_context.WaterLevel.Max(w => w.Year),
                _context.SurfaceFlow.Max(w => w.Year),
                _context.Precipitation.Max(w => w.Year),
                _context.UndergroundFlow.Max(w => w.Year),
                _context.SurfaceOutflow.Max(w => w.Year),
                _context.Evaporation.Max(w => w.Year),
                _context.UndergroundOutflow.Max(w => w.Year),
                _context.Hydrochemistry.Max(w => w.Year));

            string[] DataTypes = new string[] {
                "WaterLevels",
                "SurfaceFlows",
                "Precipitations",
                "UndergroundFlows",
                "SurfaceOutflows",
                "Evaporations",
                "UndergroundOutflows",
                "HydrochemistryMineralizations",
                "HydrochemistryTotalHardnesss",
                "HydrochemistryDissOxygWaters",
                "HydrochemistryPercentOxygWaters",
                "HydrochemistrypHs",
                "HydrochemistryOrganicSubstancess",
                "HydrochemistryCas",
                "HydrochemistryMgs",
                "HydrochemistryNaKs",
                "HydrochemistryCls",
                "HydrochemistryHCOs",
                "HydrochemistrySOs",
                "HydrochemistryNHs",
                "HydrochemistryNO2s",
                "HydrochemistryNO3s",
                "HydrochemistryPPOs",
                "HydrochemistryCus",
                "HydrochemistryZns",
                "HydrochemistryMns",
                "HydrochemistryPbs",
                "HydrochemistryNis",
                "HydrochemistryCds",
                "HydrochemistryCos",
                "HydrochemistryCIWPs"
            };
            ViewBag.DataType = DataTypes.Select(r => new SelectListItem { Text = _sharedLocalizer[r], Value = r });

            return View(layer);
        }

        // GET: Layers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Layers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GeoServerName,FileNameWithPath,GeoServerStyle")] Layer layer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(layer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(layer);
        }

        // GET: Layers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layer.SingleOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }

            layer.LayerIntervalsWaterLevel = new List<LayerInterval>();
            for (int i = 0;i< layer.MinValuesWaterLevel.Count(); i++)
            {
                layer.LayerIntervalsWaterLevel.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesWaterLevel[i],
                    Color = layer.ColorsWaterLevel[i]
                });
            }

            layer.LayerIntervalsSurfaceFlow = new List<LayerInterval>();
            for (int i = 0;i< layer.MinValuesSurfaceFlow.Count(); i++)
            {
                layer.LayerIntervalsSurfaceFlow.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesSurfaceFlow[i],
                    Color = layer.ColorsSurfaceFlow[i]
                });
            }

            layer.LayerIntervalsPrecipitation = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesPrecipitation.Count(); i++)
            {
                layer.LayerIntervalsPrecipitation.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesPrecipitation[i],
                    Color = layer.ColorsPrecipitation[i]
                });
            }

            layer.LayerIntervalsUndergroundFlow = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesUndergroundFlow.Count(); i++)
            {
                layer.LayerIntervalsUndergroundFlow.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesUndergroundFlow[i],
                    Color = layer.ColorsUndergroundFlow[i]
                });
            }

            layer.LayerIntervalsSurfaceOutflow = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesSurfaceOutflow.Count(); i++)
            {
                layer.LayerIntervalsSurfaceOutflow.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesSurfaceOutflow[i],
                    Color = layer.ColorsSurfaceOutflow[i]
                });
            }

            layer.LayerIntervalsEvaporation = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesEvaporation.Count(); i++)
            {
                layer.LayerIntervalsEvaporation.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesEvaporation[i],
                    Color = layer.ColorsEvaporation[i]
                });
            }

            layer.LayerIntervalsUndergroundOutflow = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesUndergroundOutflow.Count(); i++)
            {
                layer.LayerIntervalsUndergroundOutflow.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesUndergroundOutflow[i],
                    Color = layer.ColorsUndergroundOutflow[i]
                });
            }

            layer.LayerIntervalsHydrochemistryMineralization = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryMineralization.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryMineralization.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryMineralization[i],
                    Color = layer.ColorsHydrochemistryMineralization[i]
                });
            }

            layer.LayerIntervalsHydrochemistryTotalHardness = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryTotalHardness.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryTotalHardness.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryTotalHardness[i],
                    Color = layer.ColorsHydrochemistryTotalHardness[i]
                });
            }

            layer.LayerIntervalsHydrochemistryDissOxygWater = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryDissOxygWater.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryDissOxygWater.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryDissOxygWater[i],
                    Color = layer.ColorsHydrochemistryDissOxygWater[i]
                });
            }

            layer.LayerIntervalsHydrochemistryPercentOxygWater = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryPercentOxygWater.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryPercentOxygWater.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryPercentOxygWater[i],
                    Color = layer.ColorsHydrochemistryPercentOxygWater[i]
                });
            }

            layer.LayerIntervalsHydrochemistrypH = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistrypH.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistrypH.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistrypH[i],
                    Color = layer.ColorsHydrochemistrypH[i]
                });
            }

            layer.LayerIntervalsHydrochemistryOrganicSubstances = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryOrganicSubstances.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryOrganicSubstances.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryOrganicSubstances[i],
                    Color = layer.ColorsHydrochemistryOrganicSubstances[i]
                });
            }

            layer.LayerIntervalsHydrochemistryCa = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCa.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCa.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCa[i],
                    Color = layer.ColorsHydrochemistryCa[i]
                });
            }

            layer.LayerIntervalsHydrochemistryMg = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryMg.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryMg.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryMg[i],
                    Color = layer.ColorsHydrochemistryMg[i]
                });
            }

            layer.LayerIntervalsHydrochemistryNaK = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNaK.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNaK.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNaK[i],
                    Color = layer.ColorsHydrochemistryNaK[i]
                });
            }

            layer.LayerIntervalsHydrochemistryCl = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCl.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCl.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCl[i],
                    Color = layer.ColorsHydrochemistryCl[i]
                });
            }

            layer.LayerIntervalsHydrochemistryHCO = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryHCO.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryHCO.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryHCO[i],
                    Color = layer.ColorsHydrochemistryHCO[i]
                });
            }

            layer.LayerIntervalsHydrochemistrySO = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistrySO.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistrySO.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistrySO[i],
                    Color = layer.ColorsHydrochemistrySO[i]
                });
            }

            layer.LayerIntervalsHydrochemistryNH = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNH.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNH.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNH[i],
                    Color = layer.ColorsHydrochemistryNH[i]
                });
            }

            layer.LayerIntervalsHydrochemistryNO2 = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNO2.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNO2.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNO2[i],
                    Color = layer.ColorsHydrochemistryNO2[i]
                });
            }

            layer.LayerIntervalsHydrochemistryNO3 = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNO3.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNO3.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNO3[i],
                    Color = layer.ColorsHydrochemistryNO3[i]
                });
            }

            layer.LayerIntervalsHydrochemistryPPO = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryPPO.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryPPO.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryPPO[i],
                    Color = layer.ColorsHydrochemistryPPO[i]
                });
            }

            layer.LayerIntervalsHydrochemistryCu = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCu.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCu.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCu[i],
                    Color = layer.ColorsHydrochemistryCu[i]
                });
            }

            layer.LayerIntervalsHydrochemistryZn = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryZn.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryZn.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryZn[i],
                    Color = layer.ColorsHydrochemistryZn[i]
                });
            }

            layer.LayerIntervalsHydrochemistryMn = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryMn.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryMn.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryMn[i],
                    Color = layer.ColorsHydrochemistryMn[i]
                });
            }

            layer.LayerIntervalsHydrochemistryPb = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryPb.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryPb.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryPb[i],
                    Color = layer.ColorsHydrochemistryPb[i]
                });
            }

            layer.LayerIntervalsHydrochemistryNi = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNi.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNi.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNi[i],
                    Color = layer.ColorsHydrochemistryNi[i]
                });
            }

            layer.LayerIntervalsHydrochemistryCd = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCd.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCd.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCd[i],
                    Color = layer.ColorsHydrochemistryCd[i]
                });
            }

            layer.LayerIntervalsHydrochemistryCo = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCo.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCo.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCo[i],
                    Color = layer.ColorsHydrochemistryCo[i]
                });
            }

            layer.LayerIntervalsHydrochemistryCIWP = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCIWP.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCIWP.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCIWP[i],
                    Color = layer.ColorsHydrochemistryCIWP[i]
                });
            }

            return View(layer);
        }

        // POST: Layers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Lake,GeoServerName,FileNameWithPath,GeoServerStyle,NameKK,NameRU,NameEN,LayerIntervalsWaterLevel,LayerIntervalsSurfaceFlow,LayerIntervalsPrecipitation,LayerIntervalsUndergroundFlow,LayerIntervalsSurfaceOutflow,LayerIntervalsEvaporation,LayerIntervalsUndergroundOutflow,LayerIntervalsHydrochemistry")] Layer layer)
        public async Task<IActionResult> Edit(int id, [Bind("Id,Lake,GeoServerName,FileNameWithPath,GeoServerStyle,NameKK,NameRU,NameEN,Tags,DescriptionKK,DescriptionRU,DescriptionEN,LayerIntervalsWaterLevel,LayerIntervalsSurfaceFlow,LayerIntervalsPrecipitation,LayerIntervalsUndergroundFlow,LayerIntervalsSurfaceOutflow,LayerIntervalsEvaporation,LayerIntervalsUndergroundOutflow,LayerIntervalsHydrochemistryMineralization,LayerIntervalsHydrochemistryTotalHardness,LayerIntervalsHydrochemistryDissOxygWater,LayerIntervalsHydrochemistryPercentOxygWater,LayerIntervalsHydrochemistrypH,LayerIntervalsHydrochemistryOrganicSubstances,LayerIntervalsHydrochemistryCa,LayerIntervalsHydrochemistryMg,LayerIntervalsHydrochemistryNaK,LayerIntervalsHydrochemistryCl,LayerIntervalsHydrochemistryHCO,LayerIntervalsHydrochemistrySO,LayerIntervalsHydrochemistryNH,LayerIntervalsHydrochemistryNO2,LayerIntervalsHydrochemistryNO3,LayerIntervalsHydrochemistryPPO,LayerIntervalsHydrochemistryCu,LayerIntervalsHydrochemistryZn,LayerIntervalsHydrochemistryMn,LayerIntervalsHydrochemistryPb,LayerIntervalsHydrochemistryNi,LayerIntervalsHydrochemistryCd,LayerIntervalsHydrochemistryCo,LayerIntervalsHydrochemistryCIWP")] Layer layer)
        {
            if (id != layer.Id)
            {
                return NotFound();
            }

            if (layer.LayerIntervalsWaterLevel == null)
            {
                layer.LayerIntervalsWaterLevel = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsWaterLevel.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsWaterLevel[i].Color))
                {
                    layer.LayerIntervalsWaterLevel.RemoveAt(i);
                }
            }
            layer.MinValuesWaterLevel = layer.LayerIntervalsWaterLevel.Select(m => m.MinValue).ToArray();
            layer.ColorsWaterLevel = layer.LayerIntervalsWaterLevel.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsSurfaceFlow == null)
            {
                layer.LayerIntervalsSurfaceFlow = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsSurfaceFlow.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsSurfaceFlow[i].Color))
                {
                    layer.LayerIntervalsSurfaceFlow.RemoveAt(i);
                }
            }
            layer.MinValuesSurfaceFlow = layer.LayerIntervalsSurfaceFlow.Select(m => m.MinValue).ToArray();
            layer.ColorsSurfaceFlow = layer.LayerIntervalsSurfaceFlow.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsPrecipitation == null)
            {
                layer.LayerIntervalsPrecipitation = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsPrecipitation.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsPrecipitation[i].Color))
                {
                    layer.LayerIntervalsPrecipitation.RemoveAt(i);
                }
            }
            layer.MinValuesPrecipitation = layer.LayerIntervalsPrecipitation.Select(m => m.MinValue).ToArray();
            layer.ColorsPrecipitation = layer.LayerIntervalsPrecipitation.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsUndergroundFlow == null)
            {
                layer.LayerIntervalsUndergroundFlow = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsUndergroundFlow.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsUndergroundFlow[i].Color))
                {
                    layer.LayerIntervalsUndergroundFlow.RemoveAt(i);
                }
            }
            layer.MinValuesUndergroundFlow = layer.LayerIntervalsUndergroundFlow.Select(m => m.MinValue).ToArray();
            layer.ColorsUndergroundFlow = layer.LayerIntervalsUndergroundFlow.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsSurfaceOutflow == null)
            {
                layer.LayerIntervalsSurfaceOutflow = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsSurfaceOutflow.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsSurfaceOutflow[i].Color))
                {
                    layer.LayerIntervalsSurfaceOutflow.RemoveAt(i);
                }
            }
            layer.MinValuesSurfaceOutflow = layer.LayerIntervalsSurfaceOutflow.Select(m => m.MinValue).ToArray();
            layer.ColorsSurfaceOutflow = layer.LayerIntervalsSurfaceOutflow.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsEvaporation == null)
            {
                layer.LayerIntervalsEvaporation = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsEvaporation.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsEvaporation[i].Color))
                {
                    layer.LayerIntervalsEvaporation.RemoveAt(i);
                }
            }
            layer.MinValuesEvaporation = layer.LayerIntervalsEvaporation.Select(m => m.MinValue).ToArray();
            layer.ColorsEvaporation = layer.LayerIntervalsEvaporation.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsUndergroundOutflow == null)
            {
                layer.LayerIntervalsUndergroundOutflow = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsUndergroundOutflow.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsUndergroundOutflow[i].Color))
                {
                    layer.LayerIntervalsUndergroundOutflow.RemoveAt(i);
                }
            }
            layer.MinValuesUndergroundOutflow = layer.LayerIntervalsUndergroundOutflow.Select(m => m.MinValue).ToArray();
            layer.ColorsUndergroundOutflow = layer.LayerIntervalsUndergroundOutflow.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryMineralization == null)
            {
                layer.LayerIntervalsHydrochemistryMineralization = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryMineralization.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryMineralization[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryMineralization.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryMineralization = layer.LayerIntervalsHydrochemistryMineralization.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryMineralization = layer.LayerIntervalsHydrochemistryMineralization.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryTotalHardness == null)
            {
                layer.LayerIntervalsHydrochemistryTotalHardness = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryTotalHardness.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryTotalHardness[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryTotalHardness.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryTotalHardness = layer.LayerIntervalsHydrochemistryTotalHardness.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryTotalHardness = layer.LayerIntervalsHydrochemistryTotalHardness.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryDissOxygWater == null)
            {
                layer.LayerIntervalsHydrochemistryDissOxygWater = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryDissOxygWater.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryDissOxygWater[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryDissOxygWater.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryDissOxygWater = layer.LayerIntervalsHydrochemistryDissOxygWater.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryDissOxygWater = layer.LayerIntervalsHydrochemistryDissOxygWater.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryPercentOxygWater == null)
            {
                layer.LayerIntervalsHydrochemistryPercentOxygWater = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryPercentOxygWater.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryPercentOxygWater[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryPercentOxygWater.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryPercentOxygWater = layer.LayerIntervalsHydrochemistryPercentOxygWater.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryPercentOxygWater = layer.LayerIntervalsHydrochemistryPercentOxygWater.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistrypH == null)
            {
                layer.LayerIntervalsHydrochemistrypH = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistrypH.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistrypH[i].Color))
                {
                    layer.LayerIntervalsHydrochemistrypH.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistrypH = layer.LayerIntervalsHydrochemistrypH.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistrypH = layer.LayerIntervalsHydrochemistrypH.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryOrganicSubstances == null)
            {
                layer.LayerIntervalsHydrochemistryOrganicSubstances = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryOrganicSubstances.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryOrganicSubstances[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryOrganicSubstances.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryOrganicSubstances = layer.LayerIntervalsHydrochemistryOrganicSubstances.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryOrganicSubstances = layer.LayerIntervalsHydrochemistryOrganicSubstances.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryCa == null)
            {
                layer.LayerIntervalsHydrochemistryCa = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryCa.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryCa[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryCa.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryCa = layer.LayerIntervalsHydrochemistryCa.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryCa = layer.LayerIntervalsHydrochemistryCa.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryMg == null)
            {
                layer.LayerIntervalsHydrochemistryMg = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryMg.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryMg[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryMg.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryMg = layer.LayerIntervalsHydrochemistryMg.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryMg = layer.LayerIntervalsHydrochemistryMg.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryNaK == null)
            {
                layer.LayerIntervalsHydrochemistryNaK = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryNaK.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryNaK[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryNaK.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryNaK = layer.LayerIntervalsHydrochemistryNaK.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryNaK = layer.LayerIntervalsHydrochemistryNaK.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryCl == null)
            {
                layer.LayerIntervalsHydrochemistryCl = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryCl.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryCl[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryCl.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryCl = layer.LayerIntervalsHydrochemistryCl.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryCl = layer.LayerIntervalsHydrochemistryCl.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryHCO == null)
            {
                layer.LayerIntervalsHydrochemistryHCO = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryHCO.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryHCO[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryHCO.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryHCO = layer.LayerIntervalsHydrochemistryHCO.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryHCO = layer.LayerIntervalsHydrochemistryHCO.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistrySO == null)
            {
                layer.LayerIntervalsHydrochemistrySO = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistrySO.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistrySO[i].Color))
                {
                    layer.LayerIntervalsHydrochemistrySO.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistrySO = layer.LayerIntervalsHydrochemistrySO.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistrySO = layer.LayerIntervalsHydrochemistrySO.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryNH == null)
            {
                layer.LayerIntervalsHydrochemistryNH = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryNH.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryNH[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryNH.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryNH = layer.LayerIntervalsHydrochemistryNH.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryNH = layer.LayerIntervalsHydrochemistryNH.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryNO2 == null)
            {
                layer.LayerIntervalsHydrochemistryNO2 = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryNO2.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryNO2[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryNO2.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryNO2 = layer.LayerIntervalsHydrochemistryNO2.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryNO2 = layer.LayerIntervalsHydrochemistryNO2.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryNO3 == null)
            {
                layer.LayerIntervalsHydrochemistryNO3 = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryNO3.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryNO3[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryNO3.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryNO3 = layer.LayerIntervalsHydrochemistryNO3.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryNO3 = layer.LayerIntervalsHydrochemistryNO3.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryPPO == null)
            {
                layer.LayerIntervalsHydrochemistryPPO = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryPPO.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryPPO[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryPPO.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryPPO = layer.LayerIntervalsHydrochemistryPPO.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryPPO = layer.LayerIntervalsHydrochemistryPPO.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryCu == null)
            {
                layer.LayerIntervalsHydrochemistryCu = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryCu.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryCu[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryCu.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryCu = layer.LayerIntervalsHydrochemistryCu.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryCu = layer.LayerIntervalsHydrochemistryCu.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryZn == null)
            {
                layer.LayerIntervalsHydrochemistryZn = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryZn.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryZn[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryZn.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryZn = layer.LayerIntervalsHydrochemistryZn.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryZn = layer.LayerIntervalsHydrochemistryZn.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryMn == null)
            {
                layer.LayerIntervalsHydrochemistryMn = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryMn.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryMn[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryMn.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryMn = layer.LayerIntervalsHydrochemistryMn.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryMn = layer.LayerIntervalsHydrochemistryMn.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryPb == null)
            {
                layer.LayerIntervalsHydrochemistryPb = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryPb.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryPb[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryPb.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryPb = layer.LayerIntervalsHydrochemistryPb.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryPb = layer.LayerIntervalsHydrochemistryPb.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryNi == null)
            {
                layer.LayerIntervalsHydrochemistryNi = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryNi.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryNi[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryNi.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryNi = layer.LayerIntervalsHydrochemistryNi.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryNi = layer.LayerIntervalsHydrochemistryNi.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryCd == null)
            {
                layer.LayerIntervalsHydrochemistryCd = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryCd.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryCd[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryCd.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryCd = layer.LayerIntervalsHydrochemistryCd.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryCd = layer.LayerIntervalsHydrochemistryCd.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryCo == null)
            {
                layer.LayerIntervalsHydrochemistryCo = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryCo.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryCo[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryCo.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryCo = layer.LayerIntervalsHydrochemistryCo.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryCo = layer.LayerIntervalsHydrochemistryCo.Select(m => m.Color).ToArray();

            if (layer.LayerIntervalsHydrochemistryCIWP == null)
            {
                layer.LayerIntervalsHydrochemistryCIWP = new List<LayerInterval>();
            }
            for (int i = layer.LayerIntervalsHydrochemistryCIWP.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(layer.LayerIntervalsHydrochemistryCIWP[i].Color))
                {
                    layer.LayerIntervalsHydrochemistryCIWP.RemoveAt(i);
                }
            }
            layer.MinValuesHydrochemistryCIWP = layer.LayerIntervalsHydrochemistryCIWP.Select(m => m.MinValue).ToArray();
            layer.ColorsHydrochemistryCIWP = layer.LayerIntervalsHydrochemistryCIWP.Select(m => m.Color).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(layer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LayerExists(layer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(layer);
        }


        // GET: Layers/Edit/5
        public async Task<IActionResult> EditNL(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layer.SingleOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }


            return View(layer);
        }

        // POST: Layers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Lake,GeoServerName,FileNameWithPath,GeoServerStyle,NameKK,NameRU,NameEN,LayerIntervalsWaterLevel,LayerIntervalsSurfaceFlow,LayerIntervalsPrecipitation,LayerIntervalsUndergroundFlow,LayerIntervalsSurfaceOutflow,LayerIntervalsEvaporation,LayerIntervalsUndergroundOutflow,LayerIntervalsHydrochemistry")] Layer layer)
        public async Task<IActionResult> EditNL(int id, [Bind("Id,Lake,GeoServerName,FileNameWithPath,GeoServerStyle,NameKK,NameRU,NameEN,Tags,DescriptionKK,DescriptionRU,DescriptionEN,LayerIntervalsWaterLevel,LayerIntervalsSurfaceFlow,LayerIntervalsPrecipitation,LayerIntervalsUndergroundFlow,LayerIntervalsSurfaceOutflow,LayerIntervalsEvaporation,LayerIntervalsUndergroundOutflow,LayerIntervalsHydrochemistryMineralization,LayerIntervalsHydrochemistryTotalHardness,LayerIntervalsHydrochemistryDissOxygWater,LayerIntervalsHydrochemistryPercentOxygWater,LayerIntervalsHydrochemistrypH,LayerIntervalsHydrochemistryOrganicSubstances,LayerIntervalsHydrochemistryCa,LayerIntervalsHydrochemistryMg,LayerIntervalsHydrochemistryNaK,LayerIntervalsHydrochemistryCl,LayerIntervalsHydrochemistryHCO,LayerIntervalsHydrochemistrySO,LayerIntervalsHydrochemistryNH,LayerIntervalsHydrochemistryNO2,LayerIntervalsHydrochemistryNO3,LayerIntervalsHydrochemistryPPO,LayerIntervalsHydrochemistryCu,LayerIntervalsHydrochemistryZn,LayerIntervalsHydrochemistryMn,LayerIntervalsHydrochemistryPb,LayerIntervalsHydrochemistryNi,LayerIntervalsHydrochemistryCd,LayerIntervalsHydrochemistryCo,LayerIntervalsHydrochemistryCIWP")] Layer layer)
        {
            if (id != layer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(layer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LayerExists(layer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(layer);
        }


        // GET: Layers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layer
                .SingleOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }

            return View(layer);
        }

        // POST: Layers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var layer = await _context.Layer.SingleOrDefaultAsync(m => m.Id == id);
            _context.Layer.Remove(layer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LayerExists(int id)
        {
            return _context.Layer.Any(e => e.Id == id);
        }

        [HttpPost]
        public JsonResult GetWaterLevelsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<WaterLevel> waterLevels = _context.WaterLevel.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsWaterLevel = new List<LayerInterval>();
            for (int i=0;i < layer.MinValuesWaterLevel.Count();i++)
            {
                layer.LayerIntervalsWaterLevel.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesWaterLevel[i],
                    Color = layer.ColorsWaterLevel[i]
                });
            }
            layer.LayerIntervalsWaterLevel = layer.LayerIntervalsWaterLevel.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach(int lake in shp)
            {
                WaterLevel waterLevel = waterLevels.FirstOrDefault(w => w.LakeId == lake);
                if(waterLevel != null)
                {
                    string color = "ffffff";
                    for(int i = 0;i< layer.LayerIntervalsWaterLevel.Count();i++)
                    {
                        if(waterLevel.WaterLavelM > layer.LayerIntervalsWaterLevel[i].MinValue)
                        {
                            color = layer.LayerIntervalsWaterLevel[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            //link = "env=color601810:00ff00;color500030:9c4555;color500062:56bb5e;color500063:d58e40;color500080:DC143C;color500090:66CDAA;color500100:FFF0F5;color500110:40E0D0;color500120:5F9EA0;color500130:000080;";
            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetSurfaceFlowsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<SurfaceFlow> surfaceFlows = _context.SurfaceFlow.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsSurfaceFlow = new List<LayerInterval>();
            for (int i=0;i < layer.MinValuesSurfaceFlow.Count();i++)
            {
                layer.LayerIntervalsSurfaceFlow.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesSurfaceFlow[i],
                    Color = layer.ColorsSurfaceFlow[i]
                });
            }
            layer.LayerIntervalsSurfaceFlow = layer.LayerIntervalsSurfaceFlow.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach(int lake in shp)
            {
                SurfaceFlow waterLevel = surfaceFlows.FirstOrDefault(w => w.LakeId == lake);
                if(waterLevel != null)
                {
                    string color = "ffffff";
                    for(int i = 0;i< layer.LayerIntervalsSurfaceFlow.Count();i++)
                    {
                        if(waterLevel.Value > layer.LayerIntervalsSurfaceFlow[i].MinValue)
                        {
                            color = layer.LayerIntervalsSurfaceFlow[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetPrecipitationsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Precipitation> precipitations = _context.Precipitation.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsPrecipitation = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesPrecipitation.Count(); i++)
            {
                layer.LayerIntervalsPrecipitation.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesPrecipitation[i],
                    Color = layer.ColorsPrecipitation[i]
                });
            }
            layer.LayerIntervalsPrecipitation = layer.LayerIntervalsPrecipitation.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Precipitation waterLevel = precipitations.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsPrecipitation.Count(); i++)
                    {
                        if (waterLevel.Value > layer.LayerIntervalsPrecipitation[i].MinValue)
                        {
                            color = layer.LayerIntervalsPrecipitation[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetUndergroundFlowsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<UndergroundFlow> undergroundFlows = _context.UndergroundFlow.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsUndergroundFlow = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesUndergroundFlow.Count(); i++)
            {
                layer.LayerIntervalsUndergroundFlow.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesUndergroundFlow[i],
                    Color = layer.ColorsUndergroundFlow[i]
                });
            }
            layer.LayerIntervalsUndergroundFlow = layer.LayerIntervalsUndergroundFlow.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                UndergroundFlow waterLevel = undergroundFlows.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsUndergroundFlow.Count(); i++)
                    {
                        if (waterLevel.Value > layer.LayerIntervalsUndergroundFlow[i].MinValue)
                        {
                            color = layer.LayerIntervalsUndergroundFlow[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetSurfaceOutflowsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<SurfaceOutflow> surfaceOutflows = _context.SurfaceOutflow.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsSurfaceOutflow = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesSurfaceOutflow.Count(); i++)
            {
                layer.LayerIntervalsSurfaceOutflow.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesSurfaceOutflow[i],
                    Color = layer.ColorsSurfaceOutflow[i]
                });
            }
            layer.LayerIntervalsSurfaceOutflow = layer.LayerIntervalsSurfaceOutflow.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                SurfaceOutflow waterLevel = surfaceOutflows.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsSurfaceOutflow.Count(); i++)
                    {
                        if (waterLevel.Value > layer.LayerIntervalsSurfaceOutflow[i].MinValue)
                        {
                            color = layer.LayerIntervalsSurfaceOutflow[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetEvaporationsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Evaporation> evaporations = _context.Evaporation.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsEvaporation = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesEvaporation.Count(); i++)
            {
                layer.LayerIntervalsEvaporation.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesEvaporation[i],
                    Color = layer.ColorsEvaporation[i]
                });
            }
            layer.LayerIntervalsEvaporation = layer.LayerIntervalsEvaporation.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Evaporation waterLevel = evaporations.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsEvaporation.Count(); i++)
                    {
                        if (waterLevel.Value > layer.LayerIntervalsEvaporation[i].MinValue)
                        {
                            color = layer.LayerIntervalsEvaporation[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetUndergroundOutflowsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<UndergroundOutflow> undergroundOutflows = _context.UndergroundOutflow.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsUndergroundOutflow = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesUndergroundOutflow.Count(); i++)
            {
                layer.LayerIntervalsUndergroundOutflow.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesUndergroundOutflow[i],
                    Color = layer.ColorsUndergroundOutflow[i]
                });
            }
            layer.LayerIntervalsUndergroundOutflow = layer.LayerIntervalsUndergroundOutflow.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                UndergroundOutflow waterLevel = undergroundOutflows.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsUndergroundOutflow.Count(); i++)
                    {
                        if (waterLevel.Value > layer.LayerIntervalsUndergroundOutflow[i].MinValue)
                        {
                            color = layer.LayerIntervalsUndergroundOutflow[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryMineralizationsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryMineralizations = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryMineralization = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryMineralization.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryMineralization.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryMineralization[i],
                    Color = layer.ColorsHydrochemistryMineralization[i]
                });
            }
            layer.LayerIntervalsHydrochemistryMineralization = layer.LayerIntervalsHydrochemistryMineralization.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryMineralizations.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryMineralization.Count(); i++)
                    {
                        if (waterLevel.Mineralization > layer.LayerIntervalsHydrochemistryMineralization[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryMineralization[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryTotalHardnesssStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryTotalHardnesss = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryTotalHardness = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryTotalHardness.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryTotalHardness.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryTotalHardness[i],
                    Color = layer.ColorsHydrochemistryTotalHardness[i]
                });
            }
            layer.LayerIntervalsHydrochemistryTotalHardness = layer.LayerIntervalsHydrochemistryTotalHardness.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryTotalHardnesss.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryTotalHardness.Count(); i++)
                    {
                        if (waterLevel.TotalHardness > layer.LayerIntervalsHydrochemistryTotalHardness[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryTotalHardness[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryDissOxygWatersStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryDissOxygWaters = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryDissOxygWater = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryDissOxygWater.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryDissOxygWater.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryDissOxygWater[i],
                    Color = layer.ColorsHydrochemistryDissOxygWater[i]
                });
            }
            layer.LayerIntervalsHydrochemistryDissOxygWater = layer.LayerIntervalsHydrochemistryDissOxygWater.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryDissOxygWaters.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryDissOxygWater.Count(); i++)
                    {
                        if (waterLevel.DissOxygWater > layer.LayerIntervalsHydrochemistryDissOxygWater[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryDissOxygWater[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryPercentOxygWatersStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryPercentOxygWaters = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryPercentOxygWater = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryPercentOxygWater.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryPercentOxygWater.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryPercentOxygWater[i],
                    Color = layer.ColorsHydrochemistryPercentOxygWater[i]
                });
            }
            layer.LayerIntervalsHydrochemistryPercentOxygWater = layer.LayerIntervalsHydrochemistryPercentOxygWater.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryPercentOxygWaters.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryPercentOxygWater.Count(); i++)
                    {
                        if (waterLevel.PercentOxygWater > layer.LayerIntervalsHydrochemistryPercentOxygWater[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryPercentOxygWater[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistrypHsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistrypHs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistrypH = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistrypH.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistrypH.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistrypH[i],
                    Color = layer.ColorsHydrochemistrypH[i]
                });
            }
            layer.LayerIntervalsHydrochemistrypH = layer.LayerIntervalsHydrochemistrypH.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistrypHs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistrypH.Count(); i++)
                    {
                        if (waterLevel.pH > layer.LayerIntervalsHydrochemistrypH[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistrypH[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryOrganicSubstancessStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryOrganicSubstancess = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryOrganicSubstances = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryOrganicSubstances.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryOrganicSubstances.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryOrganicSubstances[i],
                    Color = layer.ColorsHydrochemistryOrganicSubstances[i]
                });
            }
            layer.LayerIntervalsHydrochemistryOrganicSubstances = layer.LayerIntervalsHydrochemistryOrganicSubstances.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryOrganicSubstancess.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryOrganicSubstances.Count(); i++)
                    {
                        if (waterLevel.OrganicSubstances > layer.LayerIntervalsHydrochemistryOrganicSubstances[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryOrganicSubstances[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryCasStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryCas = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryCa = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCa.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCa.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCa[i],
                    Color = layer.ColorsHydrochemistryCa[i]
                });
            }
            layer.LayerIntervalsHydrochemistryCa = layer.LayerIntervalsHydrochemistryCa.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryCas.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryCa.Count(); i++)
                    {
                        if (waterLevel.Ca > layer.LayerIntervalsHydrochemistryCa[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryCa[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryMgsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryMgs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryMg = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryMg.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryMg.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryMg[i],
                    Color = layer.ColorsHydrochemistryMg[i]
                });
            }
            layer.LayerIntervalsHydrochemistryMg = layer.LayerIntervalsHydrochemistryMg.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryMgs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryMg.Count(); i++)
                    {
                        if (waterLevel.Mg > layer.LayerIntervalsHydrochemistryMg[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryMg[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryNaKsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryNaKs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryNaK = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNaK.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNaK.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNaK[i],
                    Color = layer.ColorsHydrochemistryNaK[i]
                });
            }
            layer.LayerIntervalsHydrochemistryNaK = layer.LayerIntervalsHydrochemistryNaK.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryNaKs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryNaK.Count(); i++)
                    {
                        if (waterLevel.NaK > layer.LayerIntervalsHydrochemistryNaK[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryNaK[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryClsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryCls = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryCl = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCl.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCl.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCl[i],
                    Color = layer.ColorsHydrochemistryCl[i]
                });
            }
            layer.LayerIntervalsHydrochemistryCl = layer.LayerIntervalsHydrochemistryCl.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryCls.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryCl.Count(); i++)
                    {
                        if (waterLevel.Cl > layer.LayerIntervalsHydrochemistryCl[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryCl[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryHCOsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryHCOs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryHCO = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryHCO.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryHCO.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryHCO[i],
                    Color = layer.ColorsHydrochemistryHCO[i]
                });
            }
            layer.LayerIntervalsHydrochemistryHCO = layer.LayerIntervalsHydrochemistryHCO.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryHCOs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryHCO.Count(); i++)
                    {
                        if (waterLevel.HCO > layer.LayerIntervalsHydrochemistryHCO[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryHCO[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistrySOsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistrySOs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistrySO = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistrySO.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistrySO.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistrySO[i],
                    Color = layer.ColorsHydrochemistrySO[i]
                });
            }
            layer.LayerIntervalsHydrochemistrySO = layer.LayerIntervalsHydrochemistrySO.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistrySOs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistrySO.Count(); i++)
                    {
                        if (waterLevel.SO > layer.LayerIntervalsHydrochemistrySO[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistrySO[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryNHsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryNHs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryNH = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNH.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNH.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNH[i],
                    Color = layer.ColorsHydrochemistryNH[i]
                });
            }
            layer.LayerIntervalsHydrochemistryNH = layer.LayerIntervalsHydrochemistryNH.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryNHs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryNH.Count(); i++)
                    {
                        if (waterLevel.NH > layer.LayerIntervalsHydrochemistryNH[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryNH[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryNO2sStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryNO2s = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryNO2 = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNO2.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNO2.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNO2[i],
                    Color = layer.ColorsHydrochemistryNO2[i]
                });
            }
            layer.LayerIntervalsHydrochemistryNO2 = layer.LayerIntervalsHydrochemistryNO2.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryNO2s.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryNO2.Count(); i++)
                    {
                        if (waterLevel.NO2 > layer.LayerIntervalsHydrochemistryNO2[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryNO2[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryNO3sStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryNO3s = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryNO3 = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNO3.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNO3.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNO3[i],
                    Color = layer.ColorsHydrochemistryNO3[i]
                });
            }
            layer.LayerIntervalsHydrochemistryNO3 = layer.LayerIntervalsHydrochemistryNO3.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryNO3s.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryNO3.Count(); i++)
                    {
                        if (waterLevel.NO3 > layer.LayerIntervalsHydrochemistryNO3[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryNO3[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryPPOsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryPPOs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryPPO = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryPPO.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryPPO.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryPPO[i],
                    Color = layer.ColorsHydrochemistryPPO[i]
                });
            }
            layer.LayerIntervalsHydrochemistryPPO = layer.LayerIntervalsHydrochemistryPPO.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryPPOs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryPPO.Count(); i++)
                    {
                        if (waterLevel.PPO > layer.LayerIntervalsHydrochemistryPPO[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryPPO[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryCusStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryCus = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryCu = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCu.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCu.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCu[i],
                    Color = layer.ColorsHydrochemistryCu[i]
                });
            }
            layer.LayerIntervalsHydrochemistryCu = layer.LayerIntervalsHydrochemistryCu.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryCus.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryCu.Count(); i++)
                    {
                        if (waterLevel.Cu > layer.LayerIntervalsHydrochemistryCu[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryCu[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryZnsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryZns = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryZn = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryZn.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryZn.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryZn[i],
                    Color = layer.ColorsHydrochemistryZn[i]
                });
            }
            layer.LayerIntervalsHydrochemistryZn = layer.LayerIntervalsHydrochemistryZn.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryZns.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryZn.Count(); i++)
                    {
                        if (waterLevel.Zn > layer.LayerIntervalsHydrochemistryZn[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryZn[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryMnsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryMns = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryMn = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryMn.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryMn.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryMn[i],
                    Color = layer.ColorsHydrochemistryMn[i]
                });
            }
            layer.LayerIntervalsHydrochemistryMn = layer.LayerIntervalsHydrochemistryMn.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryMns.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryMn.Count(); i++)
                    {
                        if (waterLevel.Mn > layer.LayerIntervalsHydrochemistryMn[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryMn[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryPbsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryPbs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryPb = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryPb.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryPb.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryPb[i],
                    Color = layer.ColorsHydrochemistryPb[i]
                });
            }
            layer.LayerIntervalsHydrochemistryPb = layer.LayerIntervalsHydrochemistryPb.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryPbs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryPb.Count(); i++)
                    {
                        if (waterLevel.Pb > layer.LayerIntervalsHydrochemistryPb[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryPb[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryNisStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryNis = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryNi = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryNi.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryNi.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryNi[i],
                    Color = layer.ColorsHydrochemistryNi[i]
                });
            }
            layer.LayerIntervalsHydrochemistryNi = layer.LayerIntervalsHydrochemistryNi.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryNis.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryNi.Count(); i++)
                    {
                        if (waterLevel.Ni > layer.LayerIntervalsHydrochemistryNi[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryNi[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryCdsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryCds = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryCd = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCd.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCd.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCd[i],
                    Color = layer.ColorsHydrochemistryCd[i]
                });
            }
            layer.LayerIntervalsHydrochemistryCd = layer.LayerIntervalsHydrochemistryCd.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryCds.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryCd.Count(); i++)
                    {
                        if (waterLevel.Cd > layer.LayerIntervalsHydrochemistryCd[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryCd[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryCosStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryCos = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryCo = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCo.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCo.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCo[i],
                    Color = layer.ColorsHydrochemistryCo[i]
                });
            }
            layer.LayerIntervalsHydrochemistryCo = layer.LayerIntervalsHydrochemistryCo.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryCos.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryCo.Count(); i++)
                    {
                        if (waterLevel.Co > layer.LayerIntervalsHydrochemistryCo[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryCo[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }

        [HttpPost]
        public JsonResult GetHydrochemistryCIWPsStyleLink(int Year, int LayerId)
        {
            Layer layer = _context.Layer.SingleOrDefault(m => m.Id == LayerId);

            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(layer.FileNameWithPath, "id").ToList();

            List<Hydrochemistry> HydrochemistryCIWPs = _context.Hydrochemistry.Where(w => w.Year == Year).ToList();
            layer.LayerIntervalsHydrochemistryCIWP = new List<LayerInterval>();
            for (int i = 0; i < layer.MinValuesHydrochemistryCIWP.Count(); i++)
            {
                layer.LayerIntervalsHydrochemistryCIWP.Add(new LayerInterval()
                {
                    MinValue = layer.MinValuesHydrochemistryCIWP[i],
                    Color = layer.ColorsHydrochemistryCIWP[i]
                });
            }
            layer.LayerIntervalsHydrochemistryCIWP = layer.LayerIntervalsHydrochemistryCIWP.OrderBy(l => l.MinValue).ToList();

            string link = "env=";

            foreach (int lake in shp)
            {
                Hydrochemistry waterLevel = HydrochemistryCIWPs.FirstOrDefault(w => w.LakeId == lake);
                if (waterLevel != null)
                {
                    string color = "ffffff";
                    for (int i = 0; i < layer.LayerIntervalsHydrochemistryCIWP.Count(); i++)
                    {
                        if (waterLevel.CIWP > layer.LayerIntervalsHydrochemistryCIWP[i].MinValue)
                        {
                            color = layer.LayerIntervalsHydrochemistryCIWP[i].Color;
                        }
                    }
                    link += $"color{lake.ToString()}:{color.Replace("#", "")};";
                }
            }

            JsonResult result = new JsonResult(link);
            return result;
        }
    }
}
