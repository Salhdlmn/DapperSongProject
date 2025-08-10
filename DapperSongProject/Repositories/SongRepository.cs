using Dapper;
using DapperSongProject.Context;
using DapperSongProject.Dtos.SongDtos;
using DapperSongProject.Models;

namespace DapperSongProject.Repositories
{
    public class SongRepository : ISongRepository
    {
        private readonly DapperContext _dapperContext;

        public SongRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<List<MonthlyStreamsDto>> GetMonthlyStreamsAsync()
        {
            var query = @"
        SELECT DATENAME(MONTH, Date) AS Month, COUNT(*) AS StreamCount
        FROM SpotifySongs
        GROUP BY DATENAME(MONTH, Date), DATEPART(MONTH, Date)
        ORDER BY DATEPART(MONTH, Date);";

            using var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryAsync<MonthlyStreamsDto>(query);
            return result.ToList();
        }

        public async Task<List<MonthlyTasksDto>> GetMonthlyTasksAsync()
        {
            var query = @"
        SELECT DATENAME(MONTH, Date) AS Month, COUNT(DISTINCT Track_Name) AS TaskCount
        FROM SpotifySongs
        GROUP BY DATENAME(MONTH, Date), DATEPART(MONTH, Date)
        ORDER BY DATEPART(MONTH, Date);";

            using var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryAsync<MonthlyTasksDto>(query);
            return result.ToList();
        }

        public async Task<MostListedTrackDto> GetMostListedTracksAsync()
        {
            var query = @"SELECT TOP 1 
                     Track_Name, 
                     COUNT(*) AS ListedCount 
                  FROM 
                     SpotifySongs 
                  GROUP BY 
                     Track_Name 
                  ORDER BY 
                     ListedCount DESC;";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<MostListedTrackDto>(query);
                return result;
            }
        }

        public async Task<MostStreamedArtistDto> GetMostStreamedArtistAsync()
        {
            var query = @"Select Top 1 Artist, Sum(CAST(Streams AS BIGINT)) as TotalStreams 
                        From SpotifySongs 
                        Group By Artist 
                        Order By TotalStreams Desc";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<MostStreamedArtistDto>(query);
                return result;
            }

        }

        public async Task<PagedResultViewModel<SongRowDto>> GetSongsPagedAsync(int page, int pageSize, string? search)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0 || pageSize > 200) pageSize = 25;
            int skip = (page - 1) * pageSize;

            // arama parametresi hazırlanır
            var s = string.IsNullOrWhiteSpace(search) ? null : $"%{search.Trim()}%";

            var sql = @"
-- 1) Toplam kayıt
;WITH Base AS (
    SELECT 
        Id,
        Position,
        Track_Name      AS TrackName,
        Artist,
        CAST(Streams AS BIGINT) AS Streams,
        Url,
        CAST([Date] AS date) AS [Date],
        Region
    FROM SpotifySongs
    WHERE (@s IS NULL OR Track_Name LIKE @s OR Artist LIKE @s)
)
SELECT COUNT(1) FROM Base;

-- 2) Sayfalı liste (CTE tekrar tanımlanır)
;WITH Base AS (
    SELECT 
        Id,
        Position,
        Track_Name      AS TrackName,
        Artist,
        CAST(Streams AS BIGINT) AS Streams,
        Url,
        CAST([Date] AS date) AS [Date],
        Region
    FROM SpotifySongs
    WHERE (@s IS NULL OR Track_Name LIKE @s OR Artist LIKE @s)
)
SELECT *
FROM Base
ORDER BY [Date] DESC, Streams DESC, Id DESC
OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;
"; 
            using var conn = _dapperContext.CreateConnection();
            using var multi = await conn.QueryMultipleAsync(sql, new { s, skip, take = pageSize });

            int total = await multi.ReadFirstAsync<int>();
            var rows = (await multi.ReadAsync<SongRowDto>()).ToList();

            return new PagedResultViewModel<SongRowDto>
            {
                Items = rows,
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Search = search
            };
        }
        

        public async Task<List<TopTracksDto>> GetTopTracksAsync(int top = 10)
        {
            var sql = @"
        SELECT TOP (@top)
            Track_Name  AS TrackName,
            Artist,
            SUM(CAST(Streams AS BIGINT)) AS TotalStreams
            FROM SpotifySongs
            GROUP BY Track_Name, Artist
            ORDER BY SUM(CAST(Streams AS BIGINT)) DESC;";
            using var conn = _dapperContext.CreateConnection();
            var result = await conn.QueryAsync<TopTracksDto>(sql, new { top });
            return result.ToList();
        }

        public async Task<List<DailyStreamDto>> GetTotalStreamsPerDayAsync()
        {
            var query = @"Select [Date],  SUM(CAST(Streams AS BIGINT))  As TotalStreams From SpotifySongs Group By              [Date] Order By [Date];";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<DailyStreamDto>(query);
                return result.ToList();
            }
        }

        public async Task<List<RegionTrackCountDto>> GetTrackCountPerRegionAsync()
        {
            var query = @" SELECT Region, TrackCount
                FROM (
               SELECT Region,
               COUNT(*) AS TrackCount,
               ROW_NUMBER() OVER (ORDER BY COUNT(*) DESC) AS RowNum
               FROM SpotifySongs
                GROUP BY Region
                ) AS RankedRegions
                WHERE RowNum > 35 OR Region = 'tr'
                ORDER BY TrackCount DESC
;";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<RegionTrackCountDto>(query);
                return result.ToList();
            }
        }

        public async Task<List<WeeklyStreamDto>> GetWeeklyTotalStreamsAsync()
        {
            var query = @"SELECT 
                     DATEADD(DAY, 1 - DATEPART(WEEKDAY, [Date]), [Date]) AS WeekStart,
                     SUM(Streams) AS TotalStreams
                  FROM 
                     SpotifySongs
                  GROUP BY 
                     DATEADD(DAY, 1 - DATEPART(WEEKDAY, [Date]), [Date])
                  ORDER BY 
                     WeekStart;";

            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<WeeklyStreamDto>(query);
                return result.ToList();
            }
        }

        public async Task<List<WeeklyViewsDto>> GetWeeklyViewsAsync()
        {
            var query = @"
                        SELECT DATENAME(WEEKDAY, Date) AS Day, COUNT(*) AS Count
                        FROM SpotifySongs
                        GROUP BY DATENAME(WEEKDAY, Date), DATEPART(WEEKDAY, Date)
                        ORDER BY DATEPART(WEEKDAY, Date);

    ";

            using var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryAsync<WeeklyViewsDto>(query);
            return result.ToList();
        }
    }
}
