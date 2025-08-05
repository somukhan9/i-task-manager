using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
    : IdentityDbContext<User, Role, int>(options);