using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace HafizDemoAPI.ModelCDN;
//Scaffold-DbContext "Server=CNKZ463-DELL\SQLEXPRESS;Initial Catalog=dbdemo;User ID=demo;Password=P@ssw0rd;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer -OutputDir ModelCDN -Context "CDNContext" 
public partial class CDNContext : DbContext
{
    public CDNContext()
    {
    }

    public CDNContext(DbContextOptions<CDNContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Hobby> Hobbies { get; set; }


    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<CDNUserConn> CDNUserConn { get; set; }
    public virtual DbSet<Simpleuser> Simpleuser { get; set; }

    //"Data Source=CNKZ463-DELL\\SQLEXPRESS;Initial Catalog=dbdemo;User ID=demo;Password=P@ssw0rd;TrustServerCertificate=true"
    //"Data Source=tcp:shas.database.windows.net,1433;Initial Catalog=dbdemo;Persist Security Info=False;User ID=demo;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=true;Connection Timeout=30;"
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
       //=> optionsBuilder.UseSqlServer("Data Source=CNKZ463-DELL\\SQLEXPRESS;Initial Catalog=dbdemo;User ID=demo;Password=P@ssw0rd;TrustServerCertificate=true");
          => optionsBuilder.UseSqlServer("Data Source=tcp:shas.database.windows.net,1433;Initial Catalog=dbdemo;Persist Security Info=False;User ID=demo;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=true;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
