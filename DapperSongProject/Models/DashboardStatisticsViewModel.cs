using DapperSongProject.Dtos.SongDtos;

namespace DapperSongProject.Models
{
    public class DashboardStatisticsViewModel
    {
        public List<WeeklyViewsDto> WeeklyViews { get; set; } = new();

        public List<MonthlyStreamsDto> MonthlyStreams { get; set; } = new();

        public List<MonthlyTasksDto> MonthlyTasks { get; set; } = new();

        public List<RegionTrackCountDto> RegionStreams { get; set; } = new();
        public List<TopTracksDto> TopTracks { get; set; } = new();
    }
}
