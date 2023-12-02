using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Entities;
using Pronia.ViewModels;

namespace Pronia.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;

        public LayoutService(AppDbContext context,IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task<Dictionary<string,string>> GetSettingsAsync()
        {
            Dictionary<string, string> settings =  await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);

            return settings;


        }

        

    }
}
