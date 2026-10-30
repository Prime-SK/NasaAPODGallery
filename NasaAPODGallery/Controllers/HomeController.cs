using Microsoft.AspNetCore.Mvc;
using NasaAPODGallery.Models;
using System.Diagnostics;
using NasaAPODGallery.Services;
using System.Text.Json;
using NasaAPODGallery.Repositories;

namespace NasaAPODGallery.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index( [FromServices] ApodRepository repository)
        {
            var entries = await repository.GetAllAsync();

            return View(entries);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> TestNasa( [FromServices] NasaApodService nasaService)
        {
            var entries = await nasaService.GetApodsAsync(
                new DateOnly(2026, 10, 20),
                new DateOnly(2026, 10, 22));

            return Content(
                JsonSerializer.Serialize(entries),
                "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> TestDatabase([FromServices] ApodRepository repository)
        {
            var entries = await repository.GetAllAsync();

            return Json(entries);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Fetch(DateOnly? startDate, DateOnly? endDate, 
            [FromServices] NasaApodService nasaService, [FromServices] ApodRepository repository)
        {
            if (!ModelState.IsValid ||
                startDate is null ||
                endDate is null ||
                startDate > endDate)
            {
                TempData["Message"] = "Please select a valid date range.";
                return RedirectToAction(nameof(Index));
            }

            if (endDate.Value.DayNumber - startDate.Value.DayNumber > 30)
            {
                TempData["Message"] = "Please select at most 31 days.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var apods = await nasaService.GetApodsAsync(
                    startDate.Value, endDate.Value);

                int savedCount = 0;

                foreach (var dto in apods)
                {
                    var entry = new ApodEntry
                    {
                        Date = dto.Date,
                        Title = dto.Title,
                        Explanation = dto.Explanation,
                        Url = dto.Url,
                        HdUrl = dto.HdUrl,
                        MediaType = dto.MediaType,
                        ServiceVersion = dto.ServiceVersion,
                        PostId = dto.PostId,
                        Permalink = dto.Permalink,
                        Credit = dto.Credit,
                        Copyright = dto.Copyright,
                        Alt = dto.Alt
                    };

                    if (await repository.SaveAsync(entry))
                    {
                        savedCount++;
                    }
                }

                TempData["Message"] = $"Saved {savedCount} new records.";
            }
            catch (HttpRequestException)
            {
                TempData["Message"] =
                    "Could not fetch NASA data. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
