using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using WareHouse.Entities;

public sealed class CategoryService : ICategoryService
{
    private readonly string _pathData;

    public CategoryService(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", "true");
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Categories));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        XDocument document = XDocument.Load(_pathData);
        return document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Categories)?
            .Elements(XmlElements.Category)
            .Select(x => new Category
            {
                Id = (int)x.Element(XmlElements.CategoryId)!,
                Name = (string)x.Element(XmlElements.CategoryName)!,
                Description = (string)x.Element(XmlElements.CategoryDescription)!
            }) ?? Enumerable.Empty<Category>();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        XDocument document = XDocument.Load(_pathData);
        var categoryElement = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Categories)?
            .Elements(XmlElements.Category)
            .FirstOrDefault(x => (int)x.Element(XmlElements.CategoryId)! == id);

        return categoryElement != null ? new Category
        {
            Id = (int)categoryElement.Element(XmlElements.CategoryId)!,
            Name = (string)categoryElement.Element(XmlElements.CategoryName)!,
            Description = (string)categoryElement.Element(XmlElements.CategoryDescription)!
        } : null;
    }

    public async Task<bool> CreateCategoryAsync(Category category)
    {
        XDocument document = XDocument.Load(_pathData);
        int maxId = 0;

        var categoryElements = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Categories)?.Elements(XmlElements.Category);

        if (categoryElements != null && categoryElements.Any())
        {
            maxId = categoryElements.Max(x => (int)x.Element(XmlElements.CategoryId)!);
        }

        XElement categoryElement = new XElement(XmlElements.Category,
            new XElement(XmlElements.CategoryId, maxId + 1),
            new XElement(XmlElements.CategoryName, category.Name),
            new XElement(XmlElements.CategoryDescription, category.Description)
        );

        document.Element(XmlElements.DataSource)!.Element(XmlElements.Categories)!.Add(categoryElement);
        document.Save(_pathData);
        return true;
    }

    public async Task<bool> UpdateCategoryAsync(Category category)
    {
        XDocument document = XDocument.Load(_pathData);
        var categoryElement = document.Descendants(XmlElements.Category)
            .FirstOrDefault(x => (int)x.Element(XmlElements.CategoryId)! == category.Id);

        if (categoryElement == null) return false;

        categoryElement.SetElementValue(XmlElements.CategoryName, category.Name);
        categoryElement.SetElementValue(XmlElements.CategoryDescription, category.Description);

        document.Save(_pathData);
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        XDocument document = XDocument.Load(_pathData);
        var categoryElement = document.Descendants(XmlElements.Category)
            .FirstOrDefault(x => (int)x.Element(XmlElements.CategoryId)! == id);

        if (categoryElement == null) return false;

        categoryElement.Remove();
        document.Save(_pathData);
        return true;
    }
    
   
}
