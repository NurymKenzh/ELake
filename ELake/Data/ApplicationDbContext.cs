using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ELake.Models;

namespace ELake.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<ELake.Models.Layer> Layer { get; set; }

        public DbSet<ELake.Models.Map> Map { get; set; }

        public DbSet<ELake.Models.WaterLevel> WaterLevel { get; set; }

        public DbSet<ELake.Models.SurfaceFlow> SurfaceFlow { get; set; }

        public DbSet<ELake.Models.Precipitation> Precipitation { get; set; }

        public DbSet<ELake.Models.UndergroundFlow> UndergroundFlow { get; set; }

        public DbSet<ELake.Models.SurfaceOutflow> SurfaceOutflow { get; set; }

        public DbSet<ELake.Models.Evaporation> Evaporation { get; set; }

        public DbSet<ELake.Models.UndergroundOutflow> UndergroundOutflow { get; set; }

        public DbSet<ELake.Models.Hydrochemistry> Hydrochemistry { get; set; }

        public DbSet<ELake.Models.KATO> KATO { get; set; }

        public DbSet<ELake.Models.LakeKATO> LakeKATO { get; set; }

        public DbSet<ELake.Models.Lake> Lake { get; set; }

        public DbSet<ELake.Models.LakesArchiveData> LakesArchiveData { get; set; }

        public DbSet<ELake.Models.LakesGlobalData> LakesGlobalData { get; set; }

        public DbSet<ELake.Models.WaterBalance> WaterBalance { get; set; }
    }
}
