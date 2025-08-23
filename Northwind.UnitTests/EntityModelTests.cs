using Northwind.DataContext;
using Northwind.EntityModels;

namespace Northwind.UnitTests;

public class EntityModelTests
{
    [Fact]
    public void DatabaseConnectTest()
    {
        using NorthwindMvcContext db = new();
        Assert.True(db.Database.CanConnect());
    }

    [Fact]
    public void CategoryCountTest()
    {
        using NorthwindMvcContext db = new();

        int expected = 8;
        int actual = db.Categories.Count();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ProductId1IsChaiTest()
    {
        using NorthwindMvcContext db = new();

        string expected = "Chai";

        Product? product = db.Products.Find(keyValues: 1);
        string actual = product?.ProductName ?? string.Empty;

        Assert.Equal(expected, actual);
    }
}
