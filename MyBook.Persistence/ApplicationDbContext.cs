using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBook.Domain.Models;
using MyBook.Persistence.Seeder;

namespace MyBook.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Image> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<BookAuthor>()
                .HasOne(u => u.Book)
                .WithMany(u => u.BookAuthors)
                .HasForeignKey(bi => bi.BookId);

            modelBuilder.Entity<BookAuthor>()
                .HasOne(u => u.Author)
                .WithMany(u => u.BookAuthors)
                .HasForeignKey(bi => bi.AuthorId);

            modelBuilder.Entity<Book>()
           .HasOne(b => b.Publisher) // Configure the relationship for Book.Publisher
           .WithMany() // Assuming a Publisher can have many Books
           .HasForeignKey(b => b.PublisherId);

        //    modelBuilder.Entity<Book>()
        //.HasOne(b => b.Author)  // Assuming Book has a single Author navigation property
        //.WithMany();

            base.OnModelCreating(modelBuilder);

            // Call the SeedData method from the DataSeeder class
            //AddDbInitializer.Seed(modelBuilder);
        }
    }
}
