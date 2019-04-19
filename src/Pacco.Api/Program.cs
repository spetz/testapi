using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Pacco.Api.Messages;

namespace Pacco.Api
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
                .Configure(app => app.UseDispatcherEndpoints(endpoints => endpoints
                    .Get("", ctx => ctx.Response.WriteAsync("Welcome to My API!"))
                    .Get<GetProducts, IEnumerable<ProductDto>>("products",
                        afterDispatch: (query, products, ctx) => ctx.Response.Ok(products))
                    .Get<GetProducts, IEnumerable<ProductDto>>("products/{minPrice}/{maxPrice}",
                        afterDispatch: (query, products, ctx) => ctx.Response.Ok(products))
                    .Post<AddProduct>("products",
                        afterDispatch: (cmd, ctx) => ctx.Response.Created($"products/{cmd.Id}"))
                    .Delete<DeleteProduct>("products/{id}", afterDispatch: (cmd, ctx) => ctx.Response.NoContent())))
                .Build()
                .RunAsync();
    }
}
