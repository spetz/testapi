using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pacco.Api.Messages;

namespace Pacco.Api.Builders
{
    public interface IDispatcherEndpointsBuilder
    {
        IDispatcherEndpointsBuilder Get(string path, Func<HttpContext, Task> context = null);

        IDispatcherEndpointsBuilder Get<TQuery, TResult>(string path,
            Func<TQuery, HttpContext, Task> beforeDispatch = null,
            Func<TQuery, TResult, HttpContext, Task> afterDispatch = null) where TQuery : class, IQuery<TResult>;

        IDispatcherEndpointsBuilder Post(string path, Func<HttpContext, Task> context = null);

        IDispatcherEndpointsBuilder Post<T>(string path, Func<T, HttpContext, Task> beforeDispatch = null,
            Func<T, HttpContext, Task> afterDispatch = null) where T : class, ICommand;

        IDispatcherEndpointsBuilder Put(string path, Func<HttpContext, Task> context = null);

        IDispatcherEndpointsBuilder Put<T>(string path, Func<T, HttpContext, Task> beforeDispatch = null,
            Func<T, HttpContext, Task> afterDispatch = null) where T : class, ICommand;

        IDispatcherEndpointsBuilder Delete(string path, Func<HttpContext, Task> context = null);

        IDispatcherEndpointsBuilder Delete<T>(string path, Func<T, HttpContext, Task> beforeDispatch = null,
            Func<T, HttpContext, Task> afterDispatch = null) where T : class, ICommand;
    }
}