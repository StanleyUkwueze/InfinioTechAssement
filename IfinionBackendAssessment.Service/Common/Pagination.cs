using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.Common
{
    public static class Pagination
    {
        public static PagedResponse<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize) where T : class
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;

            var pagedResult = new PagedResponse<T>();
            pagedResult.CurrentPage = pageNumber;
            pagedResult.PageSize = pageSize;
            pagedResult.TotalRecords = query.Count();

            var pageCount = (double)pagedResult.TotalRecords / pageSize;
            pagedResult.TotalPages = (int)Math.Ceiling(pageCount);

            var pagesToSkip = (pageNumber - 1) * pageSize;

            pagedResult.Result = query.Skip(pagesToSkip).Take(pageSize).ToList();
            return pagedResult;
        }
    }
}
