using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GGS.Engine;

public partial class NewContext : DbContext
{
    public NewContext(DbContextOptions<NewContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Catalog> Catalogs { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<RequirementConsole> RequirementConsoles { get; set; }

    public virtual DbSet<RequirementPc> RequirementPcs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserGameRating> UserGameRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Catalog>(entity =>
        {
            entity.HasKey(e => e.CatId).HasName("PK__Catalog__6A1C8ADAB29BACBD");

            entity.ToTable("Catalog");

            entity.Property(e => e.CatId).HasColumnName("CatID");
            entity.Property(e => e.CatName).IsUnicode(false);

            entity.HasMany(d => d.Games).WithMany(p => p.Cats)
                .UsingEntity<Dictionary<string, object>>(
                    "GameCatalog",
                    r => r.HasOne<Game>().WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GameCatal__GameI__4AB81AF0"),
                    l => l.HasOne<Catalog>().WithMany()
                        .HasForeignKey("CatId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GameCatal__CatID__49C3F6B7"),
                    j =>
                    {
                        j.HasKey("CatId", "GameId").HasName("PK__GameCata__A8B703A7B15D18D5");
                        j.ToTable("GameCatalog");
                        j.IndexerProperty<int>("CatId").HasColumnName("CatID");
                        j.IndexerProperty<int>("GameId").HasColumnName("GameID");
                    });
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("PK__Game__2AB897DD82E9C502");

            entity.ToTable("Game");

            entity.Property(e => e.GameId).HasColumnName("GameID");
            entity.Property(e => e.GameDescription).IsUnicode(false);
            entity.Property(e => e.GameDev).IsUnicode(false);
            entity.Property(e => e.GameImg)
                .IsUnicode(false)
                .HasColumnName("GameIMG");
            entity.Property(e => e.GameName).IsUnicode(false);
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.ListId).HasName("PK__List__E3832865B918E1F0");

            entity.ToTable("List");

            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.ListDesc).IsUnicode(false);
            entity.Property(e => e.ListName).IsUnicode(false);
            entity.Property(e => e.UsId).HasColumnName("UsID");

            entity.HasOne(d => d.Us).WithMany(p => p.Lists)
                .HasForeignKey(d => d.UsId)
                .HasConstraintName("FK__List__UsID__3B75D760");

            entity.HasMany(d => d.Games).WithMany(p => p.Lists)
                .UsingEntity<Dictionary<string, object>>(
                    "GameList",
                    r => r.HasOne<Game>().WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GameList__GameID__46E78A0C"),
                    l => l.HasOne<List>().WithMany()
                        .HasForeignKey("ListId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__GameList__ListID__45F365D3"),
                    j =>
                    {
                        j.HasKey("ListId", "GameId").HasName("PK__GameList__2128A11824CCA34D");
                        j.ToTable("GameList");
                        j.IndexerProperty<int>("ListId").HasColumnName("ListID");
                        j.IndexerProperty<int>("GameId").HasColumnName("GameID");
                    });
        });

        modelBuilder.Entity<RequirementConsole>(entity =>
        {
            entity.HasKey(e => e.ConId).HasName("PK__Requirem__E19F47A938EB958F");

            entity.ToTable("RequirementConsole");

            entity.Property(e => e.ConId).HasColumnName("ConID");
            entity.Property(e => e.ConName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.GameId).HasColumnName("GameID");

            entity.HasOne(d => d.Game).WithMany(p => p.RequirementConsoles)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__Requireme__GameI__4316F928");
        });

        modelBuilder.Entity<RequirementPc>(entity =>
        {
            entity.HasKey(e => e.ReqId).HasName("PK__Requirem__28A9A3A22559C39F");

            entity.ToTable("RequirementPC");

            entity.Property(e => e.ReqId).HasColumnName("ReqID");
            entity.Property(e => e.GameId).HasColumnName("GameID");
            entity.Property(e => e.ReqCard)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ReqCpu)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ReqCPU");
            entity.Property(e => e.ReqType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Game).WithMany(p => p.RequirementPcs)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__Requireme__GameI__403A8C7D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsId).HasName("PK__User__BD21E37FAF0CBBDB");

            entity.ToTable("User");

            entity.Property(e => e.UsId).HasColumnName("UsID");
            entity.Property(e => e.UsName).IsUnicode(false);
        });

        modelBuilder.Entity<UserGameRating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__UserGame__FCCDF85C008684A9");

            entity.ToTable("UserGameRating");

            entity.Property(e => e.RatingId).HasColumnName("RatingID");
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.GameId).HasColumnName("GameID");
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Game).WithMany(p => p.UserGameRatings)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK__UserGameR__GameI__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.UserGameRatings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserGameR__UserI__5812160E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
