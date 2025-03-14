using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

namespace WebTruyenMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<CategoryController> logger;

        public CategoryController(MongoContext mongoContext, ILogger<CategoryController> logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new CategoryModel(mongoContext, logger);
            var response = await csModel.GetAllCategoryAsync(request);

            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var csModel = new CategoryModel(mongoContext, logger);
            var response = await csModel.GetCategoryByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CategoryEntity newCategory)
        {
            var csModel = new CategoryModel(mongoContext, logger);
            var response = await csModel.CreateCategoryAsync(newCategory);
            return Ok(response);
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CategoryEntity updateCategory)
        {
            updateCategory.Id = id;
            var csModel = new CategoryModel(mongoContext, logger);
            var response = await csModel.UpdateCategoryAsync(updateCategory);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var csModel = new CategoryModel(mongoContext, logger);
            var response = await csModel.DeleteCategoryAsync(id);
            return Ok(response);
        }
    }
}
