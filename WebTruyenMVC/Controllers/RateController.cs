using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;

namespace WebTruyenMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateController : Controller
    {
        private readonly MongoContext mongoContext;
        private readonly ILogger<RateController> logger;

        public RateController(MongoContext mongoContext, ILogger<RateController> logger)
        {
            this.mongoContext = mongoContext;
            this.logger = logger;
        }

        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new RateModel(mongoContext, logger);
            var response = await csModel.GetAllRateAsync(request);

            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var csModel = new RateModel(mongoContext, logger);
            var response = await csModel.GetRateByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] RateEntity newRate)
        {
            var csModel = new RateModel(mongoContext, logger);
            var response = await csModel.CreateRateAsync(newRate);
            return Ok(response);
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RateEntity updateRate)
        {
            updateRate.Id = id;
            var csModel = new RateModel(mongoContext, logger);
            var response = await csModel.UpdateRateAsync(updateRate);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var csModel = new RateModel(mongoContext, logger);
            var response = await csModel.DeleteRateAsync(id);
            return Ok(response);
        }
    }
}
