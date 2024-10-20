using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using WareHouse.Entities;
using WareHouse.Services.OrderService;

public sealed class OrderService : IOrderService
{
    private readonly string _pathData;

    public OrderService(IConfiguration configuration)
    {
        _pathData = configuration.GetSection(XmlElements.PathData).Value!;

        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration("1.0", "utf-8", "true");
            XElement xElement = new XElement(XmlElements.DataSource, new XElement(XmlElements.Orders));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        XDocument document = XDocument.Load(_pathData);
        return document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Orders)?
            .Elements(XmlElements.Order)
            .Select(x => new Order
            {
                Id = (int)x.Element(XmlElements.OrderId)!,
                ProductId = (int)x.Element(XmlElements.OrderProductId)!,
                Quantity = (int)x.Element(XmlElements.OrderQuantity)!,
                OrderDate = (DateTime)x.Element(XmlElements.OrderDate)!,
                SupplierId = (int)x.Element(XmlElements.OrderSupplierId)!,
                Status = (string)x.Element(XmlElements.OrderStatus)!
            }) ?? Enumerable.Empty<Order>();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        XDocument document = XDocument.Load(_pathData);
        var orderElement = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Orders)?
            .Elements(XmlElements.Order)
            .FirstOrDefault(x => (int)x.Element(XmlElements.OrderId)! == id);

        return orderElement != null ? new Order
        {
            Id = (int)orderElement.Element(XmlElements.OrderId)!,
            ProductId = (int)orderElement.Element(XmlElements.OrderProductId)!,
            Quantity = (int)orderElement.Element(XmlElements.OrderQuantity)!,
            OrderDate = (DateTime)orderElement.Element(XmlElements.OrderDate)!,
            SupplierId = (int)orderElement.Element(XmlElements.OrderSupplierId)!,
            Status = (string)orderElement.Element(XmlElements.OrderStatus)!
        } : null;
    }

    public async Task<bool> CreateOrderAsync(Order order)
    {
        XDocument document = XDocument.Load(_pathData);
        int maxId = 0;

        var orderElements = document.Element(XmlElements.DataSource)?
            .Element(XmlElements.Orders)?.Elements(XmlElements.Order);

        if (orderElements != null && orderElements.Any())
        {
            maxId = orderElements.Max(x => (int)x.Element(XmlElements.OrderId)!);
        }

        XElement orderElement = new XElement(XmlElements.Order,
            new XElement(XmlElements.OrderId, maxId + 1),
            new XElement(XmlElements.OrderProductId, order.ProductId),
            new XElement(XmlElements.OrderQuantity, order.Quantity),
            new XElement(XmlElements.OrderDate, order.OrderDate),
            new XElement(XmlElements.OrderSupplierId, order.SupplierId),
            new XElement(XmlElements.OrderStatus, order.Status)
        );

        document.Element(XmlElements.DataSource)!.Element(XmlElements.Orders)!.Add(orderElement);
        document.Save(_pathData);
        return true;
    }

    public async Task<bool> UpdateOrderAsync(Order order)
    {
        XDocument document = XDocument.Load(_pathData);
        var orderElement = document.Descendants(XmlElements.Order)
            .FirstOrDefault(x => (int)x.Element(XmlElements.OrderId)! == order.Id);

        if (orderElement == null) return false;

        orderElement.SetElementValue(XmlElements.OrderProductId, order.ProductId);
        orderElement.SetElementValue(XmlElements.OrderQuantity, order.Quantity);
        orderElement.SetElementValue(XmlElements.OrderDate, order.OrderDate);
        orderElement.SetElementValue(XmlElements.OrderSupplierId, order.SupplierId);
        orderElement.SetElementValue(XmlElements.OrderStatus, order.Status);

        document.Save(_pathData);
        return true;
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        XDocument document = XDocument.Load(_pathData);
        var orderElement = document.Descendants(XmlElements.Order)
            .FirstOrDefault(x => (int)x.Element(XmlElements.OrderId)! == id);

        if (orderElement == null) return false;

        orderElement.Remove();
        document.Save(_pathData);
        return true;
    }
    public async Task<IEnumerable<Order>> GetOrdersBySupplierAndStatusAsync(int supplierId, string status)
    {
        XDocument document = XDocument.Load(_pathData);
        var orders = document.Element(XmlElements.DataSource)
            .Element(XmlElements.Orders)
            .Elements(XmlElements.Order)
            .Where(o => (int)o.Element(XmlElements.OrderSupplierId) == supplierId && 
                        (string)o.Element(XmlElements.OrderStatus) == status);

        return orders.Select(o => new Order
        {
            Id = (int)o.Element(XmlElements.OrderId),
            ProductId = (int)o.Element(XmlElements.OrderProductId),
            Quantity = (int)o.Element(XmlElements.OrderQuantity),
            OrderDate = (DateTime)o.Element(XmlElements.OrderDate),
            SupplierId = (int)o.Element(XmlElements.OrderSupplierId),
            Status = (string)o.Element(XmlElements.OrderStatus)
        });
    }
    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        XDocument document = XDocument.Load(_pathData);
        var orders = document.Element(XmlElements.DataSource)
            .Element(XmlElements.Orders)
            .Elements(XmlElements.Order)
            .Where(o => (DateTime)o.Element(XmlElements.OrderDate) >= startDate &&
                        (DateTime)o.Element(XmlElements.OrderDate) <= endDate);

        return orders.Select(o => new Order
        {
            Id = (int)o.Element(XmlElements.OrderId),
            ProductId = (int)o.Element(XmlElements.OrderProductId),
            Quantity = (int)o.Element(XmlElements.OrderQuantity),
            OrderDate = (DateTime)o.Element(XmlElements.OrderDate),
            SupplierId = (int)o.Element(XmlElements.OrderSupplierId),
            Status = (string)o.Element(XmlElements.OrderStatus)
        });
    }
    public async Task<IEnumerable<Order>> GetProductsThatWasOrderMorethan5Time()
    {
        XDocument document = XDocument.Load(_pathData);
        var orders = document.Element(XmlElements.DataSource)
           .Element(XmlElements.Orders)
           .Elements(XmlElements.Order)
           .GroupBy(o => (int)o.Element(XmlElements.OrderProductId))
           .Where(g => g.Count() > 5);

        return orders.SelectMany(g => g.Select(o => new Order
        {
            Id = (int)o.Element(XmlElements.OrderId),
            ProductId = (int)o.Element(XmlElements.OrderProductId),
            Quantity = (int)o.Element(XmlElements.OrderQuantity),
            OrderDate = (DateTime)o.Element(XmlElements.OrderDate),
            SupplierId = (int)o.Element(XmlElements.OrderSupplierId),
            Status = (string)o.Element(XmlElements.OrderStatus)
        }));
    }
    
}
