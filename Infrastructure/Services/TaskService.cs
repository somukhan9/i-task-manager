using Application.Contracts;
using Dapper;
using Domain.Entities;
using Infrastructure.Dapper;
using System.Data;
using Task = Domain.Entities.Task;

namespace Infrastructure.Services;

public class TaskService(
    ISqlConnectionFactory factory
    //UserManager<User> userManager,
    //SignInManager<User> signInManager,
    ) : ITaskService
{
    public async Task<IEnumerable<Task>> GetTasksAsync(int userId, string? query, CancellationToken cancellationToken)
    {
        var sql = @"
                    SELECT t.*, u.[Name], u.[UserName], u.[Email]
                    FROM [dbo].[Tasks] t
                    INNER JOIN [dbo].[AspNetUsers] u ON u.Id = t.UserId
                    WHERE t.[Finished] != 1 and u.Id = @UserId 
                    ORDER BY t.[Priority] ASC;
                ";

        using var connection = await factory.CreateConnectionAsync();

        var rawResult =
            await connection.QueryAsync<dynamic>(new CommandDefinition(
                    sql, new { UserId = userId }, commandType: CommandType.Text, commandTimeout: 60,
                    cancellationToken: cancellationToken
                )
            );

        var result = rawResult.Select(r => new Task()
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            AttachmentUrl = r.AttachmentUrl,
            Finished = r.Finished,
            FinishedDate = r.FinishedDate,
            CreatedDate = r.CreatedDate,
            UpdatedDate = r.UpdatedDate,
            Priority = r.Priority,
            User = new User()
            {
                Id = r.UserId,
                Name = r.Name,
                UserName = r.Username,
                Email = r.Email,
            }
        });

        return result;
    }

    public async Task<Task?> GetTaskByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var connection = await factory.CreateConnectionAsync();

        var sql = @"
            SELECT *
            FROM [dbo].[Tasks]
            WHERE [Id] = @Id
        ";
        var result = await connection.QueryFirstOrDefaultAsync<Task>(new CommandDefinition(sql, new { Id = id },
            commandType: CommandType.Text, commandTimeout: 60, cancellationToken: cancellationToken));

        return result;
    }

    public async Task<int> AddTaskAsync(Task task, CancellationToken cancellationToken)
    {
        using var connection = await factory.CreateConnectionAsync();

        //var sql = @"
        //    INSERT INTO [dbo].[Tasks]
        //    ([Title], [Description], [AttachmentUrl], [CreatedDate], [Priority], [UserId])
        //    VALUES
        //    (@Title, @Description, @AttachmentUrl, @CreatedDate, (COALESCE(
        //    SELECT MAX(MaxPriority) AS MaxPriority
        //        FROM (
        //         SELECT COALESCE(MAX([Priority]), 0) AS MaxPriority FROM [dbo].[Tasks]
        //         UNION ALL
        //         SELECT COALESCE(MAX([Priority]), 0) AS MaxPriority FROM [dbo].[TasksLog]
        //        ) AS Combined
        //    , 0) + 1), @UserId);
        //    SELECT CAST(SCOPE_IDENTITY() as int);
        //";

        var sql = @"
            DECLARE @NextPriority INT;

            SELECT @NextPriority = COALESCE(MAX(MaxPriority), 0) + 1
            FROM (
                SELECT COALESCE(MAX([Priority]), 0) AS MaxPriority FROM [dbo].[Tasks]
                UNION ALL
                SELECT COALESCE(MAX([Priority]), 0) AS MaxPriority FROM [dbo].[TasksLog]
            ) AS Combined;

            INSERT INTO [dbo].[Tasks]
            ([Title], [Description], [AttachmentUrl], [CreatedDate], [Priority], [UserId])
            VALUES
            (@Title, @Description, @AttachmentUrl, @CreatedDate, @NextPriority, @UserId);

            SELECT CAST(SCOPE_IDENTITY() as int);
        ";


        var taskId = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new
        {
            task.Title,
            task.Description,
            task.AttachmentUrl,
            Finished = 0,
            CreatedDate = DateTime.UtcNow,
            task.UserId
        }, commandType: CommandType.Text, commandTimeout: 60));

        return taskId > 0 ? taskId : throw new Exception("Error occured while creating task.");
    }

    public async Task<int> UpdateTaskAsync(Task task, CancellationToken cancellationToken)
    {
        var connection = await factory.CreateConnectionAsync();

        var sql = @$"
            UPDATE [dbo].[Tasks] SET
              [Title] = @Title
            , [Description] = @Description
            , [Finished] = @Finished
            , [FinishedDate] = (CASE WHEN @Finished = 1 THEN GETDATE() ELSE NULL END)
            , [UpdatedDate] = GETDATE()
            WHERE [Id] = @Id;
        ";

        var rowsAffected = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                task.Id,
                task.Title,
                task.Description,
                task.Finished,
            }, commandType: CommandType.Text, commandTimeout: 60));

        return rowsAffected > 0 ? rowsAffected : throw new Exception("Error occured while updating task.");
    }

    public Task<bool> DeleteTaskAsync(int taskId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteFinishedTask(CancellationToken cancellationToken)
    {
        using var connection = await factory.CreateConnectionAsync();

        var sql = @"EXEC [dbo].[sp_ClearFinishedTasks]";

        var result = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { }, commandType: CommandType.Text, commandTimeout: 60));

        Console.WriteLine($"DeleteFinishedTask (Number of Rows Executed :: {result})");

        return result > 0;
    }

    public Task<bool> UpgradeTaskPriority(int taskId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DowngradeTaskPriority(int taskId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}