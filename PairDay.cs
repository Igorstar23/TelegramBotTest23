using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class PairDay
    {
        public SortedList<int, Pair> Pairs; //key = start.Hour
        public PairDay(Pair[] pairArr) 
        {
            Pairs = new SortedList<int, Pair>();
            foreach (Pair pair in pairArr) { Pairs.Add(pair.PairTime.Start.Hour, pair); }
        }
        public PairDay(List<Pair> pairList)
        {
            Pairs = new SortedList<int, Pair>();
            foreach (Pair pair in pairList) { Pairs.Add(pair.PairTime.Start.Hour, pair); }
        }

        public Pair getPairNow()
        {
            if (Pairs == null || Pairs.Count <= 0) return null;

            Time nowTime = new Time();
            int diffMinute = Pairs.First().Value.PairTime.End.Minute - nowTime.Minute;

            TimeInterval firstInterval = Pairs.First().Value.PairTime;

            if (Pairs.First().Key >= nowTime.Hour) 
            {
                return Pairs.First().Value;

            } 
            else if (firstInterval.End.Hour > nowTime.Hour ||
                firstInterval.End.Hour == nowTime.Hour &&diffMinute > 10)
            {
                return Pairs.First().Value;
            }

            foreach(var pair in Pairs) 
            {
                diffMinute = pair.Value.PairTime.End.Minute - nowTime.Minute;

                if (pair.Key >= nowTime.Hour) 
                {
                    return pair.Value;
                } 
                else if(pair.Value.PairTime.End.Hour > nowTime.Hour || 
                    pair.Value.PairTime.End.Hour == nowTime.Hour && diffMinute > 10) 
                {
                    return pair.Value;
                }    
            }
            return null;
        }
        public Pair getPairNow(Time nowTime)
        {
            if (nowTime == null) return null;
            if (Pairs == null || Pairs.Count <= 0) return null;

            int diffMinute = Pairs.First().Value.PairTime.End.Minute - nowTime.Minute;

            TimeInterval firstInterval = Pairs.First().Value.PairTime;

            if (Pairs.First().Key >= nowTime.Hour)
            {
                return Pairs.First().Value;

            }
            else if (firstInterval.End.Hour > nowTime.Hour ||
                firstInterval.End.Hour == nowTime.Hour && diffMinute > 10)
            {
                return Pairs.First().Value;
            }

            foreach (var pair in Pairs)
            {
                diffMinute = pair.Value.PairTime.End.Minute - nowTime.Minute;

                if (pair.Key >= nowTime.Hour)
                {
                    return pair.Value;
                }
                else if (pair.Value.PairTime.End.Hour > nowTime.Hour ||
                    pair.Value.PairTime.End.Hour == nowTime.Hour && diffMinute > 10)
                {
                    return pair.Value;
                }
            }
            return null;
        }
        //format <KOD_DISC, ABR_DISC>
        public SortedList<long, string> getDesciplines() 
        {
            SortedList<long, string> disciplines = new SortedList<long, string>();

            if (Pairs == null) return null;
            foreach(var pair in Pairs) 
            {
                long KOD_DISC = long.Parse(pair.Value.KOD_DISC);

                if (!disciplines.ContainsKey(KOD_DISC)) disciplines.Add(KOD_DISC, pair.Value.ShortNamePair);
            }
            return disciplines;
        }

    }
}
