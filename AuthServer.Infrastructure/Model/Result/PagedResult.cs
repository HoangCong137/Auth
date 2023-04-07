using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuthServer.Infrastructure.Model.Result
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPage => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PagedResult<TDto> ChangeType<TDto>(Func<T, TDto> cast)
        {
            return new PagedResult<TDto>
            {
                Items = Items.Select(cast).ToList(),
                PageIndex = PageIndex,
                PageSize = PageSize,
                TotalCount = TotalCount
            };
        }
    }
}
