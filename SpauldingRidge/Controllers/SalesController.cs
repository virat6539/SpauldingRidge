using Microsoft.AspNetCore.Mvc;
using SpauldingRidge.Models;
using System.Linq;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Formats.Asn1;
using System.IO;
using CsvHelper;
using System.Globalization;
namespace SpauldingRidge.Controllers
{
    public class SalesController : Controller
    {
        private readonly SpauldingContext _context;

        public SalesController(SpauldingContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Chart(int year, double percentage)
        {
            ViewBag.Year = year;
            ViewBag.Percentage = percentage;
            return View();
        }

        [HttpPost]
        public IActionResult Index(int year, double percentage)
        {
            var salesData = _context.Orders
                .Where(o => o.OrderDate.Value.Year == year)
                .Join(_context.Products, o => o.OrderId, p => p.OrderId, (o, p) => new { o.State, p.Sales })
                .GroupBy(op => op.State)
                .Select(g => new SalesData
                {
                    State = g.Key,
                    Sales = g.Sum(op => op.Sales ?? 0),
                    IncrementedSales = g.Sum(op => op.Sales ?? 0) * (1 + (percentage / 100)),
                    PercentageIncrease = percentage
                })
                .ToList();

            ViewBag.TotalSales = salesData.Sum(s => s.Sales);
            ViewBag.TotalIncrementedSales = salesData.Sum(s => s.IncrementedSales);
            return View(salesData);
        }
        public IActionResult GetChartData(int year, double percentage)
        {
            var salesData = _context.Orders
                .Where(o => o.OrderDate.Value.Year == year)
                .Join(_context.Products, o => o.OrderId, p => p.OrderId, (o, p) => new { o.State, p.Sales })
                .GroupBy(op => op.State)
                .Select(g => new SalesData
                {
                    State = g.Key,
                    Sales = g.Sum(op => op.Sales ?? 0),
                    IncrementedSales = g.Sum(op => op.Sales ?? 0) * (1 + (percentage / 100)),
                    PercentageIncrease = percentage
                })
                .ToList();

            return Json(salesData);
        }
        public IActionResult GetAggregatedData(int year, double percentage)
        {
            var totalSales = _context.Orders
                .Where(o => o.OrderDate.Value.Year == year)
                .Join(_context.Products, o => o.OrderId, p => p.OrderId, (o, p) => p.Sales)
                .Sum(sales => sales ?? 0);

            // Calculate the total incremented sales correctly
            var totalIncrementedSales = totalSales * (1 + (percentage / 100));

            var aggregatedData = new
            {
                TotalSales = totalSales,
                TotalIncrementedSales = totalIncrementedSales
            };

            return Json(aggregatedData);
        }


        public IActionResult DownloadCsv(int year, double percentage)
        {
            var salesData = _context.Orders
                .Where(o => o.OrderDate.Value.Year == year)
                .Join(_context.Products, o => o.OrderId, p => p.OrderId, (o, p) => new { o.State, p.Sales })
                .GroupBy(op => op.State)
                .Select(g => new SalesData
                {
                    State = g.Key,
                    Sales = g.Sum(op => op.Sales ?? 0),
                    IncrementedSales = g.Sum(op => op.Sales ?? 0) * (1 + (percentage / 100)),
                    PercentageIncrease = percentage
                })
                .ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memoryStream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null,
                        MissingFieldFound = null
                    };
                    csv.Context.RegisterClassMap<SalesDataMap>();
                    csv.WriteRecords(salesData);
                    writer.Flush();
                    memoryStream.Position = 0;

                    // Use a new MemoryStream to return the file
                    var result = new FileStreamResult(new MemoryStream(memoryStream.ToArray()), "text/csv")
                    {
                        FileDownloadName = $"SalesForecast_{year}.csv"
                    };
                    return result;
                }
            }
        }
    }

    public class SalesDataMap : ClassMap<SalesData>
    {
        public SalesDataMap()
        {
            Map(m => m.State).Name("State");
            Map(m => m.PercentageIncrease).Name("Percentage Increase");
            Map(m => m.Sales).Name("Sales Value");
            Map(m => m.IncrementedSales).Name("Incremented Sales Value");
        }
    }
}
