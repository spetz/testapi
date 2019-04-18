using System;

namespace Pacco.Api.Messages
{
    public class DeleteProduct : ICommand
    {
        public Guid Id { get; }

        public DeleteProduct(Guid id)
        {
            Id = id;
        }
    }
}