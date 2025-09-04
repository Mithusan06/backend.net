using Microsoft.EntityFrameworkCore;
using Poddle.CommunicationService.Entities;

namespace Poddle.CommunicationService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Message> Messages => Set<Message>();
    public DbSet<ConversationLog> ConversationLogs => Set<ConversationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.From).IsRequired().HasMaxLength(128);
            entity.Property(m => m.To).IsRequired().HasMaxLength(128);
            entity.Property(m => m.Content).IsRequired().HasMaxLength(4000);
            entity.Property(m => m.Direction).IsRequired().HasMaxLength(32);
            entity.Property(m => m.Status).IsRequired().HasMaxLength(32);
            entity.HasMany(m => m.ConversationLogs)
                  .WithOne(cl => cl.Message)
                  .HasForeignKey(cl => cl.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ConversationLog>(entity =>
        {
            entity.ToTable("ConversationLogs");
            entity.HasKey(cl => cl.Id);
            entity.Property(cl => cl.Role).IsRequired().HasMaxLength(32);
            entity.Property(cl => cl.Content).IsRequired().HasMaxLength(4000);
        });

        base.OnModelCreating(modelBuilder);
    }
}
