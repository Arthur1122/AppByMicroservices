using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using Catalog.API.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _catalogContext;

        public ProductRepository(ICatalogContext catalogContext)
        {
            this._catalogContext = catalogContext;
        }

        public async Task<Product> GetProductById(string id)
        {
            return await _catalogContext
                                     .Products
                                     .Find(c => c.Id == id)
                                     .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _catalogContext
                                    .Products
                                    .Find(c => true)
                                    .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(c => c.Category, categoryName);

            return await _catalogContext
                                    .Products
                                    .Find(filter)
                                    .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(c => c.Name, name);

            return await _catalogContext
                                    .Products
                                    .Find(filter)
                                    .ToListAsync();
        }
        public async Task Create(Product product)
        {
            await _catalogContext.Products.InsertOneAsync(product);
        }
        public async Task<bool> Update(Product product)
        {
            var updateResult = await _catalogContext
                                                .Products
                                                .ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);
            return updateResult.IsAcknowledged
                    && updateResult.MatchedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(c => c.Id, id);

            DeleteResult deleteResult = await _catalogContext.Products.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged
                                    && deleteResult.DeletedCount > 0;
        }
        
    }
}
