using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyBook.Application.Interfaces;
using MyBook.Domain.Dto;
using MyBook.Persistence.Repositories;

namespace MyBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorSerice _authorSerice;
        public AuthorController(IAuthorSerice authorSerice)
        {
            _authorSerice = authorSerice;
        }

        [HttpPost("add-author")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorAddDto authorAddDto)
        {            
          var addAuthor =  await _authorSerice.AddAuthorAsync(authorAddDto);
            if(addAuthor == null)
            {
                return BadRequest("Author was not add");
            }
            return  Ok("Author Added success");
        }

        [HttpGet("get-book-by-id/{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _authorSerice.GetAuthorByIdAsync(id);
            if(author == null)
            {
                return BadRequest("Book with the given Id notFound or Something went wrong");
            }
            return Ok(author);
        }

        [HttpGet("get-all-books")]
        public async Task<IActionResult> GetAllAuthor()
        {
            var authors = await _authorSerice.GetAllAuthor();
            if(authors == null)
            {
                return BadRequest("Not found");
            }
            return Ok(authors);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAuthorById(int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Please check your input");
            }
           var author = await _authorSerice.DeleteAuthor(id);
            if (author == null)
            {
                return BadRequest("Book with the given Id not found or Something went wrong");
            }
            return Ok(author);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAuthorById(int id, [FromBody] AuthorAddDto authorAddDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please check your input");
            }
            var author = await _authorSerice.UpdateAuthor(id, authorAddDto);
            if (author == null)
            {
                return BadRequest("Book with the given Id not found or Something went wrong");
            }
            return Ok(author);
        }

        [HttpGet("get-author-with-books-by-id/{id}")]
        public async Task<IActionResult> GetBookWithAuthor(int id)
        {
            var authorBooks = await _authorSerice.GetBookWithAuthor(id);
            if(authorBooks == null)
                return NotFound("Book with the given Id not found");
            return Ok(authorBooks);
        }


        
    }
}
