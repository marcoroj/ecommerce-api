using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Utils
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarPaginacionHeader<T>
            (this HttpContext httpContext,IQueryable<T> quearyable)
        {
            if(httpContext is null)
            {
                throw new ArgumentNullException(nameof (httpContext));
            }

            double cantidad=await quearyable.CountAsync();
            httpContext.Response.Headers.Add("X-Total-Count",cantidad.ToString());

        }
    }
}
