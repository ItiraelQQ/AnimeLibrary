

namespace AnimeLibrary.Models
{
    public class MangaViewModel
    {
        public int Id { get; set; }
        public string? TitleEnglish { get; set; }
        public string? TitleRomaji { get; set; }
        public string? TitleNative { get; set; }
        public string? CoverImage { get; set; }
        public string? Description { get; set; }
        public int? Chapters { get; set; }
        public List<string>? Genres { get; set; }
        public string? Genre { get; set; }
        public int? Volumes { get; set; }
        public double? Score { get; set; }
        public double? AverageScore { get; set; }
        public DateTime? StartDate { get; set; }
        public string MediaType { get; set; }
        public List<Media> Mangas { get; set; }
        public List<MangaViewModel> TrendingManga { get; set; }
        public List<MangaViewModel> PopularManga { get; set; }
        public List<MangaViewModel> PopularManhwa { get; set; }
        public List<RelatedMangaViewModel> RelatedManga { get; set; }
        public List<MangaViewModel> AllMangas { get; set; }
        public class RelatedMangaViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string CoverImage { get; set; }
            public string Type { get; set; }
        }
    }
    public class MangaGenreViewModel
    {
        public string Genre { get; set; }
        public List<MangaViewModel> Mangas { get; set; }
        public List<string> Genres { get; set; }
    }

}
