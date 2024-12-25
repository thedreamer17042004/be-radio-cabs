using Microsoft.EntityFrameworkCore;
using radioCabs.Models;

namespace radioCabs.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Kiểm tra nếu dữ liệu đã tồn tại
                if (context.Users.Any())
                {
                    return; // DB đã được seed
                }

                // Thêm dữ liệu mẫu
                context.Users.AddRange(
                    new User
                    {
                        Username = "admin",
                        Password = "827ccb0eea8a706c4c34a16891f84e7b", // use md5 to hash //12345
                        Email = "admin@gmail.com",
                        Role = "ADMIN",
                        Images = null,
                        CreatedAt = DateTime.Now
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
