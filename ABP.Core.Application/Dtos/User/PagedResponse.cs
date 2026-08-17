using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Core.Application.Dtos.User
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }

        public PagedResponse(IEnumerable<T> data, int totalRecords, int currentPage, int limit)
        {
            Data = data;
            TotalRecords = totalRecords;
            CurrentPage = currentPage;
            Limit = limit;
            TotalPages = (int)Math.Ceiling((double)totalRecords / limit);
        }
    }
}
