using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DotNETCoreDay.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

namespace DotNETCoreDay.Controllers
{
    public class CustomersController : Controller
    {
        readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;
        readonly IMemoryCache _memoryCache;
        readonly IDistributedCache _distributedCache;
        string _cacheKey;

        public CustomersController(IStringLocalizer<SharedResource> sharedResourceLocalizer, IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _sharedResourceLocalizer = sharedResourceLocalizer;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;

            #region Set Redis
            _cacheKey = "customers";
            List<Customer> customers = new List<Customer>();

            customers.Add(new Customer() { Id= 1, Name = "Gökhan" });
            customers.Add(new Customer() { Id = 2, Name = "Veli" });

            distributedCache.SetString(_cacheKey, JsonConvert.SerializeObject(customers));
            #endregion
        }

        public IActionResult Index()
        {
            CustomerModel customerModel = new CustomerModel
            {
              Customers =   JsonConvert.DeserializeObject<List<Customer>>(_distributedCache.GetString(_cacheKey))
            };
            
            return View(customerModel);
        }

        public IActionResult Detail(int id)
        {
            Customer customer = GetCustomer(id);

            return View(customer);
        }

        Customer GetCustomer(int id)
        {
            Customer customer;

            if (!_memoryCache.TryGetValue(id, out customer))
            {
                var users = JsonConvert.DeserializeObject<List<Customer>>(_distributedCache.GetString(_cacheKey));

                customer = users.FirstOrDefault(u => u.Id == id);

                var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

               _memoryCache.Set(id, customer, memoryCacheEntryOptions);
            }

            return customer;
        }
    }
}