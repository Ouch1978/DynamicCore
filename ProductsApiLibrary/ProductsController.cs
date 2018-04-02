using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using DynamicCore.DynamicController.Common;
using ProductsApiLibrary.Models;

namespace ProductsApiLibrary
{
    public class ProductsController : DynamicControllerBase
    {

        public ProductsController()
        {
        }

        readonly Product[] _products = new Product[]  
        {  
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },  
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },  
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }  
        };

        /// <summary>
        /// 
        /// </summary>
        /// <returns>All the products.</returns>
        public IEnumerable<Product> GetAllProducts()
        {
            return _products;
        }

        /// <summary>
        /// Test3
        /// </summary>
        /// <param name="id">Test</param>
        /// <returns>Test2</returns>
        public Product GetProductById( int id )
        {
            var product = _products.FirstOrDefault( ( p ) => p.Id == id );
            if( product == null )
            {
                throw new HttpResponseException( HttpStatusCode.NotFound );
            }
            return product;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetProductsByCategory( string category )
        {
            return _products.Where( p => string.Equals( p.Category , category ,
                StringComparison.OrdinalIgnoreCase ) );
        }
    }}
