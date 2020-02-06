﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext (DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

        //The highlighted code creates a DbSet<TEntity> property for each entity set. In EF Core terminology:
        //An entity set typically corresponds to a database table.
        //An entity corresponds to a row in the table.

        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        //public DbSet<Email> Email { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
            //modelBuilder.Entity<Email>().ToTable("Email");


        }

        public DbSet<ContosoUniversity.Models.Student> Student { get; set; }
    }
}
