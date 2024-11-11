﻿// <auto-generated />
using System;
using Cypherly.Authentication.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cypherly.Authentication.Persistence.Migrations
{
    [DbContext(typeof(AuthenticationDbContext))]
    [Migration("20241111145930_device_remove_nullability_columns")]
    partial class device_remove_nullability_columns
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "citext");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Cypherly.Authentication.Domain.Aggregates.Claim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Claim", (string)null);
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Aggregates.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("AppVersion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PublicKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Device", (string)null);
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.ToTable("RefreshToken", (string)null);
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.UserClaim", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ClaimId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "ClaimId");

                    b.HasIndex("ClaimId");

                    b.ToTable("UserClaim", (string)null);
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.VerificationCode", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("CodeType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("VerificationCode", (string)null);
                });

            modelBuilder.Entity("Cypherly.Persistence.Outbox.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<DateTime>("OccurredOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ProcessedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("OutboxMessage", (string)null);
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Aggregates.User", b =>
                {
                    b.OwnsOne("Cypherly.Authentication.Domain.ValueObjects.Email", "Email", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Address")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("citext")
                                .HasColumnName("Email");

                            b1.HasKey("UserId");

                            b1.HasIndex("Address")
                                .IsUnique();

                            b1.ToTable("User");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Cypherly.Authentication.Domain.ValueObjects.Password", "Password", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("HashedPassword")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("character varying(255)")
                                .HasColumnName("Password");

                            b1.HasKey("UserId");

                            b1.ToTable("User");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Email")
                        .IsRequired();

                    b.Navigation("Password")
                        .IsRequired();
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.Device", b =>
                {
                    b.HasOne("Cypherly.Authentication.Domain.Aggregates.User", "User")
                        .WithMany("Devices")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.RefreshToken", b =>
                {
                    b.HasOne("Cypherly.Authentication.Domain.Entities.Device", "Device")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.UserClaim", b =>
                {
                    b.HasOne("Cypherly.Authentication.Domain.Aggregates.Claim", "Claim")
                        .WithMany("UserClaims")
                        .HasForeignKey("ClaimId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cypherly.Authentication.Domain.Aggregates.User", "User")
                        .WithMany("UserClaims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Claim");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.VerificationCode", b =>
                {
                    b.HasOne("Cypherly.Authentication.Domain.Aggregates.User", null)
                        .WithMany("VerificationCodes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Aggregates.Claim", b =>
                {
                    b.Navigation("UserClaims");
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Aggregates.User", b =>
                {
                    b.Navigation("Devices");

                    b.Navigation("UserClaims");

                    b.Navigation("VerificationCodes");
                });

            modelBuilder.Entity("Cypherly.Authentication.Domain.Entities.Device", b =>
                {
                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
