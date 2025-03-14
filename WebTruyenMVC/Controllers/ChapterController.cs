using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

namespace WebTruyenMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<ChapterController> logger;

        public ChapterController(MongoContext mongoContext, ILogger<ChapterController> logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new ChapterModel(mongoContext, logger);
            var response = await csModel.GetAllChapterAsync(request);

            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var csModel = new ChapterModel(mongoContext, logger);
            var response = await csModel.GetChapterByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ChapterEntity newChapter)
        {
            var csModel = new ChapterModel(mongoContext, logger);
            var response = await csModel.CreateChapterAsync(newChapter);
            return Ok(response);
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ChapterEntity updateChapter)
        {
            updateChapter.Id = id;
            var csModel = new ChapterModel(mongoContext, logger);
            var response = await csModel.UpdateChapterAsync(updateChapter);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var csModel = new ChapterModel(mongoContext, logger);
            var response = await csModel.DeleteChapterAsync(id);
            return Ok(response);
        }

        [HttpPost("Read")]
        public async Task<IActionResult> ReadChapter([FromBody] ReadChapterRequest request)
        {
            var csModel = new ChapterModel(mongoContext, logger);
            await csModel.ReadChapter(request.UserId, request.StoryId, request.ChapterNumber);
            return Ok(new { message = "Reading history updated successfully!" });
        }
    }
}
