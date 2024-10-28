﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectBase.Helpers;

#nullable disable

namespace ProjectBase.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProjectBase.Models.BlogsModel", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("blog_picture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("link_media")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("publishAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("status")
                        .HasColumnType("bit");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("userID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("ProjectBase.Models.Blogs_CategoryModel", b =>
                {
                    b.Property<long>("BlogID")
                        .HasColumnType("bigint");

                    b.Property<long>("CategoryID")
                        .HasColumnType("bigint");

                    b.ToTable("Blogs_Category");
                });

            modelBuilder.Entity("ProjectBase.Models.CategoryModel", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ProjectBase.Models.DAO.User", b =>
                {
                    b.Property<long?>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long?>("ID"));

                    b.Property<DateTime?>("Dob")
                        .HasColumnType("datetime2");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("RoleID")
                        .HasColumnType("bigint");

                    b.Property<string>("address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fullname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("gender")
                        .HasColumnType("bit");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("profile_picture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("register_date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("status")
                        .HasColumnType("int");

                    b.Property<string>("verificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProjectBase.Models.PricePackageModel", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("ListPrice")
                        .HasColumnType("bigint");

                    b.Property<int>("PackageType")
                        .HasColumnType("int");

                    b.Property<long>("SalePrice")
                        .HasColumnType("bigint");

                    b.Property<long>("SubjectID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.ToTable("PricePackages");
                });

            modelBuilder.Entity("ProjectBase.Models.SliderModel", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("backlink")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("status")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("Slider");
                });

            modelBuilder.Entity("ProjectBase.Models.SubjectsModel", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("UserID")
                        .HasColumnType("bigint");

                    b.Property<string>("brief_info")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("isHot")
                        .HasColumnType("bit");

                    b.Property<int>("rate")
                        .HasColumnType("int");

                    b.Property<string>("thumbnail_color")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Subjects");
                });
#pragma warning restore 612, 618
        }
    }
}
