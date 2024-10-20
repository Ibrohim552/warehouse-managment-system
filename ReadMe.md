# Warehouse Management API

## Описание

**Warehouse Management API** — это система управления складом, разработанная с использованием **ASP.NET Core Web API (.NET 8)**. Она использует **XML-файлы** для хранения данных о товарах, заказах, категориях и поставщиках. API поддерживает базовые операции **CRUD** и сложные запросы с фильтрацией и сортировкой.

### Основные технологии:

- **.NET 8** (ASP.NET Core Web API)
- **XML-файлы** для хранения данных
- **Асинхронное программирование** для оптимальной работы
- **Dependency Injection** (DI) для управления зависимостями
- **Middleware** для обработки ошибок
- **API Conventions** для стандартизации методов API

## Структура проекта

### Основные компоненты:

- **Entities**: содержит классы сущностей (Product, Category, Order, Supplier).
- **Services**: содержит сервисы для операций с данными.
- **Controllers**: контроллеры для обработки запросов API.
- **Middleware**: промежуточное ПО для обработки ошибок и логики.
- **Extensions**: методы расширения для работы с DI и другими компонентами.

## Структура XML-файлов

### Пример файла продуктов:

```xml
<Products>
    <Product>
        <Id>1</Id>
        <Name>Товар 1</Name>
        <Description>Описание товара 1</Description>
        <Quantity>100</Quantity>
        <Price>10.99</Price>
        <CategoryId>1</CategoryId>
    </Product>
    <Product>
        <Id>2</Id>
        <Name>Товар 2</Name>
        <Description>Описание товара 2</Description>
        <Quantity>50</Quantity>
        <Price>20.99</Price>
        <CategoryId>2</CategoryId>
    </Product>
</Products>
<Categories>
    <Category>
        <Id>1</Id>
        <Name>Категория 1</Name>
        <Description>Описание категории 1</Description>
    </Category>
    <Category>
        <Id>2</Id>
        <Name>Категория 2</Name>
        <Description>Описание категории 2</Description>
    </Category>
</Categories>
```
#Функциональность API:
##CRUD Операции для всех сущностей:
Получение товаров с фильтрацией по категории и сортировкой по цене:

GET /api/products?categoryId={categoryId}&sortBy=price&sortOrder={asc|desc}
Получение товаров с количеством меньше заданного значения:

GET /api/products?maxQuantity={maxQuantity}
Получение всех заказов для конкретного поставщика с фильтрацией по статусу:

GET /api/orders?supplierId={supplierId}&status={status}
Получение списка поставщиков, у которых есть товары с определённым количеством на складе:

GET /api/suppliers?minProductQuantity={minQuantity}
Получение информации о заказах с указанием диапазона дат:

GET /api/orders?startDate={startDate}&endDate={endDate}
Получение категории с количеством товаров в каждой категории:

GET /api/categories/withProductCount
Получение информации о товаре вместе с его категорией и поставщиком:

GET /api/products/{id}/details
Получение всех заказов с пагинацией:

GET /api/orders?pageNumber={pageNumber}&pageSize={pageSize}
Получение всех товаров с их категориями и поставщиками с пагинацией:

GET /api/products?pageNumber={pageNumber}&pageSize={pageSize}&includeDetails=true
Получение всех товаров, которые были заказаны более 5 раз:

GET /api/products/mostOrdered?minOrders=5
