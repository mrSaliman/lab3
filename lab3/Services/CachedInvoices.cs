using lab3.Models;
using Microsoft.Extensions.Caching.Memory;

namespace lab3.Services;

public class CachedInvoices
{
    private readonly AcmeDataContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly int _saveTime;
    
    public CachedInvoices(AcmeDataContext dbContext, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
        _saveTime = 2 * 12 + 240;
    }
    
    public void AddInvoicesToCache(string key, int rowsNumber = 100)
    {
        if (!_memoryCache.TryGetValue(key, out IEnumerable<Invoice>? cachedInvoices))
        {
            cachedInvoices = _dbContext.Invoices.Take(rowsNumber).ToList();

            _memoryCache.Set(key, cachedInvoices, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_saveTime)
            });
            Console.WriteLine("Таблица Invoice занесена в кеш");
        }
        else
        {
            Console.WriteLine("Таблица Invoice уже находится в кеше");
        }
    }
    public IEnumerable<Invoice> GetInvoices(string key, int rowsNumber = 100)
    {
        IEnumerable<Invoice> invoices;
        if (_memoryCache.TryGetValue(key, out invoices)) return invoices;
        invoices = _dbContext.Invoices.Take(rowsNumber).ToList();
        _memoryCache.Set(key, invoices,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(_saveTime)));
        return invoices;
    }
}