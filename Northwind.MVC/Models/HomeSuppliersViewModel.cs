using Northwind.EntityModels;

namespace Northwind.MVC.Models;

public record HomeSuppliersViewModel(IEnumerable<Supplier>? Suppliers);
