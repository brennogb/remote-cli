namespace CliService.Domain.Models
{
    public class StorageDevice
    {
        public string Name { get; set; }
        
        public long AvailableSize { get; set; }

        public long TotalSize { get; set; }
    }
}