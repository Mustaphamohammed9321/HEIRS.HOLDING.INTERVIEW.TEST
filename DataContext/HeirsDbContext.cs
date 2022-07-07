using HEIRS.HOLDING.INTERVIEW.TEST.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HEIRS.HOLDING.INTERVIEW.TEST.DataContext
{
    public class HeirsDbContext : DbContext
    {
        public HeirsDbContext() : base("name:HeirsConnectionString") { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Person> People { get; set; }

    }
}