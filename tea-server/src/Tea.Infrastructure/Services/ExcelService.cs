using System.Drawing;
using OfficeOpenXml;
using Tea.Domain.Exceptions;
using Tea.Infrastructure.Interfaces;

namespace Tea.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public ExcelService()
        {
            ExcelPackage.License.SetNonCommercialPersonal("<K TEA>");
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
    }
}
