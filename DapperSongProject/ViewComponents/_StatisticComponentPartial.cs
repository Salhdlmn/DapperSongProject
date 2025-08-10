using DapperSongProject.Dtos.SongDtos;
using DapperSongProject.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DapperSongProject.ViewComponents
{
    public class _StatisticComponentPartial:ViewComponent
    {
        private readonly ISongRepository _songRepository;

        public _StatisticComponentPartial(ISongRepository songRepository)
        {
            _songRepository = songRepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var dailyStreams = await _songRepository.GetTotalStreamsPerDayAsync();
            var mostStreamedArtist = await _songRepository.GetMostStreamedArtistAsync();
            var mostListedTrack = await _songRepository.GetMostListedTracksAsync();
            var weeklyTotal = await _songRepository.GetWeeklyTotalStreamsAsync();

            var model = new StatisticCardDto
            {
                TotalTodayStreams = dailyStreams.LastOrDefault()?.TotalStreams ?? 0,
                MostStreamedArtist = mostStreamedArtist.Artist,
                MostListedTrack = mostListedTrack.Track_Name,
                WeeklyTotalStreams = weeklyTotal.Sum(w => w.TotalStreams) 
            };

            return View(model);
        }
    }
}
