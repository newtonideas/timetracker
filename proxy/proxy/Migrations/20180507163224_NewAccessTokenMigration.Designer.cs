﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using proxy.Data;
using System;

namespace proxy.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180507163224_NewAccessTokenMigration")]
    partial class NewAccessTokenMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("proxy.AuthServices.AccessToken", b =>
                {
                    b.Property<string>("Token")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FedAuth")
                        .IsRequired();

                    b.Property<string>("FedAuth1")
                        .IsRequired();

                    b.Property<string>("SessionId")
                        .IsRequired();

                    b.HasKey("Token");

                    b.ToTable("AccessTokens");
                });
#pragma warning restore 612, 618
        }
    }
}