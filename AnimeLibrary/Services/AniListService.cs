using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnimeLibrary.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections;
using Newtonsoft.Json.Linq;
using GraphQL.Client.Abstractions;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
public static class StartDateExtensions
{
    public static DateTime? ToNullableDateTime(this StartDate startDate)
    {
        if (startDate.Year == 0 || startDate.Month == 0 || startDate.Day == 0)
        {
            return null;
        }

        try
        {
            return new DateTime(startDate.Year, startDate.Month, startDate.Day);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }
}
public class AniListService
{
    private readonly GraphQLHttpClient _client;

    public AniListService()
    {
        _client = new GraphQLHttpClient("https://graphql.anilist.co", new NewtonsoftJsonSerializer());
    }
    // Getting currently airinig anime
    public async Task<List<Media>> GetCurrentlyAiringAnime()
    {
        var query = new GraphQLRequest
        {
            Query = @"
            query {
              Page(page: 1, perPage: 10) {
                media(status: RELEASING, type: ANIME, sort: POPULARITY_DESC) {
                  id
                  title {
                    romaji
                    english
                    native
                  }
                  coverImage {
                    large
                  }
                }
              }
            }"
        };

        var response = await _client.SendQueryAsync<AniListResponse>(query);
        return response.Data.Page.Media;
    }
    // getting anime genres
    public async Task<List<Media>> GetAnimeByGenre(string genre)
    {
        var query = new GraphQLRequest
        {
            Query = @"
            query($genre: String) {
              Page(page: 1, perPage: 10) {
                media(genre_in: [$genre], type: ANIME, sort: POPULARITY_DESC) {
                  id
                  title {
                    romaji
                    english
                    native
                  }
                  coverImage {
                    large
                  }
                }
              }
            }",
            Variables = new { genre }
        };

        var response = await _client.SendQueryAsync<AniListResponse>(query);
        return response.Data.Page.Media;
    }
    // getting anime details (id, title description, etc.)
    public async Task<AnimeViewModel> GetAnimeDetails(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query($id: Int) {
          Media(id: $id, type: ANIME) {
            id
            trailer{
                id }
            title {
              romaji
              english
              native
            }
            description
            startDate {
              year
              month
              day
            }
            genres
            episodes
            averageScore
            coverImage {
              large
            }
            characters {
              edges {
                node {
                  id
                  name {
                    full
                  }
                  image {
                    large
                  }
                }
                role
              }
            }
          }
        }",
            Variables = new { id }
        };

        var response = await _client.SendQueryAsync<AniListDetailsResponse>(query);
        var media = response.Data.Media;

        var animeViewModel = new AnimeViewModel
        {
            Id = media.Id,
            RomajiTitle = media.Title?.Romaji,
            EnglishTitle = media.Title?.English,
            NativeTitle = media.Title?.Native,
            Description = media.Description,
            CoverImage = media.CoverImage?.Large,
            Score = media.AverageScore.HasValue ? media.AverageScore.Value / 10.0 : (double?)null,
            StartDate = media.StartDate?.ToNullableDateTime(),
            Episodes = media.Episodes,
            AverageScore = media.AverageScore,
            Trailer = media.Trailer?.Id,
            Genres = media.Genres,
            Characters = media.Characters?.Edges?.Select(edge => new CharacterViewModel
            {
                Name = edge.Node.Name.Full,
                ImageUrl = edge.Node.Image.Large,
                Role = edge.Role,
                CharacterId = edge.Node.Id
            }).ToList() ?? new List<CharacterViewModel>()
        };

        return animeViewModel;
    }


