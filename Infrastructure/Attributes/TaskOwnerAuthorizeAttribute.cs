using Dapper;
using Infrastructure.Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Security.Claims;
using TaskDomain = Domain.Entities.Task;


namespace Infrastructure.Attributes;

public class TaskOwnerAuthorizeAttribute(string id = "taskId") : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var routeParams = context.RouteData.Values;
        var queryParams = context.HttpContext.Request.Query;

        if (string.IsNullOrEmpty(userId) || userId.Equals("0"))
        {
            context.Result = new ForbidResult();
            return;
        }

        var taskId = routeParams.GetValueOrDefault(id)?.ToString() ?? queryParams[id].ToString() ?? "0";

        var factory = (context.HttpContext.RequestServices.GetService(typeof(ISqlConnectionFactory)) as ISqlConnectionFactory)!;
        var connection = await factory.CreateConnectionAsync();

        var sql = @"SELECT * FROM dbo.Tasks WHERE [Id] = @Id";

        var task = await connection.QueryFirstOrDefaultAsync<TaskDomain>(new CommandDefinition(
                sql, new { Id = taskId }, commandType: CommandType.Text, commandTimeout: 60, cancellationToken: cancellationToken
            )
        );

        // This means it is create task get/post route
        if (task is null) return;

        if (task.UserId.ToString() != userId)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
