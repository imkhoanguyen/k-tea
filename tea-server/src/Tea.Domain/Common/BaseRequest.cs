namespace Tea.Domain.Common
{
    public class BaseRequest
    {
        private string? _search;
        public string? Search
        {
            get => _search;
            set => _search = value?.ToLower();
        }
        public virtual string OrderBy { get; set; } = "id_desc";
    }
}
