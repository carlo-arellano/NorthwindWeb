using Northwind.EntityModels;

namespace Northwind.MVC.Models;

public record HomeIndexViewModel(int VisitorCount,
    IList<Category> Categories, IList<Product> Products);
