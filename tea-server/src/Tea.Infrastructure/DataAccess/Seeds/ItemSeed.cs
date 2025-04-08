using Microsoft.EntityFrameworkCore;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.Seeds
{
    public class ItemSeed
    {
        public static async Task SeedAsync(TeaContext context)
        {
            if (await context.Items.AnyAsync()) return;

            var Items = new List<Item>
            {
                new Item
                {
                    Name = "Matcha Đá Xay",
                    Slug = "matcha-da-xay",
                    Description = "Matcha xay cùng với sữa, kết hợp với lớp Kem sữa phía trên.",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096670/Matcha-%C4%91%C3%A1-xay-2_njen4d.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "M",
                            Price = 70000
                        },
                         new Size
                        {
                            Name = "L",
                            Price = 75000
                        }
                    },
                },

                new Item
                {
                    Name = "Trà Oolong Vải",
                    Slug = "tra-oolong-vai",
                    Description = "Trà Oolong thơm ngon hòa cùng với vị Vải thanh mát. (Có nóng và lạnh)",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096669/Oolong-v%E1%BA%A3i-2_yvxvt0.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "M",
                            Price = 54000
                        },
                        new Size
                        {
                            Name = "L",
                            Price = 62000
                        }
                    }
                },

                new Item
                {
                    Name = "Strawberry White Choco",
                    Slug = "strawberry-white-choco",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096669/Strawberry-Earl-grey-latte_umaa81.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "M",
                            Price = 70000
                        },
                    }
                },

                new Item
                {
                    Name = "Okinawa Kem Sữa Đá Xay",
                    Slug = "okinawa-kem-sua-da-xay",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096668/Okinawa-Oreo-Cream-Milk-Tea_mgo0xr.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "M",
                            Price = 70000
                        },
                    }
                },

                new Item
                {
                    Name = "Trà Sữa Okinawa Oreo",
                    Slug = "tra-sua-okinawa-oreo",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096668/Okinawa-Milk-Foam-Smoothie_oiwjmy.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "S",
                            Price = 57000
                        },
                        new Size
                        {
                            Name = "M",
                            Price = 62000
                        },
                        new Size
                        {
                            Name = "L",
                            Price = 70000
                        },
                    }
                },

                 new Item
                {
                    Name = "Matcha Latte Xoài",
                    Slug = "matcha-latte-xoai",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096667/Mango-Matcha-Latte_s91j5r.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "M",
                            Price = 69000
                        },
                    }
                },

                  new Item
                {
                    Name = "Okinawa Latte",
                    Slug = "okinawa-latte",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096667/Hinh-Web-OKINAWA-LATTE_o87df3.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "S",
                            Price = 57000
                        },
                         new Size
                        {
                            Name = "M",
                            Price = 65000
                        },
                          new Size
                        {
                            Name = "L",
                            Price = 73000
                        },
                           new Size
                        {
                            Name = "Nóng",
                            Price = 57000
                        },
                    }
                },

                  new Item
                {
                    Name = "Mango Sago",
                    Slug = "mango-sago",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096667/MANGO-SOGO-WEB_c6uw9d.png",
                    Sizes = new List<Size>
                    {
                         new Size
                        {
                            Name = "M",
                            Price = 65000
                        },
                        
                    }
                },

                   new Item
                {
                    Name = "Trà Alisan Trái Cây",
                    Slug = "tra-alisan-trai-cay",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096667/ALISAN-TRA%CC%81I-CA%CC%82Y_aja0gy.png",
                    Sizes = new List<Size>
                    {
                         new Size
                        {
                            Name = "M",
                            Price = 54000
                        },
                         new Size
                        {
                            Name = "L",
                            Price = 65000
                        },

                    }
                },

                    new Item
                {
                    Name = "Matcha Latte",
                    Slug = "matcha-latte",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096668/Matcha-Tea-Latte-with-Matcha-Jelly_txod0t.png",
                    Sizes = new List<Size>
                    {
                         new Size
                        {
                            Name = "M",
                            Price = 59000
                        },
                         new Size
                        {
                            Name = "L",
                            Price = 67000
                        },

                    }
                },

                    new Item
                {
                    Name = "Matcha Okinawa",
                    Slug = "matcha-okinawa",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096668/OKINAWA-8_dzg5ha.png",
                    Sizes = new List<Size>
                    {
                         new Size
                        {
                            Name = "M",
                            Price = 65000
                        },
                         new Size
                        {
                            Name = "L",
                            Price = 75000
                        },

                    }
                },

                      new Item
                {
                    Name = "Kem Trà Sữa & Trân Châu Đen",
                    Slug = "kem-tra-sua-va-tran-chau-den",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096667/kem-tc_nzwqol.png",
                    Sizes = new List<Size>
                    {
                         new Size
                        {
                            Name = "Thường",
                            Price = 35000
                        },
                    }
                },

                       new Item
                {
                    Name = "Khoai Môn Đá Xay",
                    Slug = "khoai-mon-da-xoay",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096667/Khoai-m%C3%B4n-%C4%91%C3%A1-xay-2_zk2nsp.png",
                    Sizes = new List<Size>
                    {
                         new Size
                        {
                            Name = "M",
                            Price = 68000
                        },
                    }
                },

                       new Item
                {
                    Name = "Kem Trà Sữa Oolong",
                    Slug = "kem-tra-sua-oolong",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096666/kem_r4ro5p.png",
                    Sizes = new List<Size>
                    {
                         new Size
                        {
                            Name = "Thường",
                            Price = 30000
                        },
                    }
                },

                         new Item
                {
                    Name = "Trà Sữa Trà Đen/ Oolong/ Earl Grey Okinawa",
                    Slug = "tra-sua-den",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096666/Hinh-Web-OKINAWA-TR%C3%80-S%E1%BB%AEA_kumrpx.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "S",
                            Price = 57000
                        },
                         new Size
                        {
                            Name = "M",
                            Price = 62000
                        },
                         new Size
                        {
                            Name = "L",
                            Price = 70000
                        },
                    }
                },
                         new Item
                {
                    Name = "Trà Đen Đào",
                    Slug = "tra-den-dao",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096665/%C4%90en-%C4%91%C3%A0o-2_craq5o.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "Nóng",
                            Price = 54000
                        },
                         new Size
                        {
                            Name = "M",
                            Price = 62000
                        },
                         new Size
                        {
                            Name = "L",
                            Price = 54000
                        },
                    }
                },

                         new Item
                {
                    Name = "Trà Sữa Earl Grey",
                    Slug = "tra-sua-ear-grey",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096662/Tra-sua-dau_zcvffd.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "S",
                            Price = 48000
                        },
                         new Size
                        {
                            Name = "M",
                            Price = 53000
                        },
                         new Size
                        {
                            Name = "L",
                            Price = 61000
                        },
                    }
                },

                          new Item
                {
                    Name = "Trà Sữa Trân Châu Đen",
                    Slug = "tra-sua-tran-chau-den",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096661/Tr%C3%A0-s%E1%BB%AFa-Tr%C3%A2n-ch%C3%A2u-%C4%91en-1_cyesmt.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "S",
                            Price = 47000
                        },
                         new Size
                        {
                            Name = "M",
                            Price = 55000
                        },
                         new Size
                        {
                            Name = "L",
                            Price = 63000
                        },
                    }
                },

                          new Item
                {
                    Name = "Trân Châu Trắng",
                    Slug = "tran-chau-trang",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096660/Tr%C3%A2n-Ch%C3%A2u-Tr%E1%BA%AFng_tkp6c2.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "Bình thường",
                            Price = 5000
                        },
                         
                    }
                },

                          new Item
                {
                    Name = "Sương Sáo",
                    Slug = "suong-sao",
                    ImgUrl = @"https://res.cloudinary.com/di9zisqnz/image/upload/v1744096658/S%C6%B0%C6%A1ng-s%C3%A1o_st1dde.png",
                    Sizes = new List<Size>
                    {
                        new Size
                        {
                            Name = "1 Cái",
                            Price = 10000
                        },

                    }
                },

            };

            context.Items.AddRange(Items);
            await context.SaveChangesAsync();
        }
    }
}
