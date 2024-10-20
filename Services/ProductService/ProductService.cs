using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using WareHouse.Entities;

public sealed class ProductService :  IProductService
{
    private readonly string _pathData;

    public ProductService(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", "true");
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Products));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        XDocument document = XDocument.Load(_pathData);
        return document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Products)?
            .Elements(XmlElements.Product)
            .Select(x => new Product
            {
                Id = (int)x.Element(XmlElements.ProductId)!,
                Name = (string)x.Element(XmlElements.ProductName)!,
                Description = (string)x.Element(XmlElements.ProductDescription)!,
                Quantity = (int)x.Element(XmlElements.ProductQuantity)!,
                Price = (decimal)x.Element(XmlElements.ProductPrice)!,
                CategoryId = (int)x.Element(XmlElements.ProductCategoryId)!
            }) ?? Enumerable.Empty<Product>();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        XDocument document = XDocument.Load(_pathData);
        var productElement = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Products)?
            .Elements(XmlElements.Product)
            .FirstOrDefault(x => (int)x.Element(XmlElements.ProductId)! == id);

        return productElement != null ? new Product
        {
            Id = (int)productElement.Element(XmlElements.ProductId)!,
            Name = (string)productElement.Element(XmlElements.ProductName)!,
            Description = (string)productElement.Element(XmlElements.ProductDescription)!,
            Quantity = (int)productElement.Element(XmlElements.ProductQuantity)!,
            Price = (decimal)productElement.Element(XmlElements.ProductPrice)!,
            CategoryId = (int)productElement.Element(XmlElements.ProductCategoryId)!
        } : null;
    }

    public async Task<bool> CreateProductAsync(Product product)
    {
        XDocument document = XDocument.Load(_pathData);
        int maxId = 0;

        var productElements = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Products)?.Elements(XmlElements.Product);

        if (productElements != null && productElements.Any())
        {
            maxId = productElements.Max(x => (int)x.Element(XmlElements.ProductId)!);
        }

        XElement productElement = new XElement(XmlElements.Product,
            new XElement(XmlElements.ProductId, maxId + 1),
            new XElement(XmlElements.ProductName, product.Name),
            new XElement(XmlElements.ProductDescription, product.Description),
            new XElement(XmlElements.ProductQuantity, product.Quantity),
            new XElement(XmlElements.ProductPrice, product.Price),
            new XElement(XmlElements.ProductCategoryId, product.CategoryId)
        );

        document.Element(XmlElements.DataSource)!.Element(XmlElements.Products)!.Add(productElement);
        document.Save(_pathData);
        return true;
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        XDocument document = XDocument.Load(_pathData);
        var productElement = document.Descendants(XmlElements.Product)
            .FirstOrDefault(x => (int)x.Element(XmlElements.ProductId)! == product.Id);

        if (productElement == null) return false;

        productElement.SetElementValue(XmlElements.ProductName, product.Name);
        productElement.SetElementValue(XmlElements.ProductDescription, product.Description);
        productElement.SetElementValue(XmlElements.ProductQuantity, product.Quantity);
        productElement.SetElementValue(XmlElements.ProductPrice, product.Price);
        productElement.SetElementValue(XmlElements.ProductCategoryId, product.CategoryId);

        document.Save(_pathData);
        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        XDocument document = XDocument.Load(_pathData);
        var productElement = document.Descendants(XmlElements.Product)
            .FirstOrDefault(x => (int)x.Element(XmlElements.ProductId)! == id);

        if (productElement == null) return false;

        productElement.Remove();
        document.Save(_pathData);
        return true;
    }
    
    public async Task<IEnumerable<Product>> GetProductsByCategoryAndSortAsync(int categoryId, string sortBy, string sortOrder)
    {
        XDocument document = XDocument.Load(_pathData);
        var products = document.Element(XmlElements.DataSource)
            .Element(XmlElements.Products)
            .Elements(XmlElements.Product)
            .Where(p => (int)p.Element(XmlElements.ProductCategoryId) == categoryId);
        products = sortOrder.ToLower() == "desc"
            ? products.OrderByDescending(p => (decimal)p.Element(sortBy))
            : products.OrderBy(p => (decimal)p.Element(sortBy));

        return products.Select(p => new Product
        {
            Id = (int)p.Element(XmlElements.ProductId),
            Name = (string)p.Element(XmlElements.ProductName),
            Description = (string)p.Element(XmlElements.ProductDescription),
            Quantity = (int)p.Element(XmlElements.ProductQuantity),
            Price = (decimal)p.Element(XmlElements.ProductPrice),
            CategoryId = (int)p.Element(XmlElements.ProductCategoryId)
        });
    }
    public async Task<IEnumerable<Product>> GetInfoAboutProductCategoryAndSuppliers()
    {
        XDocument document = XDocument.Load(_pathData);
        return document.Element(XmlElements.DataSource)
            .Element(XmlElements.Products)
            .Elements(XmlElements.Product)
            .Select(p => new Product
            {
                Id = (int)p.Element(XmlElements.ProductId),
                Name = (string)p.Element(XmlElements.ProductName),
                Description = (string)p.Element(XmlElements.ProductDescription),
                Quantity = (int)p.Element(XmlElements.ProductQuantity),
                Price = (decimal)p.Element(XmlElements.ProductPrice),
                CategoryId = (int)p.Element(XmlElements.ProductCategoryId),

            });
    }
}
