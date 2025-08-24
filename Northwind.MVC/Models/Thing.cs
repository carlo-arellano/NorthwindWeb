using System.ComponentModel.DataAnnotations;

namespace Northwind.MVC.Models;

public record Thing(
    [Range(1, 10)] int? Id,
    [Required] string? Color,
    [EmailAddress] string? Email
);