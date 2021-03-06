﻿// <auto-generated />
using ELake.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ELake.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20181009080550_Layer_20181009_00")]
    partial class Layer_20181009_00
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("ELake.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("ELake.Models.Evaporation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LakeId");

                    b.Property<decimal>("Value");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("Evaporation");
                });

            modelBuilder.Entity("ELake.Models.Hydrochemistry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("CIWP");

                    b.Property<decimal?>("Ca");

                    b.Property<decimal?>("Cd");

                    b.Property<decimal?>("Cl");

                    b.Property<decimal?>("Co");

                    b.Property<decimal?>("Cu");

                    b.Property<decimal?>("DissOxygWater");

                    b.Property<decimal?>("HCO");

                    b.Property<int>("LakeId");

                    b.Property<decimal?>("Mg");

                    b.Property<decimal?>("Mineralization");

                    b.Property<decimal?>("Mn");

                    b.Property<decimal?>("NH");

                    b.Property<decimal?>("NO2");

                    b.Property<decimal?>("NO3");

                    b.Property<decimal?>("NaK");

                    b.Property<decimal?>("Ni");

                    b.Property<decimal?>("OrganicSubstances");

                    b.Property<decimal?>("PPO");

                    b.Property<decimal?>("Pb");

                    b.Property<decimal?>("PercentOxygWater");

                    b.Property<decimal?>("SO");

                    b.Property<decimal?>("TotalHardness");

                    b.Property<int>("Year");

                    b.Property<decimal?>("Zn");

                    b.Property<decimal?>("pH");

                    b.HasKey("Id");

                    b.ToTable("Hydrochemistry");
                });

            modelBuilder.Entity("ELake.Models.KATO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Level");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.Property<string>("Number");

                    b.HasKey("Id");

                    b.ToTable("KATO");
                });

            modelBuilder.Entity("ELake.Models.LakeKATO", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("KATOId");

                    b.Property<int>("LakeId");

                    b.HasKey("Id");

                    b.HasIndex("KATOId");

                    b.ToTable("LakeKATO");
                });

            modelBuilder.Entity("ELake.Models.Layer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string[]>("ColorsEvaporation");

                    b.Property<string[]>("ColorsHydrochemistryCIWP");

                    b.Property<string[]>("ColorsHydrochemistryCa");

                    b.Property<string[]>("ColorsHydrochemistryCd");

                    b.Property<string[]>("ColorsHydrochemistryCl");

                    b.Property<string[]>("ColorsHydrochemistryCo");

                    b.Property<string[]>("ColorsHydrochemistryCu");

                    b.Property<string[]>("ColorsHydrochemistryDissOxygWater");

                    b.Property<string[]>("ColorsHydrochemistryHCO");

                    b.Property<string[]>("ColorsHydrochemistryMg");

                    b.Property<string[]>("ColorsHydrochemistryMineralization");

                    b.Property<string[]>("ColorsHydrochemistryMn");

                    b.Property<string[]>("ColorsHydrochemistryNH");

                    b.Property<string[]>("ColorsHydrochemistryNO2");

                    b.Property<string[]>("ColorsHydrochemistryNO3");

                    b.Property<string[]>("ColorsHydrochemistryNaK");

                    b.Property<string[]>("ColorsHydrochemistryNi");

                    b.Property<string[]>("ColorsHydrochemistryOrganicSubstances");

                    b.Property<string[]>("ColorsHydrochemistryPPO");

                    b.Property<string[]>("ColorsHydrochemistryPb");

                    b.Property<string[]>("ColorsHydrochemistryPercentOxygWater");

                    b.Property<string[]>("ColorsHydrochemistrySO");

                    b.Property<string[]>("ColorsHydrochemistryTotalHardness");

                    b.Property<string[]>("ColorsHydrochemistryZn");

                    b.Property<string[]>("ColorsHydrochemistrypH");

                    b.Property<string[]>("ColorsPrecipitation");

                    b.Property<string[]>("ColorsSurfaceFlow");

                    b.Property<string[]>("ColorsSurfaceOutflow");

                    b.Property<string[]>("ColorsUndergroundFlow");

                    b.Property<string[]>("ColorsUndergroundOutflow");

                    b.Property<string[]>("ColorsWaterLevel");

                    b.Property<string>("FileNameWithPath");

                    b.Property<string>("GeoServerName");

                    b.Property<string>("GeoServerStyle");

                    b.Property<bool>("Lake");

                    b.Property<int?>("MapId");

                    b.Property<string>("MetaData");

                    b.Property<decimal[]>("MinValuesEvaporation");

                    b.Property<decimal[]>("MinValuesHydrochemistryCIWP");

                    b.Property<decimal[]>("MinValuesHydrochemistryCa");

                    b.Property<decimal[]>("MinValuesHydrochemistryCd");

                    b.Property<decimal[]>("MinValuesHydrochemistryCl");

                    b.Property<decimal[]>("MinValuesHydrochemistryCo");

                    b.Property<decimal[]>("MinValuesHydrochemistryCu");

                    b.Property<decimal[]>("MinValuesHydrochemistryDissOxygWater");

                    b.Property<decimal[]>("MinValuesHydrochemistryHCO");

                    b.Property<decimal[]>("MinValuesHydrochemistryMg");

                    b.Property<decimal[]>("MinValuesHydrochemistryMineralization");

                    b.Property<decimal[]>("MinValuesHydrochemistryMn");

                    b.Property<decimal[]>("MinValuesHydrochemistryNH");

                    b.Property<decimal[]>("MinValuesHydrochemistryNO2");

                    b.Property<decimal[]>("MinValuesHydrochemistryNO3");

                    b.Property<decimal[]>("MinValuesHydrochemistryNaK");

                    b.Property<decimal[]>("MinValuesHydrochemistryNi");

                    b.Property<decimal[]>("MinValuesHydrochemistryOrganicSubstances");

                    b.Property<decimal[]>("MinValuesHydrochemistryPPO");

                    b.Property<decimal[]>("MinValuesHydrochemistryPb");

                    b.Property<decimal[]>("MinValuesHydrochemistryPercentOxygWater");

                    b.Property<decimal[]>("MinValuesHydrochemistrySO");

                    b.Property<decimal[]>("MinValuesHydrochemistryTotalHardness");

                    b.Property<decimal[]>("MinValuesHydrochemistryZn");

                    b.Property<decimal[]>("MinValuesHydrochemistrypH");

                    b.Property<decimal[]>("MinValuesPrecipitation");

                    b.Property<decimal[]>("MinValuesSurfaceFlow");

                    b.Property<decimal[]>("MinValuesSurfaceOutflow");

                    b.Property<decimal[]>("MinValuesUndergroundFlow");

                    b.Property<decimal[]>("MinValuesUndergroundOutflow");

                    b.Property<decimal[]>("MinValuesWaterLevel");

                    b.Property<string>("NameEN");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.Property<string>("Tags");

                    b.HasKey("Id");

                    b.HasIndex("MapId");

                    b.ToTable("Layer");
                });

            modelBuilder.Entity("ELake.Models.Map", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int[]>("LayersId");

                    b.Property<string>("NameEN");

                    b.Property<string>("NameKK");

                    b.Property<string>("NameRU");

                    b.HasKey("Id");

                    b.ToTable("Map");
                });

            modelBuilder.Entity("ELake.Models.Precipitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LakeId");

                    b.Property<decimal>("Value");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("Precipitation");
                });

            modelBuilder.Entity("ELake.Models.SurfaceFlow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LakeId");

                    b.Property<decimal>("Value");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("SurfaceFlow");
                });

            modelBuilder.Entity("ELake.Models.SurfaceOutflow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LakeId");

                    b.Property<decimal>("Value");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("SurfaceOutflow");
                });

            modelBuilder.Entity("ELake.Models.UndergroundFlow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LakeId");

                    b.Property<decimal>("Value");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("UndergroundFlow");
                });

            modelBuilder.Entity("ELake.Models.UndergroundOutflow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LakeId");

                    b.Property<decimal>("Value");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("UndergroundOutflow");
                });

            modelBuilder.Entity("ELake.Models.WaterLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("LakeId");

                    b.Property<decimal>("WaterLavelM");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("WaterLevel");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ELake.Models.LakeKATO", b =>
                {
                    b.HasOne("ELake.Models.KATO", "KATO")
                        .WithMany()
                        .HasForeignKey("KATOId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ELake.Models.Layer", b =>
                {
                    b.HasOne("ELake.Models.Map")
                        .WithMany("Layers")
                        .HasForeignKey("MapId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ELake.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ELake.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ELake.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ELake.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
