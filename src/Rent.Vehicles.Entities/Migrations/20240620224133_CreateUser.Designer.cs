// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Rent.Vehicles.Entities.Contexts;

#nullable disable

namespace Rent.Vehicles.Entities.Migrations
{
    [DbContext(typeof(RentVehiclesContext))]
    [Migration("20240620224133_CreateUser")]
    partial class CreateUser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Rent.Vehicles.Entities.Command", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("ActionType")
                        .HasColumnType("integer")
                        .HasColumnName("action_type");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("smallint[]")
                        .HasColumnName("data");

                    b.Property<int>("EntityType")
                        .HasColumnType("integer")
                        .HasColumnName("entity_type");

                    b.Property<Guid>("SagaId")
                        .HasColumnType("uuid")
                        .HasColumnName("saga_id");

                    b.Property<int>("SerializerType")
                        .HasColumnType("integer")
                        .HasColumnName("serializer_type");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_commands");

                    b.ToTable("commands", "events");
                });

            modelBuilder.Entity("Rent.Vehicles.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("birthday");

                    b.Property<string>("LicenseNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("license_number");

                    b.Property<string>("LicensePath")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("license_path");

                    b.Property<int>("LicenseType")
                        .HasColumnType("integer")
                        .HasColumnName("license_type");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("number");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", "vehicles");
                });

            modelBuilder.Entity("Rent.Vehicles.Entities.Vehicle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("LicensePlate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("license_plate");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("model");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<int>("Year")
                        .HasColumnType("integer")
                        .HasColumnName("year");

                    b.HasKey("Id")
                        .HasName("pk_vehicles");

                    b.ToTable("vehicles", "vehicles");
                });
#pragma warning restore 612, 618
        }
    }
}
