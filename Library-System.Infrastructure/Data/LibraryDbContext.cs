using Library_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library_System.Infrastructure.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }

    public DbSet<BookRequest> BookRequests { get; set; }

    public DbSet<BookRequestor> BookRequestors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookRequest>()
            .HasOne(br => br.Book)
            .WithMany(b => b.BookRequests)
            .HasForeignKey(br => br.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookRequest>()
            .HasOne(br => br.BookRequestor)
            .WithMany(r => r.BookRequests)
            .HasForeignKey(br => br.BookRequestorId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
