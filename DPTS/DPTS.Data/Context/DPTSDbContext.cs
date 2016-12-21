﻿using DPTS.Data.IdentityEntities;
using DPTS.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DPTS.Data.Context
{
    public class DPTSDbContext : IdentityDbContext<User>
    {
        public DPTSDbContext() : base("DPTSDbContext")
        {
        }
        public virtual DbSet<Doctor> Doctor { get; set; }

        public virtual DbSet<Address> Address { get; set; }

        public virtual DbSet<Country> Country { get; set; }

        public virtual DbSet<StateProvince> StateProvince { get; set; }

        public virtual DbSet<Speciality> Speciality { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
