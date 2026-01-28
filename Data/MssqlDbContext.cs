using aresu_txt_editor_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace aresu_txt_editor_backend.Data;

public class MssqlDbContext(DbContextOptions<MssqlDbContext> options) : DbContext(options)
{
    public DbSet<TextDocument> TextDocuments { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<TextDocument>()
            .HasIndex(doc => new {doc.Id, doc.UserId});

        modelBuilder.Entity<TextDocument>()
            .HasIndex(doc => new {doc.Title, doc.UserId})
            .IsUnique();

        modelBuilder.Entity<TextDocument>()
            .Property(doc => doc.Content)
            .HasDefaultValue("");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new UpdatedAtTimestampInterceptor());
    }
}