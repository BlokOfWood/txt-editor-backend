using aresu_txt_editor_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace aresu_txt_editor_backend.Data;

public class MssqlDbContext(DbContextOptions<MssqlDbContext> options) : DbContext(options)
{
    public DbSet<TextDocument> TextDocuments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<TextDocument>()
            .HasIndex(doc => new { doc.UserId, doc.Id });

        modelBuilder.Entity<TextDocument>()
            .HasIndex(doc => new { doc.UserId, doc.Title })
            .IsUnique();

        modelBuilder.Entity<TextDocument>()
            .Property(doc => doc.EncryptedContent);

        modelBuilder.Entity<TextDocument>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(doc => doc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        var userSessions = modelBuilder.Entity<UserSession>();

        userSessions
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .IsRequired();

        userSessions
            .HasOne<TextDocument>()
            .WithMany()
            .HasForeignKey(session => session.TextDocumentId);

        userSessions
            .HasIndex(session => new { session.TextDocumentId });

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new UpdatedAtTimestampInterceptor());
    }
}