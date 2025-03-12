﻿// <auto-generated />
using System;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(TimeScaleContext))]
    [Migration("20250312083553_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DAL.Entities.BinarySample", b =>
                {
                    b.Property<int>("FK_Datapoint")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("Status")
                        .HasColumnType("integer");

                    b.Property<bool>("Value")
                        .HasColumnType("boolean");

                    b.HasKey("FK_Datapoint", "Time", "CreationDate");

                    b.HasIndex("Time", "FK_Datapoint")
                        .IsDescending();

                    b.ToTable("BinarySamples", (string)null);
                });

            modelBuilder.Entity("DAL.Entities.DataPoint", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<int>("DataType")
                        .HasColumnType("integer");

                    b.Property<string>("DatapointType")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("FK_Device")
                        .HasColumnType("integer");

                    b.Property<string>("Icon")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Offset")
                        .HasColumnType("integer");

                    b.Property<string>("Unit")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("FK_Device");

                    b.ToTable("DataPoints", (string)null);

                    b.HasDiscriminator<string>("DatapointType").HasValue("DataPoint");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("DAL.Entities.Device", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("Aquarium")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DeviceDescription")
                        .HasColumnType("text");

                    b.Property<string>("DeviceName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DeviceType")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.HasKey("ID");

                    b.ToTable("Devices", (string)null);

                    b.HasDiscriminator<string>("DeviceType").HasValue("Device");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("DAL.Entities.NumericSample", b =>
                {
                    b.Property<int>("FK_Datapoint")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("Status")
                        .HasColumnType("integer");

                    b.Property<float>("Value")
                        .HasColumnType("real");

                    b.HasKey("FK_Datapoint", "Time", "CreationDate");

                    b.HasIndex("Time", "FK_Datapoint")
                        .IsDescending();

                    b.ToTable("NumericSamples", (string)null);
                });

            modelBuilder.Entity("DAL.Entities.Devices.MQTTDataPoint", b =>
                {
                    b.HasBaseType("DAL.Entities.DataPoint");

                    b.Property<string>("TopicName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("MQTT");
                });

            modelBuilder.Entity("DAL.Entities.Devices.ModbusDataPoint", b =>
                {
                    b.HasBaseType("DAL.Entities.DataPoint");

                    b.Property<int>("ReadingType")
                        .HasColumnType("integer");

                    b.Property<int>("Register")
                        .HasColumnType("integer");

                    b.Property<int>("RegisterCount")
                        .HasColumnType("integer");

                    b.Property<int>("RegisterType")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue("Modbus");
                });

            modelBuilder.Entity("DAL.Entities.Devices.MQTTDevice", b =>
                {
                    b.HasBaseType("DAL.Entities.Device");

                    b.Property<string>("Host")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("text")
                        .HasColumnName("Host");

                    b.Property<int>("Port")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("integer")
                        .HasColumnName("Port");

                    b.HasDiscriminator().HasValue("MQTT");
                });

            modelBuilder.Entity("DAL.Entities.Devices.ModbusDevice", b =>
                {
                    b.HasBaseType("DAL.Entities.Device");

                    b.Property<string>("Host")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("text")
                        .HasColumnName("Host");

                    b.Property<int>("Port")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("integer")
                        .HasColumnName("Port");

                    b.Property<int>("SlaveID")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue("Modbus");
                });

            modelBuilder.Entity("DAL.Entities.BinarySample", b =>
                {
                    b.HasOne("DAL.Entities.DataPoint", "DataPoint")
                        .WithMany()
                        .HasForeignKey("FK_Datapoint")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataPoint");
                });

            modelBuilder.Entity("DAL.Entities.DataPoint", b =>
                {
                    b.HasOne("DAL.Entities.Device", "Device")
                        .WithMany()
                        .HasForeignKey("FK_Device")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("DAL.Entities.NumericSample", b =>
                {
                    b.HasOne("DAL.Entities.DataPoint", "DataPoint")
                        .WithMany()
                        .HasForeignKey("FK_Datapoint")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataPoint");
                });
#pragma warning restore 612, 618
        }
    }
}
