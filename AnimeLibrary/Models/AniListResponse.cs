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
        public int? Chapters { get; set; }
        public Trailer? Trailer { get; set; }
        public int? Volumes { get; set; }
        public string MediaType { get; set; }
        public CharacterConnection? Characters { get; set; }
    }

    public class Trailer
    {
        public string Id { get; set; }
        
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

    public class CharacterConnection
    {
        public List<CharacterEdge> Edges { get; set; }
    }

    public class CharacterEdge
    {
        public CharacterNode Node { get; set; }
        public string Role { get; set; }
        
    }

    public class CharacterNode
    {
        public int Id { get; set; }
        public CharacterName Name { get; set; }
        public CharacterImage Image { get; set; }
    }

    public class CharacterName
    {
        public string Full { get; set; }
    }

    public class CharacterImage
    {
        public string Large { get; set; }
    }
}
