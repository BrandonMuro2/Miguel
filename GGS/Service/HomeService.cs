using GGS.Engine;
using Microsoft.EntityFrameworkCore;
using System.IO;
using SharpEDFlib;

namespace GGS.Service
{
    public class ChannelActivity
    {
        public int ChannelId { get; set; }
        public string SessionType { get; set; }
        public double AvgAmplitude { get; set; }
    }

    public class HomeService
    {
        private readonly NewContext _context;
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public HomeService(NewContext context)
        {
            _context = context;
        }

        public IEnumerable<MuestrasDeSeñale> GetSignalsBySubject(int subjectId, int channelId, string type)
        {
            var sessionId = _context.Sesiones
                                    .Where(ses => ses.SubjectId == subjectId && ses.SessionType == type)
                                    .Select(ses => ses.SessionId)
                                    .FirstOrDefault();




            var query = @"
            SELECT * FROM (
            SELECT *, ROW_NUMBER() OVER (ORDER BY TimeIndex) as RowNum FROM MuestrasDeSeñales
            WHERE SessionId = {0} AND ChannelId = {1} AND TimeIndex < 30000) AS SampledData
            WHERE RowNum % 10 = 1
            ORDER BY TimeIndex";

            var muestras = _context.MuestrasDeSeñales
                                          .FromSqlRaw(query, sessionId, channelId)
                                          .ToList();

            return muestras;
        }


        public IEnumerable<Sujeto> GetSubjects()
        {
            var subjects = _context.Sujetos.ToList();

            return subjects;
        }

        public IEnumerable<Canale> GetChannels()
        {
            var channels = _context.Canales.ToList();

            return channels;
        }


        public IEnumerable<ChannelActivity> GetActivityByChannels(string channelNames)
        {
            var channelNamesArray = channelNames.Split(',');
            var formattedChannelNames = string.Join(",", channelNamesArray.Select(name => $"'{name.Trim()}'"));

            var query = $@"
	    SELECT 
        ChannelId, 
		SessionType,
        AVG(ABS(Amplitude)) AS AvgAmplitude
    FROM (
        SELECT 
            M.ChannelId, 
            M.Amplitude, 
			S.SessionType,
            ROW_NUMBER() OVER (PARTITION BY M.ChannelId ORDER BY TimeIndex) as RowNum
        FROM 
            MuestrasDeSeñales M
		JOIN Sesiones S ON S.SessionID = M.SessionID
        WHERE 
            ChannelId IN (SELECT ChannelId FROM Canales WHERE ChannelName IN ({formattedChannelNames}))
            AND TimeIndex < 30000
    ) AS SampledData
     GROUP BY 
        ChannelId, SessionType
        ORDER BY 
        ChannelId, SessionType";

            var activityData = _context.ChannelActivities.FromSqlRaw(query).ToList();
            return activityData;
        }


    }
}