    // getting current season anime
    public async Task<List<Media>> GetCurrentSeasonAnimeAsync()
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query {
          Page(page: 1, perPage: 20) {
            media(season: SUMMER, seasonYear: 2024, type: ANIME, sort: POPULARITY_DESC) {
              id
              title {
                romaji
                english
                native
              }
              coverImage {
                large
              }
            }
          }
        }"
        };

        var response = await _client.SendQueryAsync<AniListResponse>(query);
        if (response.Errors == null || !response.Errors.Any())
        {
            return response.Data.Page.Media;
        }
        else
        {
            // Создание сообщения об ошибке, содержащего детали каждой ошибки GraphQL
            var errorMessages = response.Errors.Select(e => $"{e.Message} (Location: {e.Locations})");
            var errorMessage = string.Join("\n", errorMessages);
            throw new Exception($"GraphQL query failed with errors:\n{errorMessage}");
        }
    }
    // getting list of genres (anime & manga)
    public async Task<List<string>> GetGenresAsync()
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query {
          GenreCollection
        }"
        };

        var response = await _client.SendQueryAsync<dynamic>(query);
        if (response.Errors == null || !response.Errors.Any())
        {
            return response.Data.GenreCollection.ToObject<List<string>>();
        }
        else
        {
            // Обработка ошибок GraphQL
            var errorMessages = response.Errors.Select(e => $"{e.Message} (Location: {e.Locations})");
            var errorMessage = string.Join("\n", errorMessages);
            throw new Exception($"GraphQL query failed with errors:\n{errorMessage}");
        }
    }
    // getting years (not used)
    public async Task<List<int>> GetYearsAsync()
    {
        // Предполагаем, что AniList API предоставляет список годов, если нет, вам нужно будет его сгенерировать
        var currentYear = DateTime.UtcNow.Year;
        var years = Enumerable.Range(currentYear - 35, 36).OrderByDescending(year => year).ToList(); // Пример: последние 10 лет + текущий год
        return await Task.FromResult(years);
    }

    // getting genres of the current anime
    public async Task<List<Media>> ListAnimeGenre(string genre, int page, int perPage)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query($genre: String, $page: Int, $perPage: Int) {
          Page(page: $page, perPage: $perPage) {
            media(genre_in: [$genre], type: ANIME, sort: POPULARITY_DESC) {
              id
              title {
                romaji
                english
                native
              }
              coverImage {
                large
              }
            }
          }
        }",
            Variables = new { genre, page, perPage }
        };

        var response = await _client.SendQueryAsync<AniListResponse>(query);
        return response.Data.Page.Media;
    }
    // getting manga genres
    public async Task<List<Media>> ListMangaGenre(string genre, int page, int perPage)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query($genre: String, $page: Int, $perPage: Int) {
          Page(page: $page, perPage: $perPage) {
            media(genre_in: [$genre], type: MANGA, sort: POPULARITY_DESC) {
              id
              title {
                romaji
                english
                native
              }
              coverImage {
                large
              }
            }
          }
        }",
            Variables = new { genre, page, perPage }
        };

        var response = await _client.SendQueryAsync<AniListResponse>(query);
        return response.Data.Page.Media;
    }
    // search by title (TODO: optimize)
    public async Task<JObject> SearchAnimeByTitleAsync(string title)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query ($search: String) {
            Page {
                media(search: $search, type: ANIME, sort: POPULARITY_DESC) {
                    id
                    title {
                        romaji
                        english
                        native
                    }
                    description
                    startDate {
                        year
                        month
                        day
                    }
                    episodes
                    genres
                    averageScore
                    coverImage {
                        large
                    }
                }
            }
        }",
            Variables = new { search = title }
        };

        var response = await _client.SendQueryAsync<dynamic>(query);
        if (response.Errors == null || !response.Errors.Any())
        {
            return JObject.FromObject(response.Data);
        }
        else
        {
            // Обработка ошибок GraphQL
            var errorMessages = response.Errors.Select(e => $"{e.Message} (Location: {e.Locations})");
            var errorMessage = string.Join("\n", errorMessages);
            throw new Exception($"GraphQL query failed with errors:\n{errorMessage}");
        }
    }
    // manga search (TODO: optimize)
    public async Task<JObject> SearchMangaByTitleAsync(string title)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query ($search: String) {
            Page {
                media(search: $search, type: MANGA, sort: POPULARITY_DESC) {
                    id
                    title {
                        romaji
                        english
                        native
                    }
                    description
                    }
                    coverImage {
                        large
                    }
                }
            }
        }",
            Variables = new { search = title }
        };

        var response = await _client.SendQueryAsync<dynamic>(query);
        if (response.Errors == null || !response.Errors.Any())
        {
            return JObject.FromObject(response.Data);
        }
        else
        {
            var errorMessages = response.Errors.Select(e => $"{e.Message} (Location: {e.Locations})");
            var errorMessage = string.Join("\n", errorMessages);
            throw new Exception($"GraphQL query failed with errors:\n{errorMessage}");
        }
    }

    // getting related
    public async Task<List<AnimeViewModel.RelatedAnimeViewModel>> GetRelatedAnimeAsync(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
            query($id: Int) {
              Media(id: $id, type: ANIME) {
                type
                relations {
                  edges {
                    relationType(version: 2)
                    node {
                      id
                      type
                      title {
                        romaji
                        english
                        native
                      }
                      coverImage {
                        large
                      }
                    }
                  }
                }
              }
            }",
            Variables = new { id }
        };

        var response = await _client.SendQueryAsync<AniListRelatedResponse>(query);
        var relations = response.Data.Media?.Relations?.Edges
            .Select(edge => new AnimeViewModel.RelatedAnimeViewModel
            {
                Id = edge?.Node?.Id ?? 0,
                Title = edge?.Node?.Title?.Romaji ?? "",
                CoverImage = edge?.Node?.CoverImage?.Large ?? "",
                RelationType = edge?.RelationType ?? "",
                Type = edge?.Node?.Type ?? ""
            })
            .ToList();

        return relations ?? new List<AnimeViewModel.RelatedAnimeViewModel>();
    }
    // getting manga list
    public async Task<List<MangaViewModel>> GetMangaListAsync()
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query {
          Page(page: 1, perPage: 10) {
            media(type: MANGA, sort: POPULARITY_DESC) {
              id
              title {
                romaji
                english
                native
              }
              coverImage {
                large
              }
              description
              chapters
              genres
              volumes
              averageScore
            }
          }
        }"
        };

        var response = await _client.SendQueryAsync<dynamic>(query);
        if (response.Errors == null || !response.Errors.Any())
        {
            var mediaList = (response.Data.Page.media as IEnumerable<dynamic>).Select(manga => new MangaViewModel
            {
                Id = (int)manga.id,
                TitleRomaji = (string)manga.title.romaji,
                TitleEnglish = (string)manga.title.english,
                TitleNative = (string)manga.title.native,
                CoverImage = (string)manga.coverImage.large,
                Description = (string)manga.description,
                Genres = manga.genres,
                MediaType = "Manga",
                Chapters = manga.chapters != null ? (int)manga.chapters : (int?)null,
                Volumes = manga.volumes != null ? (int)manga.volumes : (int?)null,
                Score = manga.averageScore != null ? (double)manga.averageScore / 10.0 : (double?)null
            }).ToList();

            return mediaList;
        }
        else
        {
            var errorMessages = response.Errors.Select(e => $"{e.Message} (Location: {e.Locations})");
            var errorMessage = string.Join("\n", errorMessages);
            throw new Exception($"GraphQL query failed with errors:\n{errorMessage}");
        }
    }
    // getting manga trends
    public async Task<List<MangaViewModel>> GetTrendingMangaAsync()
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query {
          Page(page: 1, perPage: 15) {
            media(sort: TRENDING_DESC, type: MANGA) {
              id
              title {
                romaji
                english
                native
              }
              coverImage {
                large
              }
              description
              chapters
              volumes
              averageScore
            }
          }
        }"
        };

        var response = await _client.SendQueryAsync<AniListResponse>(query);
        if (response.Data?.Page?.Media != null)
        {
            return response.Data.Page.Media.Select(m => new MangaViewModel
            {
                Id = m.Id,
                TitleRomaji = m.Title.Romaji,
                TitleEnglish = m.Title.English,
                TitleNative = m.Title.Native,
                CoverImage = m.CoverImage.Large,
                Description = m.Description,
                Chapters = m.Chapters,
                Volumes = m.Volumes,
                Score = m.AverageScore.HasValue ? m.AverageScore.Value / 10.0 : (double?)null
            }).ToList();
        }

        return new List<MangaViewModel>();
    }
    // getting manga popular
    public async Task<List<MangaViewModel>> GetPopularMangaAsync()
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query {
          Page(page: 1, perPage: 15) {
            media(sort: POPULARITY_DESC, type: MANGA) {
              id
              title {
                romaji
                english
                native
              }
              coverImage {
                large
              }
              description
              chapters
              volumes
              averageScore
            }
          }
        }"
        };

        var response = await _client.SendQueryAsync<AniListResponse>(query);
        if (response.Data?.Page?.Media != null)
        {
            return response.Data.Page.Media.Select(m => new MangaViewModel
            {
                Id = m.Id,
                TitleRomaji = m.Title.Romaji,
                TitleEnglish = m.Title.English,
                TitleNative = m.Title.Native,
                CoverImage = m.CoverImage.Large,
                Description = m.Description,
                Chapters = m.Chapters,
                Volumes = m.Volumes,
                Score = m.AverageScore.HasValue ? m.AverageScore.Value / 10.0 : (double?)null
            }).ToList();
        }

        return new List<MangaViewModel>();
    }
    // getting popular manhwa (NOT USED)
    public async Task<List<MangaViewModel>> GetPopularManhwaAsync()
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query {
          Page(page: 1, perPage: 15) {
            media(sort: POPULARITY_DESC, type: MANGA, format: MANHWA) {
              id
              title {
                romaji
                english
                native
              }
              coverImage {
                large
              }
              description
              chapters
              volumes
              averageScore
            }
          }
        }"
        };

        var response = await _client.SendQueryAsync<AniListResponse>(query);
        if (response.Data?.Page?.Media != null)
        {
            return response.Data.Page.Media.Select(m => new MangaViewModel
            {
                Id = m.Id,
                TitleRomaji = m.Title.Romaji,
                TitleEnglish = m.Title.English,
                TitleNative = m.Title.Native,
                CoverImage = m.CoverImage.Large,
                Description = m.Description,
                Chapters = m.Chapters,
                Volumes = m.Volumes,
                Score = m.AverageScore.HasValue ? m.AverageScore.Value / 10.0 : (double?)null
            }).ToList();
        }

        return new List<MangaViewModel>();
    }
    // getting manga details
    public async Task<MangaViewModel> GetMangaDetails(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
            query($id: Int) {
              Media(id: $id, type: MANGA) {
                id
                title {
                  romaji
                  english
                  native
                }
                startDate {
                  year
                  month
                  day
                }
                description
                chapters
                volumes
                genres
                averageScore
                coverImage {
                  large
                }
              }
            }",
            Variables = new { id }
        };

        var response = await _client.SendQueryAsync<AniListDetailsResponse>(query);
        var media = response.Data.Media;

        var mangaViewModel = new MangaViewModel
        {
            Id = media.Id,
            TitleRomaji = media.Title?.Romaji,
            TitleEnglish = media.Title?.English,
            TitleNative = media.Title?.Native,
            Description = media.Description,
            Chapters = media.Chapters,
            Volumes = media.Volumes,
            Score = media.AverageScore.HasValue ? media.AverageScore.Value / 10.0 : (double?)null,
            CoverImage = media.CoverImage?.Large,
            StartDate = media.StartDate.ToNullableDateTime(),
            Genres = media.Genres,
            
        };

        return mangaViewModel;
    }
    // getting related manga
    public async Task<List<MangaViewModel.RelatedMangaViewModel>> GetRelatedMangaAsync(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query($id: Int) {
          Media(id: $id, type: MANGA) {
            relations {
              edges {
                relationType(version: 2)
                node {
                  id
                  type
                  title {
                    romaji
                    english
                    native
                  }
                  coverImage {
                    large
                  }
                }
              }
            }
          }
        }",
            Variables = new { id }
        };

        var response = await _client.SendQueryAsync<AniListRelatedResponse>(query);
        var relations = response.Data.Media?.Relations?.Edges
            .Select(edge => new MangaViewModel.RelatedMangaViewModel
            {
                Id = edge?.Node?.Id ?? 0,
                Title = edge?.Node?.Title?.Romaji ?? "",
                CoverImage = edge?.Node?.CoverImage?.Large ?? "",
                Type = edge?.Node?.Type ?? ""
            })
            .ToList();

        return relations ?? new List<MangaViewModel.RelatedMangaViewModel>();
    }
    // ???
    public async Task<string> GetContentTypeById(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
            query($id: Int) {
              Media(id: $id) {
                type
              }
            }",
            Variables = new { id }
        };

        var response = await _client.SendQueryAsync<dynamic>(query);
        var mediaType = response.Data.Media?.type?.ToString();

        // Возвращаем тип контента (anime или manga)
        return mediaType;
    }

    // ???
    public async Task<string> GetCorrectDetailPageUrl(int id)
    {
        var contentType = await GetContentTypeById(id);

        if (contentType == "ANIME")
        {
            // Return URL for anime details page
            return $"/Anime/Details/{id}";
        }
        else if (contentType == "MANGA")
        {
            // Return URL for manga details page
            return $"/Manga/Details/{id}";
        }
        else
        {
            // Handle other cases or errors
            throw new Exception("Unknown content type");
        }
    }
    // getting character details
    public async Task<CharacterViewModel> GetCharacterDetailsAsync(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query($id: Int) {
          Character(id: $id) {
            id
            name {
              full
            }
            image {
              large
            }
            description(asHtml: true)
          }
        }",
            Variables = new { id }
        };

        try
        {
            var response = await _client.SendQueryAsync<dynamic>(query);
            var characterData = response.Data.Character;

            if (characterData == null) return null;

            var character = new CharacterViewModel
            {
                CharacterId = characterData.id,
                Name = characterData.name.full,
                ImageUrl = characterData.image.large,
                Description = characterData.description
            };

            return character;
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Ошибка при получении данных персонажа: {ex.Message}");
            return null;
        }
    }
    // getting character appereances
    public async Task<List<CharacterViewModel.CharacterRelated>> GetCharacterAppearancesAsync(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
    query($id: Int) {
      Character(id: $id) {
        id
        name {
          full
        }
        image {
          large
        }
        media {
          edges {
            node {
              id
              title {
                romaji
                english
                native
              }
              type
              coverImage {
                large
              }
            }
          }
        }
        description(asHtml: true)
      }
    }",
            Variables = new { id }
        };

        try
        {
            var response = await _client.SendQueryAsync<AniListCharacterResponse>(query);
            var mediaAppearances = response.Data.Character?.Media?.Edges
                .Select(edge => new CharacterViewModel.CharacterRelated
                {
                    Id = edge.Node.Id,
                    Title = edge.Node.Title.Romaji,
                    Type = edge.Node.Type,
                    CoverImage = edge.Node.CoverImage.Large,
                })
                .ToList();

            return mediaAppearances ?? new List<CharacterViewModel.CharacterRelated>();
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Ошибка при получении данных персонажа: {ex.Message}");
            return new List<CharacterViewModel.CharacterRelated>();
        }
    }



    // secondary classes for API answers
    private class AniListRelatedResponse
    {
        public MediaDetails Media { get; set; }
    }

    private class MediaDetails
    {
        public MediaRelations Relations { get; set; }
    }

    private class MediaRelations
    {
        public List<MediaEdge> Edges { get; set; }
    }

    private class MediaEdge
    {
        public string? RelationType { get; set; }
        public MediaNode Node { get; set; }
        
        
    }

    private class MediaNode
    {
        public int Id { get; set; }
        public MediaTitle Title { get; set; }
        public MediaCoverImage CoverImage { get; set; }
        public string Type {  get; set; }
    }

    private class MediaTitle
    {
        public string Romaji { get; set; }
        public string English { get; set; }
        public string Native { get; set; }
    }

    private class MediaCoverImage
    {
        public string Large { get; set; }
    }


}


