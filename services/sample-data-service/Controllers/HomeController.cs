using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleDataService.Data;
using SampleDataService.Models;

namespace SampleDataService.Controllers;

public class HomeController : Controller
{
    private readonly SampleDataContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(SampleDataContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            ProductCount = await _context.Products.CountAsync(),
            UserCount = await _context.Users.CountAsync(),
            OrderCount = await _context.Orders.CountAsync(),
            RecentProducts = await _context.Products
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToListAsync(),
            RecentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync()
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class DashboardViewModel
{
    public int ProductCount { get; set; }
    public int UserCount { get; set; }
    public int OrderCount { get; set; }
    public List<Models.Product> RecentProducts { get; set; } = new();
    public List<Models.Order> RecentOrders { get; set; } = new();
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
} 