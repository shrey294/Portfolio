using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Portfolio.Models;

namespace Portfolio.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AboutMe> AboutMes { get; set; }

    public virtual DbSet<ContactDetail> ContactDetails { get; set; }

    public virtual DbSet<EducationMst> EducationMsts { get; set; }

    public virtual DbSet<Enquiry> Enquiries { get; set; }

    public virtual DbSet<ExperienceMst> ExperienceMsts { get; set; }

    public virtual DbSet<HeaderInformation> HeaderInformations { get; set; }

    public virtual DbSet<MySkill> MySkills { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<SkillMst> SkillMsts { get; set; }

    public virtual DbSet<UserMst> UserMsts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
