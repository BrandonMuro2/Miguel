using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GGS.Service;
namespace GGS.Engine;

public partial class NewContext : DbContext
{
    public NewContext(DbContextOptions<NewContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Canale> Canales { get; set; }

    public virtual DbSet<MuestrasDeSeñale> MuestrasDeSeñales { get; set; }

    public virtual DbSet<Sesione> Sesiones { get; set; }

    public virtual DbSet<Sujeto> Sujetos { get; set; }

    public virtual DbSet<ChannelActivity> ChannelActivities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChannelActivity>(entity =>
        {
            entity.HasNoKey();

        });

        modelBuilder.Entity<Canale>(entity =>
        {
            entity.HasKey(e => e.ChannelId).HasName("PK__Canales__38C3E8F404962D64");

            entity.Property(e => e.ChannelId).HasColumnName("ChannelID");
            entity.Property(e => e.ChannelName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MuestrasDeSeñale>(entity =>
        {
            entity.HasKey(e => e.SampleId).HasName("PK__Muestras__8B99EC0A99D6E0DE");

            entity.Property(e => e.SampleId).HasColumnName("SampleID");
            entity.Property(e => e.ChannelId).HasColumnName("ChannelID");
            entity.Property(e => e.SessionId).HasColumnName("SessionID");

            entity.HasOne(d => d.Channel).WithMany(p => p.MuestrasDeSeñales)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("FK__MuestrasD__Chann__5CD6CB2B");

            entity.HasOne(d => d.Session).WithMany(p => p.MuestrasDeSeñales)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("FK__MuestrasD__Sessi__5DCAEF64");
        });

        modelBuilder.Entity<Sesione>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__Sesiones__C9F49270B7CD8CFC");

            entity.Property(e => e.SessionId).HasColumnName("SessionID");
            entity.Property(e => e.RecordingDate).HasColumnType("datetime");
            entity.Property(e => e.SessionType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.Subject).WithMany(p => p.Sesiones)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK__Sesiones__Subjec__5629CD9C");

            entity.HasMany(d => d.Channels).WithMany(p => p.Sessions)
                .UsingEntity<Dictionary<string, object>>(
                    "SessionCanale",
                    r => r.HasOne<Canale>().WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SessionCa__Chann__59FA5E80"),
                    l => l.HasOne<Sesione>().WithMany()
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__SessionCa__Sessi__59063A47"),
                    j =>
                    {
                        j.HasKey("SessionId", "ChannelId").HasName("PK__SessionC__9A78ACFF6B6C90A6");
                        j.ToTable("SessionCanales");
                        j.IndexerProperty<int>("SessionId").HasColumnName("SessionID");
                        j.IndexerProperty<int>("ChannelId").HasColumnName("ChannelID");
                    });
        });

        modelBuilder.Entity<Sujeto>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__sujetos__AC1BA388EAD6AAB7");

            entity.ToTable("sujetos");

            entity.Property(e => e.SubjectId)
                .ValueGeneratedNever()
                .HasColumnName("SubjectID");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
