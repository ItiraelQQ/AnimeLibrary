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

    public async Task<AnimeViewModel> GetAnimeDetails(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query($id: Int) {
          Media(id: $id, type: ANIME) {
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
            genres
            episodes
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
            Episodes = media.Episodes, // Добавлено количество эпизодов
            AverageScore = media.AverageScore, // Добавлен средний балл
            Genres = media.Genres //Жанры
        };

        return animeViewModel;
    }

    public async Task<List<Media>> GetCurrentSeasonAnimeAsync()
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query {
          Page(page: 1, perPage: 20) {
            media(season: SPRING, seasonYear: 2024, type: ANIME, sort: POPULARITY_DESC) {
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

    public async Task<List<int>> GetYearsAsync()
    {
        // Предполагаем, что AniList API предоставляет список годов, если нет, вам нужно будет его сгенерировать
        var currentYear = DateTime.UtcNow.Year;
        var years = Enumerable.Range(currentYear - 35, 36).OrderByDescending(year => year).ToList(); // Пример: последние 10 лет + текущий год
        return await Task.FromResult(years);
    }


    public async Task<List<Media>> ListAnimeGenre(string genre)
    {
        var query = new GraphQLRequest
        {
            Query = @"
            query($genre: String) {
              Page(page: 1, perPage: 50) {
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

    // Добавьте этот метод в класс AniListService
    public async Task<JObject> SearchAnimeByTitleAsync(string title)
    {
        var query = new GraphQLRequest
        {
            Query = @"
        query ($search: String) {
          Page {
            media(search: $search, type: ANIME) {
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


    public async Task<List<AnimeViewModel.RelatedAnimeViewModel>> GetRelatedAnimeAsync(int id)
    {
        var query = new GraphQLRequest
        {
            Query = @"
            query($id: Int) {
              Media(id: $id, type: ANIME) {
                relations {
                  edges {
                    relationType(version: 2)
                    node {
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
                CoverImage = edge?.Node?.CoverImage?.Large ?? ""
            })
            .ToList();

        return relations ?? new List<AnimeViewModel.RelatedAnimeViewModel>();
    }

    // Вспомогательные классы моделей для ответов API
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
        public string RelationType { get; set; }
        public MediaNode Node { get; set; }
    }

    private class MediaNode
    {
        public int Id { get; set; }
        public MediaTitle Title { get; set; }
        public MediaCoverImage CoverImage { get; set; }
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


