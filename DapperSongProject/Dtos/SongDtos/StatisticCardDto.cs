namespace DapperSongProject.Dtos.SongDtos
{
    public class StatisticCardDto
    {
        public int TotalTodayStreams { get; set; }
        public string MostStreamedArtist { get; set; }
        public string MostListedTrack { get; set; }
        public long WeeklyTotalStreams { get; set; } // ✅ Yeni eklenen alan
    }

}
