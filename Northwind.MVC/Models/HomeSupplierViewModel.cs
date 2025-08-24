using Northwind.EntityModels;

namespace Northwind.MVC.Models;

public record HomeSupplierViewModel(int EntitiesAffected, Supplier? Supplier);