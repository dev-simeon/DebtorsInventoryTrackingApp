using WebApi.Configurations;
using WebApi.Data;

namespace WebApi.Controllers;

public class ControllerParameters(IHttpContextAccessor accessor, ApplicationDbContext dbContext, Settings settings)
{
    public HttpContext? HttpContext { get; } = accessor.HttpContext;
    public ApplicationDbContext DbContext { get; } = dbContext;
    public Settings Settings { get; } = settings;
}
