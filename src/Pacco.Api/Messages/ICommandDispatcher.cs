using System.Threading.Tasks;

namespace Pacco.Api.Messages
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync<T>(T command) where T : ICommand;
    }
}