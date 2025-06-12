using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

namespace WebTruyenMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoryController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<StoryController> logger;

        public StoryController(MongoContext mongoContext, ILogger<StoryController> logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new StoryModel(mongoContext, logger);
            var response = await csModel.GetAllStoryAsync(request);

            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var csModel = new StoryModel(mongoContext, logger);
            var response = await csModel.GetStoryByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] StoryEntity newStory)
        {
            var csModel = new StoryModel(mongoContext, logger);
            var response = await csModel.CreateStoryAsync(newStory);
            return Ok(response);
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] StoryEntity updateStory)
        {
            updateStory.Id = id;
            var csModel = new StoryModel(mongoContext, logger);
            var response = await csModel.UpdateStoryAsync(updateStory);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var csModel = new StoryModel(mongoContext, logger);
            var response = await csModel.DeleteStoryAsync(id);
            return Ok(response);
        }

        [HttpPost("TopRated")] //Truyện TOP đề cử
        public async Task<IActionResult> GetTopRatedStories()
        {
            var csModel = new StoryModel(mongoContext, logger);
            var response = await csModel.GetTopRatedStoriesAsync();

            return Ok(response);
        }

    }
}
