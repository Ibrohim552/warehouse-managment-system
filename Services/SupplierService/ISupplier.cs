using WareHouse.Entities;

public interface ISupplierService
{
    Task<IEnumerable<Supplier>> GetSuppliersAsync();
    Task<Supplier?> GetSupplierByIdAsync(int id);
    Task<bool> CreateSupplierAsync(Supplier supplier);
    Task<bool> UpdateSupplierAsync(Supplier supplier);
    Task<bool> DeleteSupplierAsync(int id);
    Task<IEnumerable<Supplier>> GetSuppliersByMinProductQuantityAsync(int minQuantity);
}