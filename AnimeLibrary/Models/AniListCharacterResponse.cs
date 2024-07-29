public class AniListCharacterResponse
{
    public CharacterData Character { get; set; }
}

public class CharacterData
{
    public MediaData Media { get; set; }
}

public class MediaData
{
    public List<MediaEdge> Edges { get; set; }
}

public class MediaEdge
{
    public MediaNode Node { get; set; }
}

public class MediaNode
{
    public int Id { get; set; }
    public MediaTitle Title { get; set; }
    public string Type { get; set; }
    public CoverImage CoverImage { get; set; }
}

public class MediaTitle
{
    public string Romaji { get; set; }
    public string English { get; set; }
    public string Native { get; set; }
}

public class CoverImage
{
    public string Large { get; set; }
}
