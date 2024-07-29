namespace AnimeLibrary.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public string FullText { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}