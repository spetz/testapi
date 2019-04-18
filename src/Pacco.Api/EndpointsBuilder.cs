using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Pacco.Api.Messages;

namespace Pacco.Api
{
    public class EndpointsBuilder : IEndpointsBuilder
    {
        private readonly IRouteBuilder _routeBuilder;

        public EndpointsBuilder(IRouteBuilder routeBuilder)
        {
            _routeBuilder = routeBuilder;
        }

        public IEndpointsBuilder Get(string path, Action<HttpResponse> response = null)
        {
            _routeBuilder.MapGet(path, (req, res, data) =>
            {
                response?.Invoke(res);
                return Task.CompletedTask;
            });

            return this;
        }

        public IEndpointsBuilder Get<TQuery, TResult>(string path,
            Action<TQuery, TResult, HttpResponse> response = null) where TQuery : class, IQuery<TResult>, new()
        {
            _routeBuilder.MapGet(path, (req, res, data) => req.QueryAsync<TQuery, TResult>()
                .ReturnAsync(query => response?.Invoke(query.query, query.result, res)));

            return this;
        }

        public IEndpointsBuilder Post<T>(string path, Action<T, HttpResponse> response = null) where T : class, ICommand
        {
            _routeBuilder.MapPost(path, (req, res, data) => req.SendAsync<T>()
                .ReturnAsync(cmd => response?.Invoke(cmd, res)));

            return this;
        }

        public IEndpointsBuilder Delete<T>(string path, Action<T, HttpResponse> response = null)
            where T : class, ICommand
        {
            return this;
        }
    }
}