using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Api.Messages;
using Utf8Json;
using Utf8Json.AspNetCoreMvcFormatter;
using Utf8Json.Resolvers;

namespace Pacco.Api
{
    public static class Extensions
    {
        private static readonly RouteData EmptyRouteData = new RouteData();

        public static IApplicationBuilder UseEndpoints(this IApplicationBuilder app, Action<IEndpointsBuilder> builder)
            => app.UseRouter(router => builder(new EndpointsBuilder(router)));

        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddRouting()
                .AddLogging()
                .AddMvcCore()
                .AddMvcOptions(option =>
                {
                    option.OutputFormatters.Clear();
                    option.OutputFormatters.Add(new JsonOutputFormatter(StandardResolver.Default));
                    option.InputFormatters.Clear();
                    option.InputFormatters.Add(new JsonInputFormatter());
                });

            return services;
        }

        public static IServiceCollection ScanAssemblyTypes(this IServiceCollection services)
        {
            services.Scan(scan => scan.FromCallingAssembly().AddClasses().AsMatchingInterface()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            return services;
        }

        public static IServiceCollection AddConsul(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            return services;
        }

        public static Task WriteJsonAsync<T>(this HttpResponse response, T obj)
            => JsonSerializer.NonGeneric.SerializeAsync(response.Body, obj);

        public static async Task<T> ReadJsonAsync<T>(this HttpRequest request) where T : class
            => await JsonSerializer.NonGeneric.DeserializeAsync(typeof(T), request.Body, StandardResolver.CamelCase) as
                T;

        public static async Task<(TQuery query, TResult result)> QueryAsync<TQuery, TResult>(this HttpRequest request)
            where TQuery : class, IQuery<TResult>, new()
        {
            var query = new TQuery();
            var dispatcher = request.HttpContext.RequestServices.GetService<IQueryDispatcher>();
//            var result = await dispatcher.QueryAsync(query);
            var result = await dispatcher.QueryAsync<TQuery, TResult>(query);

            return (query, result);
        }

        public static async Task<T> SendAsync<T>(this HttpRequest request) where T : class, ICommand
        {
            var command = await request.ReadJsonAsync<T>();
            var dispatcher = request.HttpContext.RequestServices.GetService<ICommandDispatcher>();
            await dispatcher.DispatchAsync(command);
            return command;
        }

        public static Task ReturnAsync(this Task _, Action @return)
        {
            @return();
            return Task.CompletedTask;
        }

        public static async Task ReturnAsync<T>(this Task<T> message, Action<T> @return) where T : ICommand
        {
            @return(await message);
        }

        public static async Task ReturnAsync<TQuery, TResult>(this Task<(TQuery query, TResult result)> queryAndResult,
            Action<(TQuery query, TResult result)> ret) where TQuery : IQuery<TResult>
        {
            ret(await queryAndResult);
        }

        public static void Accepted(this HttpResponse response)
        {
            response.StatusCode = 202;
        }

        public static void NotFound(this HttpResponse response)
        {
            response.StatusCode = 404;
        }

        public static void Ok<T>(this HttpResponse response, T data)
        {
            response.StatusCode = 202;
            response.WriteJsonAsync(data);
        }

        public static void Created(this HttpResponse response, string location = null)
        {
            response.StatusCode = 201;
            if (!string.IsNullOrEmpty(location))
            {
                response.Headers.TryAdd("Location", location);
            }
        }

        public static void NoContent(this HttpResponse response)
        {
        }
    }
}