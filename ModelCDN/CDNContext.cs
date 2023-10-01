using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;


namespace HafizDemoAPI.ModelCDN;
public partial class CDNContext : DbContext
{
    public CDNContext()
    {
    }

    private IConfiguration? _configuration;

    public CDNContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public CDNContext(DbContextOptions<CDNContext> options)
        : base(options)
    {
    }

    public virtual DbSet<StatTable> StatTables { get; set; }
    public virtual DbSet<Hobby> Hobbies { get; set; }


    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<CDNUserConn> CDNUserConn { get; set; }
    public virtual DbSet<Simpleuser> Simpleuser { get; set; }

    /// <summary>
    /// Read conn string from Environment Variable
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQLCONNSTR_CDNConn"));
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatTable>(entity =>
        {
            entity.ToTable("StatTable");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StatGroup).HasMaxLength(50);
            entity.Property(e => e.StatName).HasMaxLength(50);
        });

        modelBuilder.Entity<Hobby>(entity =>
        {
            entity.Property(e => e.HobbyId).HasColumnName("HobbyID");
            entity.Property(e => e.HobbyName)
                .HasMaxLength(50)
                .HasColumnName("HobbyName");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.Property(e => e.SkillName).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Mail).HasMaxLength(50);
            entity.Property(e => e.PhoneNo).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });


        modelBuilder.Entity<CDNUserConn>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.ConnectionId)
                .HasMaxLength(50)
                .HasColumnName("ConnectionID");

            entity.Property(e => e.DateConnect)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");


            entity.Property(e => e.UserID).HasColumnName("UserID");


        });

        modelBuilder.Entity<Simpleuser>(entity =>
        {

            entity.Property(e => e.Hobbies)
                .HasMaxLength(150)
                .HasColumnName("hobbies");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Mail)
                .HasMaxLength(50)
                .HasColumnName("mail");
            entity.Property(e => e.Phoneno)
                .HasMaxLength(20)
                .HasColumnName("phoneno");
            entity.Property(e => e.Skills)
                .HasMaxLength(150)
                .HasColumnName("skills");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
