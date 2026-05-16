using Microsoft.EntityFrameworkCore;
using SmartTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartTracker
{
    public class HabitTrackerContext: DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<HabitTask> HabitTasks { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Initial Catalog=DbForTask;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HabitTask>().HasMany(t => t.Tags).WithMany(tg => tg.Tasks);
        }

    }
}
