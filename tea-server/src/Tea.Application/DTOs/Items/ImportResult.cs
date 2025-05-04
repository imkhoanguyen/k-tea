namespace Tea.Application.DTOs.Items
{
    public class ImportResult
    {
        public int RowChange { get; set; } = 0;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
