using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

namespace WebTruyenMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadingHistoryController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<ReadingHistoryController> logger;

        public ReadingHistoryController(MongoContext mongoContext, ILogger<ReadingHistoryController> logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new ReadingHistoryModel(mongoContext, logger);
            var response = await csModel.GetAllReadingHistoryAsync(request);

            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var csModel = new ReadingHistoryModel(mongoContext, logger);
            var response = await csModel.GetReadingHistoryByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ReadingHistoryEntity newReadingHistory)
        {
            var csModel = new ReadingHistoryModel(mongoContext, logger);
            var response = await csModel.CreateReadingHistoryAsync(newReadingHistory);
            return Ok(response);
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ReadingHistoryEntity updateReadingHistory)
        {
            updateReadingHistory.Id = id;
            var csModel = new ReadingHistoryModel(mongoContext, logger);
            var response = await csModel.UpdateReadingHistoryAsync(updateReadingHistory);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var csModel = new ReadingHistoryModel(mongoContext, logger);
            var response = await csModel.DeleteReadingHistoryAsync(id);
            return Ok(response);
        }
    }
}
