using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
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

        public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;
            if (!string.IsNullOrEmpty(catalogSpecParams.Search))
                filter = filter & builder.Where(x => x.Name.ToLower().Contains(catalogSpecParams.Search.ToLower()));
            if (!string.IsNullOrEmpty(catalogSpecParams.BrandId))
            {
                var brandFilter = builder.Eq(x => x.Brands.Id, catalogSpecParams.BrandId);
                filter &= brandFilter;
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
            {
                var typeFilter = builder.Eq(x => x.Types.Id, catalogSpecParams.TypeId);
                filter &= typeFilter;
            }
            var totalItems = await _context.Products.CountDocumentsAsync(filter);
            var data = await DataFilter(catalogSpecParams, filter);

            return new Pagination<Product>(catalogSpecParams.PageIndex, catalogSpecParams.PageSize, (int)totalItems, data);
        }

        private async Task<IReadOnlyList<Product>> DataFilter(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortDefination = Builders<Product>.Sort.Ascending("Name");
            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                switch (catalogSpecParams.Sort)
                {
                    case "priceAsc":
                        sortDefination = Builders<Product>.Sort.Ascending(x => x.Price);
                        break;
                    case "priceDesc":
                        sortDefination = Builders<Product>.Sort.Descending(x => x.Price);
                        break;
                    default:
                        sortDefination = Builders<Product>.Sort.Ascending(x => x.Name);
                        break;
                }
            }
            return await _context
                .Products
                .Find(filter)
                .Sort(sortDefination)
                .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                .Limit(catalogSpecParams.PageSize)
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
