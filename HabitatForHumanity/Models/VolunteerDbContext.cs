﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.Models
{
    public class VolunteerDbContext : DbContext
    {
        public DbSet<Organization> organizations { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<TimeSheet> timeSheets { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<ProjectCategory> projectCategories { get; set; }
        public DbSet<WaiverHistory> waiverHistory { get; set; }
        public DbSet<HfhEvent> hfhEvents { get; set; }
        public DbSet<ProjectEvent> eventProjects { get; set; }
    }
}