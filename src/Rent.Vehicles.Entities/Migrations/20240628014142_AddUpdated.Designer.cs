﻿// <auto-generated />
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
    [Migration("20240628014142_AddUpdated")]
    partial class AddUpdated
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
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

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

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

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_commands");

                    b.HasAlternateKey("SagaId")
                        .HasName("ak_commands_saga_id");

                    b.ToTable("commands", "events");
                });

            modelBuilder.Entity("Rent.Vehicles.Entities.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("smallint[]")
                        .HasColumnName("data");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("SagaId")
                        .HasColumnType("uuid")
                        .HasColumnName("saga_id");

                    b.Property<int>("SerializerType")
                        .HasColumnType("integer")
                        .HasColumnName("serializer_type");

                    b.Property<int>("StatusType")
                        .HasColumnType("integer")
                        .HasColumnName("status_type");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_events");

                    b.HasIndex("SagaId")
                        .HasDatabaseName("ix_events_saga_id");

                    b.ToTable("events", "events");
                });

            modelBuilder.Entity("Rent.Vehicles.Entities.Rent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("Cost")
                        .HasColumnType("numeric")
                        .HasColumnName("cost");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

                    b.Property<decimal>("DailyCost")
                        .HasColumnType("numeric")
                        .HasColumnName("daily_cost");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("end_date");

                    b.Property<DateTime>("EstimatedDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("estimated_date");

                    b.Property<int>("NumberOfDays")
                        .HasColumnType("integer")
                        .HasColumnName("number_of_days");

                    b.Property<decimal>("PostEndDateFine")
                        .HasColumnType("numeric")
                        .HasColumnName("post_end_date_fine");

                    b.Property<decimal>("PreEndDatePercentageFine")
                        .HasColumnType("numeric")
                        .HasColumnName("pre_end_date_percentage_fine");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_date");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("uuid")
                        .HasColumnName("vehicle_id");

                    b.HasKey("Id")
                        .HasName("pk_rents");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_rents_user_id");

                    b.HasIndex("VehicleId")
                        .HasDatabaseName("ix_rents_vehicle_id");

                    b.ToTable("rents", "vehicles");
                });

            modelBuilder.Entity("Rent.Vehicles.Entities.RentalPlane", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

                    b.Property<decimal>("DailyCost")
                        .HasColumnType("numeric")
                        .HasColumnName("daily_cost");

                    b.Property<int>("NumberOfDays")
                        .HasColumnType("integer")
                        .HasColumnName("number_of_days");

                    b.Property<decimal>("PostEndDateFine")
                        .HasColumnType("numeric")
                        .HasColumnName("post_end_date_fine");

                    b.Property<decimal>("PreEndDatePercentageFine")
                        .HasColumnType("numeric")
                        .HasColumnName("pre_end_date_percentage_fine");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated");

                    b.HasKey("Id")
                        .HasName("pk_rental_planes");

                    b.ToTable("rentalPlanes", "vehicles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("9aeaca2f-d9c3-4a33-824a-c2ef92dd4355"),
                            Created = new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(910),
                            DailyCost = 30.0m,
                            NumberOfDays = 7,
                            PostEndDateFine = 50.0m,
                            PreEndDatePercentageFine = 1.20m
                        },
                        new
                        {
                            Id = new Guid("c0269299-d465-42ef-820c-6b13eef29ee9"),
                            Created = new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(947),
                            DailyCost = 28.0m,
                            NumberOfDays = 15,
                            PostEndDateFine = 50.0m,
                            PreEndDatePercentageFine = 1.40m
                        },
                        new
                        {
                            Id = new Guid("e3d5062d-ca48-40cb-b248-a9b7d47e0599"),
                            Created = new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(954),
                            DailyCost = 22.0m,
                            NumberOfDays = 30,
                            PostEndDateFine = 50.0m,
                            PreEndDatePercentageFine = 1.0m
                        },
                        new
                        {
                            Id = new Guid("df107815-8295-43d3-a023-4adb387a8601"),
                            Created = new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(960),
                            DailyCost = 20.0m,
                            NumberOfDays = 45,
                            PostEndDateFine = 50.0m,
                            PreEndDatePercentageFine = 1.0m
                        },
                        new
                        {
                            Id = new Guid("0a2c4969-093b-44e6-908d-7bb8712ed431"),
                            Created = new DateTime(2024, 6, 28, 1, 41, 42, 536, DateTimeKind.Utc).AddTicks(965),
                            DailyCost = 18.0m,
                            NumberOfDays = 50,
                            PostEndDateFine = 50.0m,
                            PreEndDatePercentageFine = 1.0m
                        });
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

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

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

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated");

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

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("created");

                    b.Property<bool>("IsRented")
                        .HasColumnType("boolean")
                        .HasColumnName("is_rented");

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

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("updated");

                    b.Property<int>("Year")
                        .HasColumnType("integer")
                        .HasColumnName("year");

                    b.HasKey("Id")
                        .HasName("pk_vehicles");

                    b.ToTable("vehicles", "vehicles");
                });

            modelBuilder.Entity("Rent.Vehicles.Entities.Event", b =>
                {
                    b.HasOne("Rent.Vehicles.Entities.Command", null)
                        .WithMany()
                        .HasForeignKey("SagaId")
                        .HasPrincipalKey("SagaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_events_commands_saga_id");
                });

            modelBuilder.Entity("Rent.Vehicles.Entities.Rent", b =>
                {
                    b.HasOne("Rent.Vehicles.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_rents_users_user_id");

                    b.HasOne("Rent.Vehicles.Entities.Vehicle", "Vehicle")
                        .WithMany()
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_rents_vehicles_vehicle_id");

                    b.Navigation("User");

                    b.Navigation("Vehicle");
                });
#pragma warning restore 612, 618
        }
    }
}
