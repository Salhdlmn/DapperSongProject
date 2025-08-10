namespace DapperSongProject.Dtos.SongDtos
{
    public class SongRowDto
    {
        public int Id { get; set; }            
        public int? Position { get; set; }
        public string TrackName { get; set; } = "";
        public string Artist { get; set; } = "";
        public long Streams { get; set; }          
        public string Url { get; set; } = "";
        public DateTime Date { get; set; }
        public string Region { get; set; } = "";
    }
}
