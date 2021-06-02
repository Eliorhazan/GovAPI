﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GovAPI
{
    public class Context : DbContext
    {
      

        public  DbSet<MOT4Wheels> MOT4Wheels { get; set; }
        public  DbSet<MOTModels> MOTModels { get; set; }
        public  DbSet<MOTRecall> MOTRecall { get; set; }
        public  DbSet<MOTTags> MOTTags { get; set; }

        public DbSet<CarHoldingHistory> CarHoldingHistory { get; set; }
        public DbSet<CarDealers> CarDealers { get; set; }

        public DbSet<Logs> Logs { get; set; }


        

        public Context() : base("BalcarDB") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}