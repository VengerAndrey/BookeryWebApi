namespace Domain.Models
{
    public class Item
    {
        public string Name { get; set; }
        public bool IsDirectory { get; set; }
        public long? Size { get; set; }
        public string Path { get; set; }
    }
}