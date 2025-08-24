namespace Northwind.MVC.Models;

public record HomeModelBindingViewModel(Thing Thing, bool HasErrors,
    IEnumerable<string> ValidationErrors);
