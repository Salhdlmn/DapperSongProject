using DapperSongProject.Models;
using DapperSongProject.Repositories;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace DapperSongProject.ViewComponents
{
    public class _GraphStatisticComponentPartial:ViewComponent
    {
        private readonly ISongRepository _songRepository;

        public _GraphStatisticComponentPartial(ISongRepository songRepository)
        {
            _songRepository = songRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var weeklyViews = await _songRepository.GetWeeklyViewsAsync();
            var monthlyStreams = await _songRepository.GetMonthlyStreamsAsync();
            var monthlyTasks = await _songRepository.GetMonthlyTasksAsync();
            var regionStreams = await _songRepository.GetTrackCountPerRegionAsync();
            var topTracks = await _songRepository.GetTopTracksAsync(10);

            var model = new DashboardStatisticsViewModel
            {
                WeeklyViews = weeklyViews,
                MonthlyStreams = monthlyStreams,
                MonthlyTasks = monthlyTasks,
                RegionStreams = regionStreams,
                TopTracks= topTracks
            };

            return View(model);
        }
    }
}
