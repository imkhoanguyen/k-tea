using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Tea.Infrastructure.Interfaces;

namespace Tea.Infrastructure.Services
{
    public class PdfService(IConverter converter) : IPdfService
    {
        public byte[] GeneratePdf(string contentHtml, Orientation orientation = Orientation.Portrait, PaperKind paperKind = PaperKind.A4)
        {
            var globalSetting = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = orientation,
                PaperSize = paperKind,
                Margins = new MarginSettings() { Top = 10, Bottom = 10 },
            };

            var objectSetting = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = contentHtml,
                WebSettings = { DefaultEncoding = "utf-8" },
                //HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSetting,
                Objects = { objectSetting }
            };

            return converter.Convert(pdf);
        }
    }
}
