using System.Drawing;
using OfficeOpenXml;
using Tea.Application.DTOs.Items;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;
using Tea.Infrastructure.Interfaces;

namespace Tea.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        private readonly TeaContext _context;
        private readonly IUnitOfWork _unit;
        public ExcelService(TeaContext context, IUnitOfWork unit)
        {
            ExcelPackage.License.SetNonCommercialPersonal("<K TEA>");
            _context = context;
            _unit = unit;
        }

        public async Task<byte[]> Export<T>(List<T> dataItems) where T : class, new()
        {
            if (!dataItems.Any())
            {
                throw new BadRequestException("Dữ liệu bị trống");
            }

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workbook = package.Workbook.Worksheets.Add("Items");


                T obj = new T();

                var properties = obj.GetType().GetProperties();

                // patch data
                for (int row = 0; row < dataItems.Count(); row++)
                {
                    for (int col = 0; col < properties.Count(); col++)
                    {
                        var rowData = dataItems[row];
                        workbook.Cells[row + 1, col + 1].Value = rowData.GetType().GetProperty(properties[col].Name).GetValue(rowData);
                    }
                }

                // create header
                for (int i = 0; i < properties.Count(); i++)
                {
                    workbook.Cells[1, i + 1].Value = properties[i].Name;
                    workbook.Column(i + 1).AutoFit();
                    workbook.Cells[1, i + 1].Style.Font.Bold = true;
                    workbook.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // có dòng này thì mới định nghĩa được màu của dòng
                    workbook.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5a90e0"));
                }

                await package.SaveAsync();

            }

            return stream.ToArray();
        }

        public async Task<byte[]> ExportTemplateUpdateItemAsync(List<int> ids)
        {
            var items = new List<Item>();

            if (ids != null && ids.Count > 0)
            {
                items = (await _unit.Item.FindAllAsync(x => ids.Contains(x.Id), false)).ToList();
            }

            if (items == null || items.Count == 0)
            {
                items = (await _unit.Item.FindAllAsync(null, false)).ToList();
            }

            using (var stream = new MemoryStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Items");

                    // Create header
                    worksheet.Cells[1, 1].Value = "Id";
                    worksheet.Cells[1, 2].Value = "Name";
                    worksheet.Cells[1, 3].Value = "Slug";
                    worksheet.Cells[1, 4].Value = "ImgUrl";
                    worksheet.Cells[1, 5].Value = "PublicId";
                    worksheet.Cells[1, 6].Value = "Description";
                    worksheet.Cells[1, 7].Value = "IsPublished";
                    worksheet.Cells[1, 8].Value = "IsFeatured";
                    worksheet.Cells[1, 9].Value = "IsDeleted";
                    worksheet.Cells[1, 10].Value = "CategoryList";

                    // Format header
                    for (int i = 1; i <= 10; i++)
                    {
                        worksheet.Cells[1, i].Style.Font.Bold = true;
                        worksheet.Column(i).AutoFit();
                        worksheet.Cells[1, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5a90e0"));
                    }

                    // patch data
                    int row = 2;
                    foreach (var item in items)
                    {
                        worksheet.Cells[row, 1].Value = item.Id;
                        worksheet.Cells[row, 2].Value = item.Name;
                        worksheet.Cells[row, 3].Value = item.Slug;
                        worksheet.Cells[row, 4].Value = item.ImgUrl;
                        worksheet.Cells[row, 5].Value = item.PublicId;
                        worksheet.Cells[row, 6].Value = item.Description;
                        worksheet.Cells[row, 7].Value = item.IsPublished;
                        worksheet.Cells[row, 8].Value = item.IsFeatured;
                        worksheet.Cells[row, 9].Value = item.IsDeleted;
                        worksheet.Cells[row, 10].Value = item.ItemCategories != null
                            ? string.Join(", ", item.ItemCategories.Select(x => x.CategoryId.ToString()))
                            : string.Empty;

                        row++;
                    }

                    await package.SaveAsync();
                }

                return stream.ToArray();
            }
        }

        public async Task<ImportResult> ImportUpdateItemsFromExcelAsync(Stream fileStream)
        {
            var result = new ImportResult();

            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                throw new BadRequestException("Excel file does not contain any worksheets");

            // Validate headers
            ValidateUpdateItemExcelHeaders(worksheet);

            await _unit.BeginTransactionAsync();

            try
            {
                // Start from row 2 (skip header)
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {

                    // Read item data
                    var itemId = worksheet.Cells[row, 1].GetValue<int?>();
                    var itemName = worksheet.Cells[row, 2].GetValue<string>();
                    var slug = worksheet.Cells[row, 3].GetValue<string>();
                    var imgUrl = worksheet.Cells[row, 4].GetValue<string>();
                    var publicId = worksheet.Cells[row, 5].GetValue<string>();
                    var description = worksheet.Cells[row, 6].GetValue<string>();
                    var isPublished = worksheet.Cells[row, 7].GetValue<bool>();
                    var isFeatured = worksheet.Cells[row, 8].GetValue<bool>();
                    var isDeleted = worksheet.Cells[row, 9].GetValue<bool>();

                    // Validate item data
                    if (!itemId.HasValue)
                    {
                        result.Errors.Add($"Id của item không được trống (dòng {row} trong excel).");
                        return result;
                    }


                    if (string.IsNullOrWhiteSpace(itemName))
                    {
                        result.Errors.Add($"Name của item không được trống (dòng {row} trong excel).");
                        return result;
                    }

                    if (string.IsNullOrWhiteSpace(slug))
                    {
                        result.Errors.Add($"Slug của item không được trống (dòng {row} trong excel).");
                        return result;
                    }


                    if (string.IsNullOrWhiteSpace(imgUrl))
                    {
                        result.Errors.Add($"ImgUrl của item không được trống (dòng {row} trong excel).");
                        return result;
                    }


                    // Read categories
                    var categoryListStr = worksheet.Cells[row, 10].GetValue<string>();
                    var categoryIds = new List<int>();

                    if (!string.IsNullOrEmpty(categoryListStr))
                    {
                        foreach (var idStr in categoryListStr.Split(','))
                        {
                            if (int.TryParse(idStr.Trim(), out var categoryId))
                            {
                                categoryIds.Add(categoryId);
                            }
                            else
                            {
                                result.Errors.Add($"Không đúng định dạng CategoryList (định dạng đúng: 1, 2, 3) (dòng {row} trong excel).");
                                return result;
                            }
                        }
                    }

                    // Verify categories exist
                    if (categoryIds.Any())
                    {
                        var existingCategories = await _unit.Category.FindAllAsync(c => categoryIds.Contains(c.Id));
                        var missingCategories = categoryIds.Except(existingCategories.Select(c => c.Id)).ToList();

                        if (missingCategories.Any())
                        {
                            result.Errors.Add($"Category không tồn tại ({string.Join(",", missingCategories)}) (dòng {row} trong excel).");
                            return result;
                        }
                    }

                    // update item
                    var item = await _unit.Item.FindAsync(x => x.Id == itemId.Value, true);

                    if (item == null)
                    {
                        result.Errors.Add($"Item có id: {itemId} không tồn tại (dòng {row} trong excel).");
                        return result;
                    }

                    // Cập nhật từng thuộc tính
                    item.Name = itemName;
                    item.Slug = slug;
                    item.ImgUrl = imgUrl;
                    item.Description = description;
                    item.IsPublished = isPublished;
                    item.IsFeatured = isFeatured;
                    item.PublicId = publicId;
                    item.IsDeleted = isDeleted;

                    // Xử lý categories
                    var existingCategoryIds = item.ItemCategories.Select(ic => ic.CategoryId).ToList();

                    // Xóa những category không còn trong danh sách mới
                    item.ItemCategories.RemoveAll(ic => !categoryIds.Contains(ic.CategoryId));

                    // Thêm những category mới
                    foreach (var categoryId in categoryIds.Except(existingCategoryIds))
                    {
                        item.ItemCategories.Add(new ItemCategory(item.Id, categoryId));
                    }

                    result.RowChange++;
                }

                if (await _unit.SaveChangesAsync())
                {
                    await _unit.CommitTransactionAsync();
                    return result;
                }

                throw new BadRequestException($"Lưu thay đổi thất bại");
            }
            catch
            {
                await _unit.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task<byte[]> ExportTemplateAddItemAsync()
        {
            var item = await _unit.Item.FindAsync(null, true);


            using (var stream = new MemoryStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Items");

                    // Create header
                    worksheet.Cells[1, 1].Value = "Id";
                    worksheet.Cells[1, 2].Value = "Name";
                    worksheet.Cells[1, 3].Value = "Slug";
                    worksheet.Cells[1, 4].Value = "ImgUrl";
                    worksheet.Cells[1, 5].Value = "PublicId";
                    worksheet.Cells[1, 6].Value = "Description";
                    worksheet.Cells[1, 7].Value = "IsPublished";
                    worksheet.Cells[1, 8].Value = "IsFeatured";
                    worksheet.Cells[1, 9].Value = "IsDeleted";
                    worksheet.Cells[1, 10].Value = "CategoryList";
                    worksheet.Cells[1, 11].Value = "SizeName";
                    worksheet.Cells[1, 12].Value = "Price";
                    worksheet.Cells[1, 13].Value = "NewPrice";
                    worksheet.Cells[1, 14].Value = "SizeDescription";
                    worksheet.Cells[1, 15].Value = "IsDeletedSize";

                    // Format header
                    for (int i = 1; i <= 15; i++)
                    {
                        worksheet.Cells[1, i].Style.Font.Bold = true;
                        worksheet.Column(i).AutoFit();
                        worksheet.Cells[1, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#5a90e0"));
                        if (i >= 11)
                            worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#09a779"));
                    }

                    // patch data
                    int row = 2;
                    foreach (var size in item.Sizes)
                    {
                        worksheet.Cells[row, 1].Value = item.Id;
                        worksheet.Cells[row, 2].Value = item.Name;
                        worksheet.Cells[row, 3].Value = item.Slug;
                        worksheet.Cells[row, 4].Value = item.ImgUrl;
                        worksheet.Cells[row, 5].Value = item.PublicId;
                        worksheet.Cells[row, 6].Value = item.Description;
                        worksheet.Cells[row, 7].Value = item.IsPublished;
                        worksheet.Cells[row, 8].Value = item.IsFeatured;
                        worksheet.Cells[row, 9].Value = item.IsDeleted;
                        worksheet.Cells[row, 10].Value = item.ItemCategories != null
                            ? string.Join(", ", item.ItemCategories.Select(x => x.CategoryId.ToString()))
                            : string.Empty;

                        worksheet.Cells[row, 11].Value = size.Name;
                        worksheet.Cells[row, 12].Value = size.Price;
                        worksheet.Cells[row, 13].Value = size.NewPrice;
                        worksheet.Cells[row, 14].Value = size.Description;
                        worksheet.Cells[row, 15].Value = size.IsDeleted;


                        row++;
                    }

                    await package.SaveAsync();
                }

                return stream.ToArray();
            }
        }


        public async Task<ImportResult> ImportAddItemsFromExcelAsync(Stream fileStream)
        {
            var result = new ImportResult();

            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                throw new BadRequestException("Excel file does not contain any worksheets");

            // Validate headers
            ValidateAddItemExcelHeaders(worksheet);

            var itemsToProcess = new Dictionary<int, Item>();

            await _unit.BeginTransactionAsync();

            try
            {
                // Start from row 2 (skip header)
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {

                    // Read item data
                    var itemId = worksheet.Cells[row, 1].GetValue<int>(); // id ở đây chỉ là con số để làm cờ (cho biết item này có bao nhiêu size)
                    var itemName = worksheet.Cells[row, 2].GetValue<string>();
                    var slug = worksheet.Cells[row, 3].GetValue<string>();
                    var imgUrl = worksheet.Cells[row, 4].GetValue<string>();
                    var publicId = worksheet.Cells[row, 5].GetValue<string>();
                    var description = worksheet.Cells[row, 6].GetValue<string>();
                    var isPublished = worksheet.Cells[row, 7].GetValue<bool>();
                    var isFeatured = worksheet.Cells[row, 8].GetValue<bool>();
                    var isDeleted = worksheet.Cells[row, 9].GetValue<bool>();

                    if (string.IsNullOrWhiteSpace(itemName))
                    {
                        result.Errors.Add($"Name của item không được trống (dòng {row} trong excel).");
                        return result;
                    }

                    if (string.IsNullOrWhiteSpace(slug))
                    {
                        result.Errors.Add($"Slug của item không được trống (dòng {row} trong excel).");
                        return result;
                    }


                    if (string.IsNullOrWhiteSpace(imgUrl))
                    {
                        result.Errors.Add($"ImgUrl của item không được trống (dòng {row} trong excel).");
                        return result;
                    }


                    // Read categories
                    var categoryListStr = worksheet.Cells[row, 10].GetValue<string>();
                    var categoryIds = new List<int>();

                    if (!string.IsNullOrEmpty(categoryListStr))
                    {
                        foreach (var idStr in categoryListStr.Split(','))
                        {
                            if (int.TryParse(idStr.Trim(), out var categoryId))
                            {
                                categoryIds.Add(categoryId);
                            }
                            else
                            {
                                result.Errors.Add($"Không đúng định dạng CategoryList (định dạng đúng: 1, 2, 3) (dòng {row} trong excel).");
                                return result;
                            }
                        }
                    }

                    // Verify categories exist
                    if (categoryIds.Any())
                    {
                        var existingCategories = await _unit.Category.FindAllAsync(c => categoryIds.Contains(c.Id));
                        var missingCategories = categoryIds.Except(existingCategories.Select(c => c.Id)).ToList();

                        if (missingCategories.Any())
                        {
                            result.Errors.Add($"Category không tồn tại ({string.Join(",", missingCategories)}) (dòng {row} trong excel).");
                            return result;
                        }
                    }

                    // Read size
                    var sizeName = worksheet.Cells[row, 11].GetValue<string>();
                    var price = worksheet.Cells[row, 12].GetValue<decimal>();
                    var newPrice = worksheet.Cells[row, 13].GetValue<decimal?>();
                    var sizeDescription = worksheet.Cells[row, 14].GetValue<string>();
                    var isDeletedSize = worksheet.Cells[row, 15].GetValue<bool>();

                    // Validate Size Data
                    if (string.IsNullOrWhiteSpace(sizeName))
                    {
                        result.Errors.Add($"Size Name không được trống (dòng {row} trong excel).");
                        return result;
                    }

                    if (price < 5000)
                    {
                        result.Errors.Add($"Price của item phải lớn hơn hoặc = 5.000 VNĐ (dòng {row} trong excel).");
                        return result;
                    }

                    if (newPrice.HasValue && newPrice >= price)
                    {
                        result.Errors.Add($"New Price của item phải bé hơn Price (dòng {row} trong excel).");
                        return result;
                    }


                    if (!itemsToProcess.ContainsKey(itemId))
                    {
                        var item = new Item
                        {
                            Name = itemName,
                            Slug = slug,
                            ImgUrl = imgUrl,
                            Description = description,
                            IsPublished = isPublished,
                            IsFeatured = isFeatured,
                            PublicId = publicId,
                            IsDeleted = isDeleted,
                            ItemCategories = categoryIds.Select(cid => new ItemCategory(itemId, cid)).ToList(),
                        };

                        _unit.Item.Add(item);
                        await _unit.SaveChangesAsync(); // Lưu để nhận ID
                        itemsToProcess.Add(itemId, item);
                        result.RowChange++;
                    }

                    var size = new Domain.Entities.Size
                    {
                        Name = sizeName,
                        Price = price,
                        NewPrice = newPrice,
                        Description = sizeDescription,
                        IsDeleted = isDeletedSize,
                        ItemId = itemId
                    };

                    itemsToProcess[itemId].Sizes.Add(size);
                    _unit.Size.Add(size);
                }

                if (await _unit.SaveChangesAsync())
                {
                    await _unit.CommitTransactionAsync();
                    return result;
                }

                throw new BadRequestException($"Lưu thay đổi thất bại");
            }
            catch
            {
                await _unit.RollbackTransactionAsync();
                throw;
            }
        }


        #region Helper validation
        private void ValidateUpdateItemExcelHeaders(ExcelWorksheet worksheet)
        {
            var expectedHeaders = new Dictionary<int, string>
            {
                {1, "Id"},
                {2, "Name"},
                {3, "Slug"},
                {4, "ImgUrl"},
                {5, "PublicId"},
                {6, "Description"},
                {7, "IsPublished"},
                {8, "IsFeatured"},
                {9, "IsDeleted"},
                {10, "CategoryList"}
            };

            foreach (var header in expectedHeaders)
            {
                var cellValue = worksheet.Cells[1, header.Key].GetValue<string>();
                if (cellValue != header.Value)
                {
                    throw new BadRequestException($"Invalid Excel template. Expected header '{header.Value}' at column {header.Key} but found '{cellValue}'");
                }
            }
        }


        private void ValidateAddItemExcelHeaders(ExcelWorksheet worksheet)
        {
            var expectedHeaders = new Dictionary<int, string>
            {
                {1, "Id"},
                {2, "Name"},
                {3, "Slug"},
                {4, "ImgUrl"},
                {5, "PublicId"},
                {6, "Description"},
                {7, "IsPublished"},
                {8, "IsFeatured"},
                {9, "IsDeleted"},
                {10, "CategoryList"},
                {11, "SizeName"},
                {12, "Price"},
                {13, "NewPrice"},
                {14, "SizeDescription"},
                {15, "IsDeletedSize"},
            };

            foreach (var header in expectedHeaders)
            {
                var cellValue = worksheet.Cells[1, header.Key].GetValue<string>();
                if (cellValue != header.Value)
                {
                    throw new BadRequestException($"Invalid Excel template. Expected header '{header.Value}' at column {header.Key} but found '{cellValue}'");
                }
            }
        }
        #endregion
    }
}
