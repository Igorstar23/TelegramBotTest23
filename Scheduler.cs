using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace TelegramBotTest
{
    // For getting data from site and convert ...
    class Scheduler
    {
        private DBWorker _DB;
        private static Scheduler s_instance;
        private static ILogger<Scheduler> _logger;

        private Scheduler(DBWorker db, ILogger<Scheduler> logger) 
        {
            _DB = db;
            _logger = logger;
        }
        public static Scheduler getInstance(DBWorker db, ILogger<Scheduler> logger) { return s_instance ??= new Scheduler(db, logger); }

        public static List <JsonPair> ToListJsonPairs(string jsonString) 
        {
            List<JsonPair> jsonPairs = JsonConvert.DeserializeObject<List<JsonPair>>(jsonString);
            return jsonPairs;
        }
        public static string GET(string url, string data = null, int recLvl = 0)
        {
            string strReq = url;
            strReq = (data != null) ? strReq + "?" + data : strReq;
            WebRequest req = WebRequest.Create(strReq);
            try
            {
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string Out = sr.ReadToEnd();
                sr.Close();
                return Out;

            } catch(HttpRequestException e)
            {
                if (recLvl > 5)
                {
                    _logger.LogError("SSL Ban.Count Try = {recLvl}: {e}", recLvl, e);
                    return null;
                }

                System.Threading.Thread.Sleep(200);
                return GET(url, data, ++recLvl);
            }
        } // IT to do get respone on url with data
        public static string getStrFromJson(string json) 
        {
            string res = json.Trim('{').Trim('}').Replace('"', ' ');
            return Regex.Unescape(res);
        }

        public PairDay getPairDayNow(User user, string url, string data = null)
        {
            string date = Pair.getUnNormalDate(Pair.getNormalDate(DateTime.Now));
            data = $"{data}{user.GrpID}&date_beg={date}&date_end={date}";
            List<JsonPair> jsonPairs = ToListJsonPairs(GET(url, data));
            List<Pair> pairs = ToPairList(jsonPairs);
            return new PairDay(pairs);
        }
        public PairDay getPairDayNow(long grpId, string url, string data = null)
        {
            string date = Pair.getUnNormalDate(Pair.getNormalDate(DateTime.Now));
            data = $"{data}{grpId}&date_beg={date}&date_end={date}";
            List<JsonPair> jsonPairs = ToListJsonPairs(GET(url, data));
            List<Pair> pairs = ToPairList(jsonPairs);
            return new PairDay(pairs);
        }
        public PairDays getPairDays(User user, string url, string data = null, string[] date = null)
        {
            date ??= new string[] { Pair.getUnNormalDate(Pair.getNormalDate(DateTime.Now)), Pair.getUnNormalDate(Pair.getNormalDate(DateTime.Now)) };
            date[1] ??= new string(date[0]);
            data = $"{data}{user.GrpID}&date_beg={date[0]}&date_end={date[1]}";
            List<JsonPair> jsonPairs = ToListJsonPairs(GET(url, data));
            List<Pair> pairs = ToPairList(jsonPairs);
            return new PairDays(pairs);
        }
        public PairDays getPairDays(long grpId, string url, string data = null, string[] date = null)
        {
            date ??= new string[] { Pair.getUnNormalDate(Pair.getNormalDate(DateTime.Now)), Pair.getUnNormalDate(Pair.getNormalDate(DateTime.Now)) };
            date[1] ??= new string(date[0]);
            data = $"{data}{grpId}&date_beg={date[0]}&date_end={date[1]}";
            List<JsonPair> jsonPairs = ToListJsonPairs(GET(url, data));
            List<Pair> pairs = ToPairList(jsonPairs);
            return new PairDays(pairs);
        }

        public List<Pair> ToPairList(List<JsonPair> jsonPairs) 
        {
            List<Pair> pairs = new List<Pair>();
            foreach(JsonPair json in jsonPairs) { pairs.Add(new Pair(json));}
            return pairs;
        }
        public void UpdateGroup(string connection) 
        {
            Group[] oldGrps = _DB.readAllGroups();
            List<Group> oldGrpsList = new List<Group>();
            List<Group> needUpdateList = new List<Group>();
            List<Group> needInsertList = new List<Group>();
            foreach(Group grp in oldGrpsList) { oldGrpsList.Add(grp); }

            string res = Scheduler.GET(connection, "method=getGroups");
            res = Scheduler.getStrFromJson(res);
            string[] rows = res.Split(',');

            Group[] grps = new Group[rows.Length];
            for (int i = 0; i < grps.Length; i++) 
            {
                grps[i] = new Group(rows[i]);

                if (oldGrpsList.Find(x => x.Id == grps[i].Id) == null)
                {
                    needInsertList.Add(grps[i]);
                }
                else if (oldGrpsList.Find(x => x.Id == grps[i].Id && x.GrpName != grps[i].GrpName) != null) 
                {
                    needUpdateList.Add(grps[i]);
                }
            }

            if (needInsertList.Count != 0) _DB.insertPoolGroups(needInsertList);

            foreach(Group grp in needUpdateList) 
            {
                _DB.updateGroup(grp);
            }
        }

    }
}
