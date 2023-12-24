using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBook.Application.Interfaces;
using MyBook.Domain.Dto;

namespace MyBookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _publisherService;
        public PublisherController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpPost("add-publisher")]
        public async Task<IActionResult> AddPublisher([FromBody] PublisherAddDto publisherAddDto)
        {
            var addPublisher = await _publisherService.AddPublisherAsync(publisherAddDto);
            if (addPublisher == null)
            {
                return BadRequest("Publisher was not add");
            }
            return Ok("Publisher Added success");
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public async Task<IActionResult> GetPublisherById(int id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null)
            {
                return BadRequest("Publisher with the given Id notFound");
            }
            return Ok(publisher);
        }

        [HttpGet("get-all-publisher")]
        public async Task<IActionResult> GetAllPublisher()
        {
            var publisher = await _publisherService.GetAllPublisher();
            if (publisher == null)
            {
                return BadRequest("Not found");
            }
            return Ok(publisher);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public async Task<IActionResult> DeletePublisherById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please check your input");
            }
            var publisher = await _publisherService.DeletePublisher(id);
            if (publisher == null)
            {
                return BadRequest("Publisher with the given Id not found");
            }
            return Ok(publisher);
        }

        [HttpPut("update-publisher-by-id/{id}")]
        public async Task<IActionResult> UpdatePublisherById(int id, [FromBody] PublisherAddDto publisherAddDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please check your input");
            }
            var publisher = await _publisherService.UpdatePublisher(id, publisherAddDto);
            if (publisher == null)
            {
                return BadRequest("Publisher with the given Id not found or Something went wrong");
            }
            return Ok(publisher);
        }

        [HttpGet("get-publisher-books-with-authors/{id}")]
        public async Task<IActionResult> GetPublisherDetails(int id)
        {
            var publisher = await _publisherService.GetPublisherDetails(id);
            if(publisher == null)
                return NotFound("Publisher Deta with the given Id was not found");
            return Ok(publisher);
        }
    }
}
