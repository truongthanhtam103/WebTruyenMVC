using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebTruyenMVC.Entity;
using WebTruyenMVC.Models;
using MongoDB.Driver;

namespace WebTruyenMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoContext _mongoContext;
        private readonly ILogger<HomeController> logger;
        private readonly IMongoCollection<StoryEntity> _stories;

        public HomeController(MongoContext mongoContext, ILogger<HomeController> logger)
        {
            _mongoContext = mongoContext;
            _stories = mongoContext.GetCollection<StoryEntity>("Stories");
            this.logger = logger;

        }

        public async Task<IActionResult> Index(string search)
        {
            var builder = Builders<StoryEntity>.Filter;
            var filter = string.IsNullOrEmpty(search)
                ? builder.Empty
                : builder.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(search, "i"));

            var allStories = await _stories.Find(filter).SortByDescending(x => x.Created).ToListAsync();
            var newestStories = allStories.Take(6).ToList();
            var recommendedStories = allStories.OrderByDescending(x => x.Views).Take(5).ToList();

            ViewBag.Recommended = recommendedStories;
            ViewBag.SearchKeyword = search;

            return View(newestStories);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult History()
        {
            return View();
        }

    }
}
