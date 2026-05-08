using Microsoft.EntityFrameworkCore;

namespace MyCosts.Infrastructure.Persistence;

public static class DbUpdateExceptionExtensions
{
    private const string PostgresUniqueViolationErrorCode = "23505";

    extension(DbUpdateException ex)
    {
        public bool IsUniqueConstraintViolation
            => ex.InnerException is Npgsql.PostgresException { SqlState: PostgresUniqueViolationErrorCode };
    }
}
