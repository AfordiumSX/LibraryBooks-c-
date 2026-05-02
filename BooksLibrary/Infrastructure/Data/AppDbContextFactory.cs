using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BooksLibrary.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql("Host=АДРЕС;Port=5432;Database=НАЗВАНИЕ_БАЗЫ_ДАННЫХ;Username=ИМЯ_ПОЛЬЗОВАТЕЛЯ;Password=ПАРОЛЬ")
                .Options;
            return new AppDbContext(options);
        }
    }
}
