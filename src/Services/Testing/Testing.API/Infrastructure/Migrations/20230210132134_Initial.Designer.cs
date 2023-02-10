﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Testing.Infrastructure.Persistence;

#nullable disable

namespace Testing.API.Infrastructure.Migrations
{
    [DbContext(typeof(TestingContext))]
    [Migration("20230210132134_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("_taskId")
                        .HasColumnType("uuid")
                        .HasColumnName("TaskId");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("Solutions", t =>
                        {
                            t.Property("TaskId")
                                .HasColumnName("TaskId1");
                        });
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

            modelBuilder.Entity("Testing.Core.Domain.AggregatesModel.SolutionAggregate.Solution", b =>
                {
                    b.HasOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.Task", null)
                        .WithMany()
                        .HasForeignKey("TaskId")
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

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.SolutionAggregate.User", "User", b1 =>
                        {
                            b1.Property<Guid>("SolutionId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Firstname")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<Guid>("Id")
                                .HasColumnType("uuid");

                            b1.Property<string>("Lastname")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("SolutionId");

                            b1.ToTable("Solutions");

                            b1.WithOwner()
                                .HasForeignKey("SolutionId");
                        });

                    b.Navigation("User")
                        .IsRequired();

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

                            b1.Property<string>("Tests")
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

                    b.OwnsOne("Testing.Core.Domain.AggregatesModel.TaskAggregate.SolutionExample", "SolutionExample", b1 =>
                        {
                            b1.Property<Guid>("TaskId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Description")
                                .HasColumnType("text");

                            b1.Property<string>("Solution")
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

                    b.Navigation("SolutionExample")
                        .IsRequired();

                    b.Navigation("Title")
                        .IsRequired();

                    b.Navigation("Type")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
