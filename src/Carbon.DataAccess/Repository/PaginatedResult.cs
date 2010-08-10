using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Carbon.Repository.Repository
{
    public class PaginatedResult<T> : List<T> where T : class
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedResult(IList<T> source, int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                throw new ArgumentException("The page index {i.e. current page }can not be set to zero.");

            if (pageIndex < 0)
                throw new ArgumentException("The page index {i.e. current page }can not be set to a negative number.");

            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            this.BuildPage(source);

            //this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }

        public bool HasPreviousPage
        {
            get
            {
                // start the page index at one instead of zero-based:
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                // inspect in case the first page number is sent as zero:
                // var index = PageIndex == 0 ? 1 : PageIndex + 1;
                return (PageIndex < TotalPages);
            }
        }

        private void BuildPage(IList<T> source)
        {
            // zero-based lists:
            var startMarker = (PageIndex - 1)*PageSize;
            var endMarker = (startMarker + PageSize) - 1;

            for (var index = startMarker; index <= endMarker; index++)
            {
                var item = source[index];
                this.Add(item);
            }
        }
    }
}