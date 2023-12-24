using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBook.Application.Implementations;
using MyBook.Application.Interfaces;
using MyBook.Domain.Dto;
using MyBook.Domain.Models;

namespace MyBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookAddDto bookAddDto)
        {
            if (bookAddDto == null)
            {
                return BadRequest("InPut not in correct state");
            }
            await _bookService.AddBookAsync(bookAddDto);
            return Ok(bookAddDto);
        }

        [HttpGet("get/all/books")]
        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _bookService.GetAllBooks();
        }

        [HttpGet("get/book/byId/{id}")]
        public  async Task<IActionResult> GetBook(int id)
        {
           var getBook = await _bookService.GetBookByIdAsync(id);
            if(getBook == null)
            {
                return NotFound("Book not found");
            }
            return Ok(getBook);
        }

        [HttpPut("update-book-by-id/{id}")]
        public  async Task<IActionResult> UpdateBook(int id, [FromBody] BookAddDto bookAddDto)
        {
            var updateBook = await _bookService.UpdateBook(id, bookAddDto);
            if(updateBook == null)
            {
                return BadRequest("Book not found.");
            }
            return Ok(updateBook);
        }

        [HttpDelete("delete-book-by-id/{id}")]
        public async Task<IActionResult> Delete(int? id)
        { 
            var book = await _bookService.DeleteBook(id);
            if(book == null)
            {
                return NotFound("Book not found.");
            }
           
            return Ok(book);
        }
        [HttpGet("get/all/books/by/sorting/filter")]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery,
         [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber = 1, int pageSize = 10)
        {
            if (ModelState.IsValid)
            {
                var bookDomainModel = await _bookService.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);
                //Map Domain Model to Dto
                return Ok(bookDomainModel);
            }
            return BadRequest(ModelState);
        }
    }
}
