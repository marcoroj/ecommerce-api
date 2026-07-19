using ECommerce.Api.DTOs;

namespace ECommerce.Api.Utils
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>
            (this IQueryable<T> queryable, PaginationDto paginationDto)
        {
            return queryable.Skip((paginationDto.Page - 1) * paginationDto.Size)
                .Take(paginationDto.Size);
        }
    }
}
