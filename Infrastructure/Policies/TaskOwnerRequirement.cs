using Dapper;
using Infrastructure.Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Security.Claims;
using TaskDomain = Domain.Entities.Task;


namespace Infrastructure.Policies;

public class TaskOwnerRequirement : IAuthorizationRequirement { }

public class TaskOwnerHandler : AuthorizationHandler<TaskOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TaskOwnerRequirement requirement)
    {
        CancellationToken cancellationToken = new CancellationTokenSource().Token;

        var httpContext = (HttpContext)context.Resource!;

        var id = "taskId";

        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var routeParams = httpContext.Request.RouteValues;
        var queryParams = httpContext.Request.Query;

        if (string.IsNullOrEmpty(userId) || userId.Equals("0"))
        {
            context.Fail();
            return;
        }

        var taskId = routeParams.GetValueOrDefault(id)?.ToString() ?? queryParams[id].ToString() ?? "0";

        var factory = (httpContext.RequestServices.GetService(typeof(ISqlConnectionFactory)) as ISqlConnectionFactory)!;
        var connection = await factory.CreateConnectionAsync();

        var sql = @"SELECT * FROM dbo.Tasks WHERE [Id] = @Id";

        var task = await connection.QueryFirstOrDefaultAsync<TaskDomain>(new CommandDefinition(
                sql, new { Id = taskId }, commandType: CommandType.Text, commandTimeout: 60, cancellationToken: cancellationToken
            )
        );

        // This means it is create task get/post route
        if (task is null)
        {
            context.Succeed(requirement);
            return;
        }

        if (task.UserId.ToString() != userId)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
