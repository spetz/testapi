using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Pacco.Api.Messages
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public Task DispatchAsync<T>(T command) where T : ICommand
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<ICommandHandler<T>>();
                return handler.HandleAsync(command);
            }
        }
    }
}