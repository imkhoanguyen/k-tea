using DinkToPdf;

namespace Tea.Infrastructure.Interfaces
{
    public interface IPdfService
    {
        byte[] GeneratePdf(string contentHtml, Orientation orientation = Orientation.Portrait, PaperKind paperKind = PaperKind.A4);
    }
}