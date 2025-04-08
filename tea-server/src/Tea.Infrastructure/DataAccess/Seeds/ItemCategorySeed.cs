using Microsoft.EntityFrameworkCore;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.Seeds
{
    public class ItemCategorySeed
    {
        public static async Task SeedAsync(TeaContext context)
        {
            if (await context.ItemCategories.AnyAsync()) return;

            var s = new List<ItemCategory>
            {
                new ItemCategory(1, 1),
                new ItemCategory(1, 7),


                new ItemCategory(2, 1),
                new ItemCategory(2, 4),
                new ItemCategory(2, 2),

                new ItemCategory(3, 1),
                new ItemCategory(3, 5),
                new ItemCategory(3, 2),


                new ItemCategory(4, 1),


                new ItemCategory(5, 1),
                new ItemCategory(5, 5),
                new ItemCategory(5, 2),

                new ItemCategory(6, 1),
                new ItemCategory(6, 7),

                new ItemCategory(7, 1),

                new ItemCategory(8, 1),


                new ItemCategory(9, 1),
                new ItemCategory(9, 4),
                new ItemCategory(9, 2),

                new ItemCategory(10, 1),
                new ItemCategory(10, 7),

                new ItemCategory(11, 1),

                new ItemCategory(12, 8),

                new ItemCategory(13, 1),

                new ItemCategory(14, 8),

                new ItemCategory(15, 1),
                new ItemCategory(15, 4),
                new ItemCategory(15, 2),
                new ItemCategory(15, 5),


                new ItemCategory(16, 1),
                new ItemCategory(16, 5),
                new ItemCategory(16, 2),

                new ItemCategory(17, 1),
                new ItemCategory(17, 5),
                new ItemCategory(17, 2),

                new ItemCategory(18, 1),
                new ItemCategory(18, 5),
                new ItemCategory(18, 2),

                new ItemCategory(19, 3),

                new ItemCategory(20, 3),

            };

            context.ItemCategories.AddRange(s);
            await context.SaveChangesAsync();
        }
    }
}
