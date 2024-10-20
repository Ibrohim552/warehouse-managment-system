using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using WareHouse.Entities;

public sealed class SupplierService : ISupplierService
{
    private readonly string _pathData;

    public SupplierService(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", "true");
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Suppliers));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<IEnumerable<Supplier>> GetSuppliersAsync()
    {
        XDocument document = XDocument.Load(_pathData);
        return document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Suppliers)?
            .Elements(XmlElements.Supplier)
            .Select(x => new Supplier
            {
                Id = (int)x.Element(XmlElements.SupplierId)!,
                Name = (string)x.Element(XmlElements.SupplierName)!,
                ContactPerson = (string)x.Element(XmlElements.SupplierContactPerson)!,
                Email = (string)x.Element(XmlElements.SupplierEmail)!,
                Phone = (string)x.Element(XmlElements.SupplierPhone)!
            }) ?? Enumerable.Empty<Supplier>();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(int id)
    {
        XDocument document = XDocument.Load(_pathData);
        var supplierElement = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Suppliers)?
            .Elements(XmlElements.Supplier)
            .FirstOrDefault(x => (int)x.Element(XmlElements.SupplierId)! == id);

        return supplierElement != null ? new Supplier
        {
            Id = (int)supplierElement.Element(XmlElements.SupplierId)!,
            Name = (string)supplierElement.Element(XmlElements.SupplierName)!,
            ContactPerson = (string)supplierElement.Element(XmlElements.SupplierContactPerson)!,
            Email = (string)supplierElement.Element(XmlElements.SupplierEmail)!,
            Phone = (string)supplierElement.Element(XmlElements.SupplierPhone)!
        } : null;
    }

    public async Task<bool> CreateSupplierAsync(Supplier supplier)
    {
        XDocument document = XDocument.Load(_pathData);
        int maxId = 0;

        var supplierElements = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Suppliers)?.Elements(XmlElements.Supplier);

        if (supplierElements != null && supplierElements.Any())
        {
            maxId = supplierElements.Max(x => (int)x.Element(XmlElements.SupplierId)!);
        }

        XElement supplierElement = new XElement(XmlElements.Supplier,
            new XElement(XmlElements.SupplierId, maxId + 1),
            new XElement(XmlElements.SupplierName, supplier.Name),
            new XElement(XmlElements.SupplierContactPerson, supplier.ContactPerson),
            new XElement(XmlElements.SupplierEmail, supplier.Email),
            new XElement(XmlElements.SupplierPhone, supplier.Phone)
        );

        document.Element(XmlElements.DataSource)!.Element(XmlElements.Suppliers)!.Add(supplierElement);
        document.Save(_pathData);
        return true;
    }

    public async Task<bool> UpdateSupplierAsync(Supplier supplier)
    {
        XDocument document = XDocument.Load(_pathData);
        var supplierElement = document.Descendants(XmlElements.Supplier)
            .FirstOrDefault(x => (int)x.Element(XmlElements.SupplierId)! == supplier.Id);

        if (supplierElement == null) return false;

        supplierElement.SetElementValue(XmlElements.SupplierName, supplier.Name);
        supplierElement.SetElementValue(XmlElements.SupplierContactPerson, supplier.ContactPerson);
        supplierElement.SetElementValue(XmlElements.SupplierEmail, supplier.Email);
        supplierElement.SetElementValue(XmlElements.SupplierPhone, supplier.Phone);

        document.Save(_pathData);
        return true;
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        XDocument document = XDocument.Load(_pathData);
        var supplierElement = document.Descendants(XmlElements.Supplier)
            .FirstOrDefault(x => (int)x.Element(XmlElements.SupplierId)! == id);

        if (supplierElement == null) return false;

        supplierElement.Remove();
        document.Save(_pathData);
        return true;
    }

    public async Task<IEnumerable<Supplier>> GetSuppliersByMinProductQuantityAsync(int minQuantity)
    {
        XDocument document = XDocument.Load(_pathData);
        var supplierIdsWithMinProducts = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Orders)?
            .Elements(XmlElements.Order)
            .Where(x => (int)x.Element(XmlElements.OrderQuantity)! >= minQuantity)
            .Select(x => (int)x.Element(XmlElements.OrderSupplierId)!);

        return document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Suppliers)?
            .Elements(XmlElements.Supplier)
            .Where(x => supplierIdsWithMinProducts.Contains((int)x.Element(XmlElements.SupplierId)!))
            .Select(x => new Supplier
            {
                Id = (int)x.Element(XmlElements.SupplierId)!,
                Name = (string)x.Element(XmlElements.SupplierName)!,
                ContactPerson = (string)x.Element(XmlElements.SupplierContactPerson)!,
                Email = (string)x.Element(XmlElements.SupplierEmail)!,
                Phone = (string)x.Element(XmlElements.SupplierPhone)!
            }) ?? Enumerable.Empty<Supplier>();
    }
    public async Task<IEnumerable<Supplier>> GetSuppliersWithMinProductQuantityAsync(int minQuantity)
    {
        XDocument document = XDocument.Load(_pathData);
        var suppliers = document.Element(XmlElements.DataSource)
            .Element(XmlElements.Suppliers)
            .Elements(XmlElements.Supplier)
            .Where(s => document.Element(XmlElements.DataSource)
                .Element(XmlElements.Products)
                .Elements(XmlElements.Product)
                .Count(p => (int)p.Element(XmlElements.SupplierId) == (int)s.Element(XmlElements.SupplierId) && 
                            (int)p.Element(XmlElements.ProductQuantity) >= minQuantity) > 0);

        return suppliers.Select(s => new Supplier
        {
            Id = (int)s.Element(XmlElements.SupplierId),
            Name = (string)s.Element(XmlElements.SupplierName),
            ContactPerson = (string)s.Element(XmlElements.SupplierContactPerson),
            Email = (string)s.Element(XmlElements.SupplierEmail),
            Phone = (string)s.Element(XmlElements.SupplierPhone)
        });
    }
}
