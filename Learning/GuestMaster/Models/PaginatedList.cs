using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace EventManagementSystem.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        //public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        //{
        //    var count = await source.CountAsync();
        //    var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        //    return new PaginatedList<T>(items, count, pageIndex, pageSize);
        //}
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source
                .ToListAsync(); // Fetch data to client side
            var paginatedItems = items
                .Select((item, index) => new { item, index = index + 1 })
                .Where(x => x.index > (pageIndex - 1) * pageSize && x.index <= pageIndex * pageSize)
                .Select(x => x.item)
                .ToList();
            return new PaginatedList<T>(paginatedItems, count, pageIndex, pageSize);
        }

    }
}