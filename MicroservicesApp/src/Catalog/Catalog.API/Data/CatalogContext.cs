using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        private readonly ICatalogDatabaseSettings _catalogSettings;

        public CatalogContext(ICatalogDatabaseSettings catalogSettings)
        {
            // getting instance of CatalogDatabaseSettings
            _catalogSettings = catalogSettings;
            // Getting databse from mongoDb database using connection string and database name
            var client = new MongoClient(_catalogSettings.ConnectionString);
            var database = client.GetDatabase(_catalogSettings.DatabaseName);
            // getting collection from Catalog database called Products
            Products = database.GetCollection<Product>(_catalogSettings.CollectionName);

            // getting data from collection Product
            CatalogContextSeed.SeedData(Products);
        }
        public IMongoCollection<Product> Products { get; }
    }
}
