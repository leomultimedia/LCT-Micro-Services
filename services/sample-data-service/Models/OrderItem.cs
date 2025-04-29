using System.ComponentModel.DataAnnotations;

namespace SampleDataService.Models;

public class OrderItem
{
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    public Order Order { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }

    [Required]
    public Product Product { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
} 