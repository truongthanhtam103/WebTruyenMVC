using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

namespace WebTruyenMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<AuthorController> logger;

        public AuthorController(MongoContext mongoContext, ILogger<AuthorController> logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        /// <summary>
        /// Lấy danh sách tất cả tác giả
        /// </summary>
        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.GetAllAuthorAsync(request);

            return Ok(response);
        }

        /// <summary>
        /// Lấy thông tin tác giả theo id
        /// </summary>
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.GetAuthorByIdAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Thêm mới tác giả
        /// </summary>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AuthorEntity newAuthor)
        {
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.CreateAuthorAsync(newAuthor);
            return Ok(response);
        }

        /// <summary>
        /// Cập nhật thông tin tác giả
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AuthorEntity updateAuthor)
        {
            updateAuthor.Id = id;
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.UpdateAuthorAsync(updateAuthor);
            return Ok(response);
        }

        /// <summary>
        /// Xóa tác giả
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.DeleteAuthorAsync(id);
            return Ok(response);
        }
    }
}
