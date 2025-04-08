using Microsoft.EntityFrameworkCore;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.Seeds
{
    public class CategorySeed
    {
        public static async Task SeedAsync(TeaContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var Categorys = new List<Category>
            {
                new Category
                {
                    Name = "Thức uống",
                    Slug = "thuc-uong",
                    Description = @"Các món nước giải khát v.v"
                },
                 new Category
                {
                    Name = "Trà",
                    Slug = "tra",
                    Description = @"Các loại trà độc lạ v.v",
                    ParentId = 1
                },
                 new Category
                {
                    Name = "Topping",
                    Slug = "topping",
                    Description = @"Các món ăn kèm thêm với món nước v.v"
                },

                 new Category
                {
                    Name = "Trà trái cây",
                    Slug = "tra-trai-cay",
                    Description = @"Các loại trà kết hợp cùng với trái cây",
                    ParentId = 2
                },
                 new Category
                {
                    Name = "Trà sữa",
                    Slug = "tra-sua",
                    Description = @"Các loại trà sữa phổ biến",
                    ParentId = 2
                },
                  new Category
                {
                    Name = "Trà nguyên chất",
                    Slug = "tra-nguyen-chat",
                    Description = @"Thành phần chủ yếu là trà",
                    ParentId = 2
                },

                   new Category
                {
                    Name = "Latte",
                    Slug = "latte",
                    Description = @"Các loại latte mới lạ",
                    ParentId = 1
                },

                   new Category
                {
                    Name = "Kem",
                    Slug = "kem",
                    Description = @"Các loại kem mát lạnh",
                },

            };

            context.Categories.AddRange(Categorys);
            await context.SaveChangesAsync();
        }
    }
}
