﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyMauiTemplate.Data.Data_Context;

#nullable disable

namespace MyMauiTemplate.Data.Migrations
{
    [DbContext(typeof(MyMauiTemplateAppContext))]
    [Migration("20231208220714_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("MyMauiTemplate.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("BackupCodes")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsGuest")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfilePicturePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Require2Fa")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("StoreSavedPassword")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TrustedDevices")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TwoFactorSecret")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("VerificationCodeId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("VerificationCodeId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MyMauiTemplate.Models.VerificationCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ExpirationTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("VerificationCode");
                });

            modelBuilder.Entity("MyMauiTemplate.Models.User", b =>
                {
                    b.HasOne("MyMauiTemplate.Models.VerificationCode", "VerificationCode")
                        .WithMany()
                        .HasForeignKey("VerificationCodeId");

                    b.Navigation("VerificationCode");
                });
#pragma warning restore 612, 618
        }
    }
}