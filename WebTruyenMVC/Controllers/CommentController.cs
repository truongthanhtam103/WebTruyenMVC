using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

namespace WebTruyenMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<CommentController> logger;

        public CommentController(MongoContext mongoContext, ILogger<CommentController> logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new CommentModel(mongoContext, logger);
            var response = await csModel.GetAllCommentAsync(request);

            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var csModel = new CommentModel(mongoContext, logger);
            var response = await csModel.GetCommentByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CommentEntity newComment)
        {
            var csModel = new CommentModel(mongoContext, logger);
            var response = await csModel.CreateCommentAsync(newComment);
            return Ok(response);
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CommentEntity updateComment)
        {
            updateComment.Id = id;
            var csModel = new CommentModel(mongoContext, logger);
            var response = await csModel.UpdateCommentAsync(updateComment);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var csModel = new CommentModel(mongoContext, logger);
            var response = await csModel.DeleteCommentAsync(id);
            return Ok(response);
        }
    }
}
