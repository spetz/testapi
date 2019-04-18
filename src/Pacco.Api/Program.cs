using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Pacco.Api.Messages;
using Pacco.Api;

// ReSharper disable once CheckNamespace

namespace MyApi
{
    public static class Program
    {
        public static async Task Main(string[] args)
            => await WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddWebApi()
                    .AddConsul()
                    .AddRabbitMq()
                    .ScanAssemblyTypes())
                .Configure(app => app.UseEndpoints(endpoints => endpoints
                    .Get("", res => res.WriteAsync("Welcome to My API!"))
                    .Get<GetProducts,IEnumerable<ProductDto>>("products", (query, products, res) => res.Ok(products))
                    .Post<AddProduct>("products", (cmd, res) => res.Created($"products/{cmd.Id}"))
                    .Delete<DeleteProduct>("products/{id}", (cmd, res) => res.NoContent())))
                .Build()
                .RunAsync();
    }
}
