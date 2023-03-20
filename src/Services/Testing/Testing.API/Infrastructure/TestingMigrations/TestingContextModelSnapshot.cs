﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Testing.Infrastructure.Persistence;

#nullable disable

namespace Testing.API.Infrastructure.TestingMigrations
{
    [DbContext(typeof(TestingContext))]
    partial class TestingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Testing.Core.Domain.AggregatesModel.SolutionAggregate.Solution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Success")
                        .HasColumnType("boolean");

                    b.Property<Guid>("_taskId")
                        .HasColumnType("uuid")
                        .HasColumnName("TaskId");

                    b.Property<Guid>("_userId")
                        .HasColumnType("uuid")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("_taskId");

                    b.HasIndex("_userId");

                    b.ToTable("Solutions");
                });

            modelBuilder.Entity("Testing.Core.Domain.AggregatesModel.TaskAggregate.Task", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Testing.Core.Domain.AggregatesModel.UserAggregate.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Testing.Core.Domain.AggregatesModel.SolutionAggregate.Solution", b =>
                {
                    b.HasOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.Task", null)
                        .WithMany()
                        .HasForeignKey("_taskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Testing.Core.Domain.AggregatesModel.UserAggregate.User", null)
                        .WithMany()
                        .HasForeignKey("_userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.SolutionAggregate.SolutionValue", "Value", b1 =>
                        {
                            b1.Property<Guid>("SolutionId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("SolutionId");

                            b1.ToTable("Solutions");

                            b1.WithOwner()
                                .HasForeignKey("SolutionId");
                        });

                    b.Navigation("Value")
                        .IsRequired();
                });

            modelBuilder.Entity("Testing.Core.Domain.AggregatesModel.TaskAggregate.Task", b =>
                {
                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.Description", "Description", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Examples")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Note")
                                .HasColumnType("text");

                            b1.Property<string>("SomeCases")
                                .HasColumnType("text");

                            b1.Property<string>("Text")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.Difficulty", "Difficulty", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .HasColumnType("integer");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.ExecutionCondition", "ExecutionCondition", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<string>("ExecutionTemplate")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<TimeSpan>("TimeLimit")
                                .HasColumnType("interval");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.ProgrammingLanguage", "ProgrammingLanguage", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .HasColumnType("integer");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.SolutionTemplate", "SolutionTemplate", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.Title", "Title", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.Type", "Type", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .HasColumnType("integer");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("TaskId");

                            b1.ToTable("Tasks");

                            b1.WithOwner()
                                .HasForeignKey("TaskId");
                        });

                    b.Navigation("Description")
                        .IsRequired();

                    b.Navigation("Difficulty")
                        .IsRequired();

                    b.Navigation("ExecutionCondition")
                        .IsRequired();

                    b.Navigation("ProgrammingLanguage")
                        .IsRequired();

                    b.Navigation("SolutionTemplate")
                        .IsRequired();

                    b.Navigation("Title")
                        .IsRequired();

                    b.Navigation("Type")
                        .IsRequired();
                });

            modelBuilder.Entity("Testing.Core.Domain.AggregatesModel.UserAggregate.User", b =>
                {
                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.UserAggregate.UserProfile", "Profile", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Firstname")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Lastname")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Profile")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
