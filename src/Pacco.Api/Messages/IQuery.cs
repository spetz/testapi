using System.Threading.Tasks;

namespace Pacco.Api.Messages
{
    public interface IQuery
    {
    }
    
    public interface IQuery<T> : IQuery
    {
    }
    
    public interface IQueryHandler<in TQuery,TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}