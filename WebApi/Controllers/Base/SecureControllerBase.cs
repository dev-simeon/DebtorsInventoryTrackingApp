using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public abstract class SecureControllerBase : ControllerBase
    {
        private readonly ClaimsPrincipal _principal;

        protected ApplicationDbContext Context { get; }

        public SecureControllerBase(ControllerParameters services)
        {
            if (services.HttpContext == null)
                throw new Exception("Request.HttpContext is NULL");

            _principal = services.HttpContext.User
                ?? throw new Exception("Request.HttpContext.User is NULL");

            Context = services.DbContext;
        }

        protected async Task<IEnumerable<TResponse>> ToPagedListAsync<TModel, TResponse>(
            IQueryable<TModel> query,
            PaginationQuery? pagination,
            Func<TModel, TResponse> projection)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query cannot be null.");

            var page = pagination?.PageNumber ?? 1;
            var pageSize = pagination?.PageSize ?? await query.CountAsync();

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            // Set pagination metadata in response headers
            HttpContext.Response.Headers["X-Total-Count"] = totalItems.ToString();
            HttpContext.Response.Headers["X-Total-Pages"] = totalPages.ToString();
            HttpContext.Response.Headers["X-Current-Page"] = page.ToString();
            HttpContext.Response.Headers["X-Page-Size"] = pageSize.ToString();

            return items.Select(projection);
        }


        protected string CurrentUserId => _principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value
            ?? throw new Exception("User ID claim not found");

        protected string CurrentUserEmail => _principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value
            ?? throw new Exception("Email claim not found");
    }

    public class PaginationQuery
    {
        public PaginationQuery() { }

        [Range(1, 1000, ErrorMessage = "Page number must be between 1 and 1000.")]
        public int PageNumber { get; set; } = 1;

        [Range(5, 500, ErrorMessage = "Page size must be between 5 and 500.")]
        public int PageSize { get; set; } = 20;
    }

}
