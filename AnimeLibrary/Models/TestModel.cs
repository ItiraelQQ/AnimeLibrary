﻿namespace AnimeLibrary.Models
{
    public class TestModel
    {
        public class AnimeViewModel
        {
            public int Id { get; set; }
            public string? RomajiTitle { get; set; }
            public string? EnglishTitle { get; set; }
            public string? NativeTitle { get; set; }
            public string? CoverImage { get; set; }
            public string? Description { get; set; }
            public List<string>? Genres { get; set; }
            public double? Score { get; set; }
            public DateTime? StartDate { get; set; }
            public int? Episodes { get; set; }
            public double? AverageScore { get; set; }
            public string MediaType { get; set; } = "Anime";
            public List<Media> Animes { get; set; }
            public List<RelatedAnimeViewModel> RelatedAnime { get; set; }
            public List<CharacterViewModel> Characters { get; set; }

            public class RelatedAnimeViewModel
            {
                public int Id { get; set; }
                public string Title { get; set; }
                public string CoverImage { get; set; }
                public string MediaType { get; set; } = "Anime";
            }
        }

        public class HomeViewModel
        {
            public List<AnimeViewModel> CurrentlyAiring { get; set; }
            public List<AnimeViewModel> CurrentSeason { get; set; }
            public List<AnimeViewModel> Action { get; set; }
            public List<AnimeViewModel> Romance { get; set; }
            public List<AnimeViewModel> Comedy { get; set; }
            public List<AnimeViewModel> Music { get; set; }
            public List<AnimeViewModel> Drama { get; set; }
            public List<AnimeViewModel> Horror { get; set; }
            public List<AnimeViewModel> Sports { get; set; }
            public List<AnimeViewModel> Psychological { get; set; }
            public List<AnimeViewModel> Fantasy { get; set; }
            public List<AnimeViewModel> Ecchi { get; set; }
            public List<string>? Genres { get; set; }
            public List<int>? Years { get; set; }
            public List<AnimeViewModel>? AllAnimes { get; set; }


        }
    }
}
