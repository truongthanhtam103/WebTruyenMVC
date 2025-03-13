﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("ListAll")]
        public async Task<IActionResult> GetAll([FromBody] FilterEntity request)
        {
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.GetAllAuthorAsync(request);

            return Ok(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.GetAuthorByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] AuthorEntity newStory)
        {
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.CreateAuthorAsync(newStory);
            return Ok(response);
        }


        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] AuthorEntity updateStory)
        {
            updateStory.Id = id;
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.UpdateAuthorAsync(updateStory);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var csModel = new AuthorModel(mongoContext, logger);
            var response = await csModel.DeleteAuthorAsync(id);
            return Ok(response);
        }
    }
}
