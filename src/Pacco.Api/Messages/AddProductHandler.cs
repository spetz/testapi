using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Pacco.Api.Messages
{
    public class AddProductHandler : ICommandHandler<AddProduct>
    {
        private readonly ILogger<AddProductHandler> _logger;

        public AddProductHandler(ILogger<AddProductHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(AddProduct command)
        {
            _logger.LogInformation($"Handle add product: {command.Name}, {command.Price}");
            return Task.CompletedTask;
        }
    }
}