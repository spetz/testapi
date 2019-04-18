using System.Threading.Tasks;

namespace Pacco.Api.Messages
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        Task HandleAsync(T command);
    }
}