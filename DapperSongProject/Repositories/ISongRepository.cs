using DapperSongProject.Dtos.SongDtos;
using DapperSongProject.Models;

namespace DapperSongProject.Repositories
{
    public interface ISongRepository
    {
        Task<List<DailyStreamDto>> GetTotalStreamsPerDayAsync();

        Task<MostStreamedArtistDto> GetMostStreamedArtistAsync();

        Task<MostListedTrackDto> GetMostListedTracksAsync();

        Task<List<RegionTrackCountDto>> GetTrackCountPerRegionAsync();


        Task<List<WeeklyStreamDto>> GetWeeklyTotalStreamsAsync();

        Task<List<WeeklyViewsDto>> GetWeeklyViewsAsync();

        Task<List<MonthlyStreamsDto>> GetMonthlyStreamsAsync();

        Task<List<MonthlyTasksDto>> GetMonthlyTasksAsync();
        // Repositories/ISongRepository.cs
        Task<List<TopTracksDto>> GetTopTracksAsync(int top = 10);

        // Repositories/ISongRepository.cs
        Task<PagedResultViewModel<SongRowDto>> GetSongsPagedAsync(int page, int pageSize, string? search);





    }
}
