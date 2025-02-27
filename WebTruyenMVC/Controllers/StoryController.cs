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
            var response = await csModel.GetAllCasesAsync(request);

            return Ok(response);
        }
    }
}
