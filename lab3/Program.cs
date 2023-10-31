using lab3.Models;
using lab3.Services;
using Microsoft.EntityFrameworkCore;

namespace lab3;

internal static class Program
{
    private static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var connection = builder.Configuration.GetConnectionString("SqlServerConnection");
        services.AddDbContext<AcmeDataContext>(options => options.UseSqlServer(connection));
        services.AddMemoryCache();

        services.AddDistributedMemoryCache();
        services.AddScoped<CachedInvoices>();
        services.AddSession();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(); 

        var app = builder.Build();
        app.UseSession();
        
        app.Map("/info", (appBuilder) =>
        {
            appBuilder.Run(async (context) =>
            {
 
                string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
                                     "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                                     "<BODY><H1>Информация:</H1>";
                strResponse += "<BR> Сервер: " + context.Request.Host;
                strResponse += "<BR> Путь: " + context.Request.PathBase;
                strResponse += "<BR> Протокол: " + context.Request.Protocol;
                strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";

                await context.Response.WriteAsync(strResponse);
            });
        });

        app.Map("/invoices", (appBuilder) =>
        {
            appBuilder.Run(async (context) =>
            {
                var cachedInvoices = context.RequestServices.GetService<CachedInvoices>();
                var tableWriter = new TableWriter();

                var subscriptions = cachedInvoices?.GetInvoices("kei");
                if (subscriptions != null)
                {
                    var htmlString = tableWriter.WriteTable(subscriptions);
                    await context.Response.WriteAsync(htmlString);
                }
            });
        });
        
        app.Map("/searchBySupplierName", (appBuilder) =>
        {
            appBuilder.Run(async (context) =>
            {
                var cachedInvoices = context.RequestServices.GetService<CachedInvoices>();
                var invoices = cachedInvoices?.GetInvoices("kei");

                var tableWriter = new TableWriter();

                var formHtml = "<form method='post' action='/searchBySupplierName'>" +
                               "<label for='name'>SupplierName:</label>";



                if (context.Request.Cookies.TryGetValue("name", value: out var inputValue))
                {
                    formHtml += $"<input type='text' name='name' value='{inputValue}'><br><br>" +
                                "<input type='submit' value='Поиск'>" +
                                "</form>"; 
                }
                else
                {
                    formHtml += "<input type='text' name='name'><br><br>" + 
                                "<input type='submit' value='Поиск'>" +
                                "</form>";
                }


                if (context.Request.Method == "POST")
                {
                    string name = context.Request.Form["name"];

                    context.Response.Cookies.Append("name", name);

                    if (invoices != null)
                    {
                        var subscriptionsByPublications = invoices.Where(s => s.SupplierName == name);

                        var htmlString = tableWriter.WriteTable(subscriptionsByPublications, formHtml);

                        await context.Response.WriteAsync(htmlString);
                    }
                }
                else
                {
                    var htmlString = tableWriter.WriteTable(invoices, formHtml);
                   
                    await context.Response.WriteAsync(htmlString);
                }
            });
        });
        
        app.Map("/searchWeight", (appBuilder) =>
        {
            appBuilder.Run(async (context) =>
            {
                var cachedInvoices = context.RequestServices.GetService<CachedInvoices>();
                var subscriptions = cachedInvoices?.GetInvoices("kei");

                var tableWriter = new TableWriter();

                var formHtml = "<form method='post' action='/searchWeight'>" +
                               "<label for='name'>minWeight:</label>";
                                    

                if (context.Session.Keys.Contains("duration"))
                {
                    var weight = int.Parse(context.Session.GetString("weight") ?? string.Empty);

                    formHtml += $"<input type='number' name='weight' value='{weight}'><br><br>" +
                                "<input type='submit' value='Поиск'>" +
                                 "</form>";
                }
                else
                {
                    formHtml += "<input type='number' name='weight'><br><br>" +
                                "<input type='submit' value='Поиск'>" +
                                 "</form>";
                }

                if (context.Request.Method == "POST")
                {
                    var weight = int.Parse(context.Request.Form["weight"]);

                    context.Session.SetString("weight", weight.ToString());

                    if (subscriptions != null)
                    {
                        var subscriptionsByPublications = subscriptions.Where(s => s.Weight >= weight);

                        var htmlString = tableWriter.WriteTable(subscriptionsByPublications, formHtml);


                        await context.Response.WriteAsync(htmlString);
                    }
                }
                else
                {
                    var htmlString = tableWriter.WriteTable(subscriptions, formHtml);
                    await context.Response.WriteAsync(htmlString);
                }
            });
        });
        
        app.Run((context) =>
        {
            
            var cachedInvoices = context.RequestServices.GetService<CachedInvoices>();

            cachedInvoices?.AddInvoicesToCache("kei");

            var htmlString = "<HTML><HEAD><TITLE>Furniture</TITLE></HEAD>" +
                             "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                             "<BODY><H1>Главная</H1>";
            htmlString += "<H2>Данные записаны в кэш сервера</H2>";
            htmlString += "<BR><A href='/'>Главная</A></BR>";
            htmlString += "<BR><A href='/info'>Информация</A></BR>";
            htmlString += "<BR><A href='/invoices'>Все подписки</A></BR>";
            htmlString += "<BR><A href='/searchBySupplierName'>Поиск по имени</A></BR>";
            htmlString += "<BR><A href='/searchWeight'>Поиск по весу</A></BR>";
            htmlString += "</BODY></HTML>";

            return context.Response.WriteAsync(htmlString);

        });
        
        app.Run();
    }
    
}