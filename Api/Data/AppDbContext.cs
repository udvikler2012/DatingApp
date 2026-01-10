using Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Api.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
        .HasData(
            new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "544246ce-8298-4365-8c26-1a16df5e61ee" },
            new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = "e43536ef-48cc-40da-86bb-cde947c2bb5c" },
            new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "b6a39d70-fcee-455a-807d-362bb13e945e" }
        );

        modelBuilder.Entity<Message>()
        .HasOne(x => x.Recipient)
        .WithMany(modelBuilder => modelBuilder.MessagesReceived)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
       .HasOne(x => x.Sender)
       .WithMany(modelBuilder => modelBuilder.MessagesSent)
       .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MemberLike>()
        .HasKey(x => new { x.SourceMemberId, x.TargetMemberId });

        modelBuilder.Entity<MemberLike>()
        .HasOne(s => s.SourceMember)
        .WithMany(t => t.LikedMembers)
        .HasForeignKey(s => s.SourceMemberId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberLike>()
       .HasOne(s => s.TargetMember)
       .WithMany(t => t.LikedByMembers)
       .HasForeignKey(s => s.TargetMemberId)
       .OnDelete(DeleteBehavior.NoAction);

        // Convert SQLite UTC Date to DateTime with timezone specifier 'Z'
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );
        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
        );
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}
