﻿// <auto-generated />
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Infrastructure.AutoRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("RoleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.ToTable("AutoRoles");
                });

            modelBuilder.Entity("Infrastructure.Rank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("RoleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.ToTable("Ranks");
                });

            modelBuilder.Entity("Infrastructure.Server", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.Property<decimal>("Announcements")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Background")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Logs")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Prefix")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Welcome")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });
#pragma warning restore 612, 618
        }
    }
}
