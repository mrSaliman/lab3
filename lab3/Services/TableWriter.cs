using System.Globalization;
using lab3.Models;

namespace lab3.Services;

public class TableWriter
{
    public string WriteTable(IEnumerable<Invoice>? invoices, params object[] addons)
    {
        var htmlString = addons.Cast<string>()
            .Aggregate(
                "<HTML><HEAD><TITLE>Invoices</TITLE></HEAD>" +
                "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>",
                (current, addon) => current + addon);
        htmlString +="<BODY><H1>Invoices Table</H1><TABLE BORDER=1>";
        htmlString += "<TR>";
        htmlString += "<TH>InvoiceId</TH>";
        htmlString += "<TH>SupplierName</TH>";
        htmlString += "<TH>DeliveryDate</TH>";
        htmlString += "<TH>MaterialType</TH>";
        htmlString += "<TH>Price</TH>";
        htmlString += "<TH>Weight</TH>";
        htmlString += "</TR>";
        foreach (var invoice in invoices)
        {
            htmlString += "<TR>";
            htmlString += "<TD>" + invoice.InvoiceId + "</TD>";
            htmlString += "<TD>" + invoice.SupplierName + "</TD>";
            htmlString += "<TD>" + invoice.DeliveryDate + "</TD>";
            htmlString += "<TD>" + invoice.MaterialType + "</TD>";
            htmlString += "<TD>" + invoice.Price + "</TD>";
            htmlString += "<TD>" + invoice.Weight + "</TD>";
            htmlString += "</TR>";
        }
        htmlString += "</TABLE>";
        htmlString += "<BR><A href='/'>Main</A></BR>";
        htmlString += "</BODY></HTML>";

        return htmlString;
    }

}