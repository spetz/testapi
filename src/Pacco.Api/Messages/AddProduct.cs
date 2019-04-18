using System;

namespace Pacco.Api.Messages
{
    public class AddProduct : ICommand
    {
        public Guid Id { get; }
        public string Name { get; }
        public decimal Price { get; }

        public AddProduct(string name, decimal price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
        }
    }
}