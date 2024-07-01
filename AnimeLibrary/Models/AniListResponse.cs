namespace AnimeLibrary.Models
{
    public class AniListResponse
    {
        public Page? Page { get; set; }
    }

    public class Page
    {
        public List<Media>? Media { get; set; }
    }

    public class Media
    {
        public int Id { get; set; }
        public Title? Title { get; set; }
        public CoverImage? CoverImage { get; set; }
        public string? Description { get; set; }
        public List<string>? Genres { get; set; }
        public double? Score { get; set; }
        public StartDate? StartDate { get; set; }
        public int? Episodes { get; set; }
        public double? AverageScore { get; set; }
        public int? SeasonYear { get; set; }
    }

    public class Title
    {
        public string? Romaji { get; set; }
        public string? English { get; set; }
        public string? Native { get; set; }
    }

    public class CoverImage
    {
        public string Large { get; set; }
    }

    public class StartDate
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}
