﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Awful.Database.Context;

namespace Awful.Migrations
{
    [DbContext(typeof(UserAuthContext))]
    [Migration("20170119144611_DefaultUser")]
    partial class DefaultUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("Awful.Models.Users.UserAuth", b =>
                {
                    b.Property<int>("UserAuthId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AvatarLink");

                    b.Property<string>("CookiePath");

                    b.Property<bool>("IsDefaultUser");

                    b.Property<string>("UserName");

                    b.HasKey("UserAuthId");

                    b.ToTable("Users");
                });
        }
    }
}