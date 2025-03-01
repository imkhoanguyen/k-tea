namespace Tea.Infrastructure.Configurations
{
    public class CloudinaryConfig
    {
        public static string ConfigName = "CloudinarySettings";
        public required string CloudName { get; set; }
        public required string ApiKey { get; set; }
        public required string ApiSecret { get; set; }
    }
}
