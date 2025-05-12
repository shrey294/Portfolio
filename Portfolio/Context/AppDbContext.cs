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

    public virtual DbSet<EducationMst> EducationMsts { get; set; }

    public virtual DbSet<ExperienceMst> ExperienceMsts { get; set; }

    public virtual DbSet<HeaderInformation> HeaderInformations { get; set; }

    public virtual DbSet<SkillMst> SkillMsts { get; set; }

    public virtual DbSet<UserMst> UserMsts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-L48UG33\\SQLEXPRESS;Database=Portfolio_web;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
