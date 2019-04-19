using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pacco.Api.Builders;
using Pacco.Api.Messages;

namespace Pacco.Api
{
    public static class Extensions
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();
        private const string EmptyJsonObject = "{}";

        public static IApplicationBuilder UseEndpoints(this IApplicationBuilder app, Action<IEndpointsBuilder> builder)
            => app.UseRouter(router => builder(new EndpointsBuilder(router)));

        public static IApplicationBuilder UseDispatcherEndpoints(this IApplicationBuilder app,
            Action<IDispatcherEndpointsBuilder> builder)
            => app.UseRouter(router => builder(new DispatcherEndpointsBuilder(new EndpointsBuilder(router))));
        
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddRouting()
                .AddLogging()
                .AddMvcCore()
                .AddJsonFormatters();

            return services;
        }

        public static IServiceCollection ScanAssemblyTypes(this IServiceCollection services)
        {
            services.Scan(scan => scan.FromCallingAssembly().AddClasses().AsMatchingInterface()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
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

        public static void WriteJson<T>(this HttpResponse response, T obj)
        {
            response.ContentType = "application/json";
            using (var writer = new HttpResponseStreamWriter(response.Body, Encoding.UTF8))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.CloseOutput = false;
                    jsonWriter.AutoCompleteOnClose = false;
                    Serializer.Serialize(jsonWriter, obj);
                }
            }
        }

        public static T ReadJson<T>(this HttpContext httpContext)
        {
            using (var streamReader = new StreamReader(httpContext.Request.Body))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var obj = Serializer.Deserialize<T>(jsonTextReader);

                var results = new List<ValidationResult>();
                if (Validator.TryValidateObject(obj, new ValidationContext(obj), results))
                {
                    return obj;
                }

                httpContext.Response.StatusCode = 400;
                httpContext.Response.WriteJson(results);

                return default(T);
            }
        }


        public static Task Accepted(this HttpResponse response)
        {
            response.StatusCode = 202;
            return Task.CompletedTask;
        }

        public static Task NotFound(this HttpResponse response)
        {
            response.StatusCode = 404;
            return Task.CompletedTask;
        }

        public static Task Ok<T>(this HttpResponse response, T data)
        {
            response.StatusCode = 202;
            response.WriteJson(data);
            return Task.CompletedTask;
        }

        public static Task Created(this HttpResponse response, string location = null)
        {
            response.StatusCode = 201;
            if (!string.IsNullOrEmpty(location))
            {
                response.Headers.TryAdd("Location", location);
            }
            return Task.CompletedTask;
        }

        public static Task NoContent(this HttpResponse response)
        {
            response.StatusCode = 204;
            return Task.CompletedTask;
        }


        public static T ReadQuery<T>(this HttpContext context) where T : class
        {
            var request = context.Request;
            RouteValueDictionary values = null;
            if (HasRouteData(request))
            {
                values = request.HttpContext.GetRouteData().Values;
            }

            if (HasQueryString(request))
            {
                var queryString = HttpUtility.ParseQueryString(request.HttpContext.Request.QueryString.Value);
                values = values ?? new RouteValueDictionary();
                foreach (var key in queryString.AllKeys)
                {
                    values.TryAdd(key, queryString[key]);
                }
            }

            return values is null
                ? JsonConvert.DeserializeObject<T>(EmptyJsonObject)
                : JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(values));
        }

        private static bool HasQueryString(this HttpRequest request)
            => request.Query.Any();

        private static bool HasRouteData(this HttpRequest request)
            => request.HttpContext.GetRouteData().Values.Any();
    }
}