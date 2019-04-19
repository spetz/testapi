using System.Collections.Generic;

namespace Pacco.Api.Messages
{
    public class GetProducts : IQuery<IEnumerable<ProductDto>>
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Name { get; set; }
    }
}