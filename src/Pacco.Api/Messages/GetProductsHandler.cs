using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pacco.Api.Messages
{
    public class GetProductsHandler : IQueryHandler<GetProducts, IEnumerable<ProductDto>>
    {
        private static readonly List<ProductDto> Products = new List<ProductDto>
        {
            new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Price = 100
            },
            new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Product 2",
                Price = 200
            },
            new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Product 3",
                Price = 300
            }
        };

        public Task<IEnumerable<ProductDto>> HandleAsync(GetProducts query)
            => Task.FromResult(Products.Where(p => p.Price >= query.MinPrice));
    }
}