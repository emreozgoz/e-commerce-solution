using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypesRepository
    {
        public ICatalogContext _context { get; }
        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }
        public async Task<Product> CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var deletedItem = await _context
                .Products.DeleteOneAsync(x => x.Id == id);

            return deletedItem.IsAcknowledged && deletedItem.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
        {
            return await _context
                .Brands
                .Find(x => true)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductType>> GetAllTypes()
        {
            return await _context
                .Types
                .Find(x => true)
                .ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _context
                .Products
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context
                .Products
                .Find(x => true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrand(string name)
        {
            return await _context
                .Products
                .Find(x => x.Brands.Name.ToLower() == name.ToLower())
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            return await _context
                .Products
                .Find(x => x.Name.ToLower() == name.ToLower())
                .ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updatedProducts = await _context
                .Products
                .ReplaceOneAsync(x => x.Id == product.Id, product);
            return updatedProducts.IsAcknowledged && updatedProducts.ModifiedCount > 0;
        }
    }
}
