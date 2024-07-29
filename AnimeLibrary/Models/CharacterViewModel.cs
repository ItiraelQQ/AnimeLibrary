namespace AnimeLibrary.Models
{
    public class CharacterViewModel
    {
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public List<CharacterRelated> MediaAppearances { get; set; }

        public class CharacterRelated
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Type { get; set; }
            public string CoverImage { get; set; }
        }
    }
}
