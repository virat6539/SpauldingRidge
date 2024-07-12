using Microsoft.AspNetCore.Mvc;
using SpauldingRidge.Models;
using System.Linq;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using SpauldingRidge.Data;
using Newtonsoft.Json;
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
            ViewBag.States = _context.Orders.Select(o => o.State).Distinct().ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Index(int year, double percentage)
        {
            return RedirectToAction("Result", new { year, percentage });
        }

        public IActionResult Result(int year, double percentage)
        {
            var returnedOrderIds = _context.OrdersReturns
                .Join(_context.Orders, r => r.OrderId, o => o.OrderId, (r, o) => new { r.OrderId, o.OrderDate })
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
                .Select(o => o.OrderId)
                .ToList();

            var returnsData = _context.Products
                .Where(p => returnedOrderIds.Contains(p.OrderId))
                .Join(_context.Orders, p => p.OrderId, o => o.OrderId, (p, o) => new { o.State, p.Sales })
                .GroupBy(po => po.State)
                .Select(g => new
                {
                    State = g.Key,
                    Returns = g.Sum(po => po.Sales ?? 0)
                })
                .ToList();

            var totalReturnsByState = returnsData.ToDictionary(r => r.State, r => r.Returns);

            var salesData = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
                .Join(_context.Products, o => o.OrderId, p => p.OrderId, (o, p) => new { o.State, p.Sales })
                .GroupBy(op => op.State)
                .Select(g => new SalesData
                {
                    State = g.Key,
                    Sales = g.Sum(op => op.Sales ?? 0) - (totalReturnsByState.ContainsKey(g.Key) ? totalReturnsByState[g.Key] : 0),
                    IncrementedSales = (g.Sum(op => op.Sales ?? 0) - (totalReturnsByState.ContainsKey(g.Key) ? totalReturnsByState[g.Key] : 0)) * (1 + (percentage / 100)),
                    PercentageIncrease = percentage
                })
                .ToList();

            ViewBag.TotalSales = salesData.Sum(s => s.Sales);
            ViewBag.TotalIncrementedSales = salesData.Sum(s => s.IncrementedSales);
            ViewBag.Year = year;
            ViewBag.Percentage = percentage;
            ViewBag.States = _context.Orders.Select(o => o.State).Distinct().ToList();

            return View("Index", salesData);
        }
        public IActionResult StateWise()
        {
            ViewBag.Step = TempData["Step"] ?? 1;
            ViewBag.States = _context.Orders.Select(o => o.State).Distinct().ToList();
            ViewBag.State = TempData["State"];
            ViewBag.Year = TempData["Year"];
            ViewBag.PercentageIncrease = TempData["PercentageIncrease"];
            ViewBag.TotalSales = TempData["TotalSales"];
            ViewBag.TotalIncrementedSales = TempData["TotalIncrementedSales"];

            if (TempData["SalesData"] != null)
            {
                var salesData = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<SalesData>>(TempData["SalesData"].ToString());
                return View(salesData);
            }

            return View();
        }

        [HttpPost]
        public IActionResult StateWise(string state, int? year, double? percentageIncrease)
        {
            if (percentageIncrease == null && year == null)
            {
                // Step 1: State and year selection
                TempData["Step"] = 2;
                TempData["State"] = state;
                TempData["Year"] = year.ToString();
                return RedirectToAction("StateWise");
            }
            else if (percentageIncrease == null)
            {
                // Step 2: Percentage input
                TempData["Step"] = 2;
                TempData["State"] = state;
                TempData["Year"] = year.ToString();
                return RedirectToAction("StateWise");
            }
            else
            {
                // Calculate results
                var returnedOrderIds = _context.OrdersReturns
                    .Join(_context.Orders, r => r.OrderId, o => o.OrderId, (r, o) => new { r.OrderId, o.OrderDate })
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year.Value)
                    .Select(o => o.OrderId)
                    .ToList();

                var returnsData = _context.Products
                    .Where(p => returnedOrderIds.Contains(p.OrderId))
                    .Join(_context.Orders, p => p.OrderId, o => o.OrderId, (p, o) => new { o.State, p.Sales })
                    .GroupBy(po => po.State)
                    .Select(g => new
                    {
                        State = g.Key,
                        Returns = g.Sum(po => po.Sales ?? 0)
                    })
                    .ToList();

                var totalReturnsByState = returnsData.ToDictionary(r => r.State, r => r.Returns);

                var salesData = _context.Orders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year.Value && o.State == state)
                    .Join(_context.Products, o => o.OrderId, p => p.OrderId, (o, p) => new { o.State, p.Sales })
                    .GroupBy(op => op.State)
                    .Select(g => new SalesData
                    {
                        State = g.Key,
                        Sales = g.Sum(op => op.Sales ?? 0) - (totalReturnsByState.ContainsKey(g.Key) ? totalReturnsByState[g.Key] : 0),
                        IncrementedSales = (g.Sum(op => op.Sales ?? 0) - (totalReturnsByState.ContainsKey(g.Key) ? totalReturnsByState[g.Key] : 0)) * (1 + (percentageIncrease.Value / 100)),
                        PercentageIncrease = percentageIncrease.Value
                    })
                    .ToList();

                TempData["TotalSales"] = salesData.Sum(s => s.Sales).ToString();
                TempData["TotalIncrementedSales"] = salesData.Sum(s => s.IncrementedSales).ToString();
                TempData["State"] = state;
                TempData["Year"] = year.Value.ToString();
                TempData["PercentageIncrease"] = percentageIncrease.Value.ToString();
                TempData["Step"] = 3;
                TempData["SalesData"] = Newtonsoft.Json.JsonConvert.SerializeObject(salesData);

                return RedirectToAction("StateWise");
            }
        }


        public IActionResult Chart(int year, double percentage)
        {
            ViewBag.Year = year;
            ViewBag.Percentage = percentage;
            return View();
        }

        public IActionResult GetChartData(int year, double percentage)
        {
            var salesData = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
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
            Console.WriteLine(year);

            var totalSales = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
                .Join(_context.Products, o => o.OrderId, p => p.OrderId, (o, p) => p.Sales)
                .Sum(sales => sales ?? 0);

            Console.WriteLine("Total Sales: " + totalSales);

            var totalIncrementedSales = totalSales * (1 + (percentage / 100));

            Console.WriteLine("Total Incremented Sales: " + totalIncrementedSales);

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
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
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
