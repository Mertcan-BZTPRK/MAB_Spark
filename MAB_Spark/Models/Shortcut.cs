namespace MAB_Spark.Models
{
    public class Shortcut
    {
        public int Id { get; set; }
        public required string ShortText { get; set; }
        public required string ExpandedText { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
