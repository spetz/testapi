using System;
using Microsoft.AspNetCore.Http;
using Pacco.Api.Messages;

namespace Pacco.Api
{
    public interface IEndpointsBuilder
    {
        IEndpointsBuilder Get(string path, Action<HttpResponse> response = null);

        IEndpointsBuilder Get<TQuery, TResult>(string path, Action<TQuery, TResult, HttpResponse> response = null)
            where TQuery : class, IQuery<TResult>, new();

        IEndpointsBuilder Post<T>(string path, Action<T, HttpResponse> response = null) where T : class, ICommand;
        IEndpointsBuilder Delete<T>(string path, Action<T, HttpResponse> response = null) where T : class, ICommand;
    }
}