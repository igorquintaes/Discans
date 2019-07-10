﻿// <auto-generated />
using System;
using Discans.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Discans.Shared.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Discans.Shared.Models.Manga", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LastRelease");

                    b.Property<int>("MangaSite");

                    b.Property<string>("MangaSiteId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Mangas");
                });

            modelBuilder.Entity("Discans.Shared.Models.PrivateAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("MangaId");

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("MangaId");

                    b.ToTable("PrivateAlerts");
                });

            modelBuilder.Entity("Discans.Shared.Models.ServerAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("MangaId");

                    b.Property<ulong>("ServerId");

                    b.HasKey("Id");

                    b.HasIndex("MangaId");

                    b.ToTable("ServerAlerts");
                });

            modelBuilder.Entity("Discans.Shared.Models.ServerChannel", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("ChannelId");

                    b.HasKey("Id");

                    b.ToTable("ServerChannels");
                });

            modelBuilder.Entity("Discans.Shared.Models.ServerLocalizer", b =>
                {
                    b.Property<ulong>("ServerId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.HasKey("ServerId");

                    b.ToTable("ServerLocalizer");
                });

            modelBuilder.Entity("Discans.Shared.Models.UserAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("MangaId");

                    b.Property<ulong>("ServerId");

                    b.Property<ulong>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("MangaId");

                    b.ToTable("UserAlerts");
                });

            modelBuilder.Entity("Discans.Shared.Models.UserLocalizer", b =>
                {
                    b.Property<ulong>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.HasKey("UserId");

                    b.ToTable("UserLocalizer");
                });

            modelBuilder.Entity("Discans.Shared.Models.PrivateAlert", b =>
                {
                    b.HasOne("Discans.Shared.Models.Manga", "Manga")
                        .WithMany("PrivateAlerts")
                        .HasForeignKey("MangaId");
                });

            modelBuilder.Entity("Discans.Shared.Models.ServerAlert", b =>
                {
                    b.HasOne("Discans.Shared.Models.Manga", "Manga")
                        .WithMany("ServerAlerts")
                        .HasForeignKey("MangaId");
                });

            modelBuilder.Entity("Discans.Shared.Models.UserAlert", b =>
                {
                    b.HasOne("Discans.Shared.Models.Manga", "Manga")
                        .WithMany("UserAlerts")
                        .HasForeignKey("MangaId");
                });
#pragma warning restore 612, 618
        }
    }
}
