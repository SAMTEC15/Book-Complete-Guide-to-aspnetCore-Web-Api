using Microsoft.EntityFrameworkCore;
using MyBook.Domain.Dto;
using MyBook.Domain.Models;
using MyBook.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyBook.Persistence.Repositories
{
    public class BooksRepository : IBooksRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public BooksRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Book> AddBook(BookAddDto books)
        {
            if (books == null)
            {
                return null;
            }
            var book = new Book
            {
                IsRead = books.IsRead,
                //Author = books.Author,
                CoverUrl = books.CoverUrl,
                DateRead = books.IsRead ? books.DateRead.Value : null,
                Description = books.Description,
                Genre = books.Genre,
                Rate = books.IsRead ? books.Rate.Value : null,
                Title = books.Title,
                PublisherId = books.PublisherId,
            };

            await _applicationDbContext.Books.AddAsync(book);
            await _applicationDbContext.SaveChangesAsync();
            foreach (var id in books.AuthorIds)
            {
                var bookAuthor = new BookAuthor
                {
                    BookId = book.Id,
                    AuthorId = id
                };
                await _applicationDbContext.BookAuthors.AddAsync(bookAuthor);
                await _applicationDbContext.SaveChangesAsync();
            }
            return book;
        }
        public async Task<Book> DeleteById(int? id)
        {
            var existingBook = await _applicationDbContext.Books.FirstOrDefaultAsync(u => u.Id == id);
            if (existingBook == null)
            {
                return null;
            }
            _applicationDbContext.Books.Remove(existingBook);
            await _applicationDbContext.SaveChangesAsync();
            return existingBook;
        }
        public async Task<IEnumerable<Book>> GetAllBooks() => await _applicationDbContext.Books.ToListAsync();
        public async Task<List<Book>> GetAllBooksAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null,
            bool isAscending = true, int pageNumber = 1, int pageSize = 10)
        {
            var books = _applicationDbContext.Books.Include("Publisher").AsQueryable();


            if(string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if(filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    books = books.Where(u => u.Title.Contains(filterQuery));
                }
            }

            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    books = isAscending ? books.OrderBy(u => u.Title) : books.OrderByDescending(u => u.Title);
                }
                else if(sortBy.Equals("Rate", StringComparison.OrdinalIgnoreCase))
                {
                    books = isAscending ? books.OrderBy(u => u.Rate) : books.OrderByDescending(u => u.Rate);
                }
                else if (sortBy.Equals("IsRead", StringComparison.OrdinalIgnoreCase))
                {
                    books = isAscending ? books.OrderBy(u => u.IsRead) : books.OrderByDescending(u => u.IsRead);
                }               
            }
            var skipResults = (pageNumber - 1) * pageSize;
            return await books.Skip(skipResults).Take(pageSize).ToListAsync();
        }
/*        public async Task<List<Book>> GetAllBooksAsync(
    string? filterOn = null, string? filterQuery = null, string? sortBy = null,
    bool isAscending = true, int pageNumber = 1, int pageSize = 10)
        {
            var query = _applicationDbContext.Books.AsQueryable();

            // Include Author and Publisher related entities in the query
            query = query.Include(book => book.Author)
                         .Include(book => book.Publisher);

            // Apply filtering based on filterOn and filterQuery
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(book => book.Title.Contains(filterQuery));
                }
                // Add more filtering conditions for other properties if needed
            }

            // Apply sorting based on sortBy
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLowerInvariant() switch
                {
                    "title" => isAscending ? query.OrderBy(book => book.Title) : query.OrderByDescending(book => book.Title),
                    "rate" => isAscending ? query.OrderBy(book => book.Rate) : query.OrderByDescending(book => book.Rate),
                    "isread" => isAscending ? query.OrderBy(book => book.IsRead) : query.OrderByDescending(book => book.IsRead),
                    _ => query // Default behavior if sortBy doesn't match known properties
                };
            }

            // Perform pagination and retrieve the result
            var skipResults = (pageNumber - 1) * pageSize;
            return await query.Skip(skipResults).Take(pageSize).ToListAsync();
        }*/
        public async Task<BookAuthorsReturnDto> GetById(int? id)
        {
            var books = await _applicationDbContext.Books.Where(u => u.Id == id).Select(book => new BookAuthorsReturnDto
            {
                Title = book.Title,
                Description = book.Description,
                IsRead = book.IsRead,
                DateRead = book.DateRead,
                Rate = book.Rate,
                Genre = book.Genre,
                CoverUrl = book.CoverUrl,
                PublisherName = book.Publisher.Name,
                AuthorName = book.BookAuthors.Select( u => u.Author.FirstName + " " +  u.Author.LastName).ToList()
            }).FirstOrDefaultAsync();
            return books;
        }
        public async Task<BookAddDto> UpdateBook(int id, BookAddDto book)
        {
            var bookCheck = await _applicationDbContext.Books.FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (book == null || bookCheck == null)
            {
                return null;
            }

            bookCheck.IsRead = book.IsRead;
            //bookCheck.Author = book.Author;
            bookCheck.CoverUrl = book.CoverUrl;
            bookCheck.DateRead = book.IsRead ? book.DateRead.Value : null;
            bookCheck.Description = book.Description;
            bookCheck.Genre = book.Genre;
            bookCheck.Rate = book.IsRead ? book.Rate.Value : null;
            bookCheck.Title = book.Title;


            await _applicationDbContext.SaveChangesAsync();
            return book;
        }

        public async Task<IEnumerable<Book>> Search(string title, string coverUrl)
        {
            IQueryable<Book> query =  _applicationDbContext.Books;
            if(string.IsNullOrEmpty(title))
            {
                return null;
            }
            query = query.Where(u  => u.Title.Contains(title));
            if(coverUrl == null)
            {
                return null;
            }
            //query = query.Where(u => u.Genre.Contains(coverUrl));   
            query = query.Where(u => u.CoverUrl == coverUrl);
            return await query.ToListAsync();
        }
    }
}
