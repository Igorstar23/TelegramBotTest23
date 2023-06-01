using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class TimeInterval
    {
        public Time Start { get; set; }
        public Time End { get; set; }

        public TimeInterval(Time start, Time end) 
        {
            Start = start;
            End = end;
        }
        public TimeInterval(string interval) 
        {
            if (string.IsNullOrEmpty(interval) || string.IsNullOrWhiteSpace(interval)) return;
            if (!interval.Contains('-') || !interval.Contains(':')) return;

            Start = new Time();
            Start.Hour = Convert.ToInt32(interval.Split("-")[0].Split(":")[0]);
            Start.Minute = Convert.ToInt32(interval.Split("-")[0].Split(":")[1]);
            End = new Time();
            End.Hour = Convert.ToInt32(interval.Split("-")[1].Split(":")[0]);
            End.Minute = Convert.ToInt32(interval.Split("-")[1].Split(":")[1]);
        }

        public static TimeInterval getCloserTimeInterval(SortedList<int, TimeInterval> itList, Time nowTime) 
        {
            if (itList == null || nowTime == null || itList.Count <= 0) return null;

            int diffMinute = itList.First().Value.End.Minute - nowTime.Minute;
            TimeInterval firstInterval = itList.First().Value;

            if (itList.First().Key >= nowTime.Hour)
            {
                return firstInterval;
            }
            else if (firstInterval.End.Hour > nowTime.Hour ||
              firstInterval.End.Hour == nowTime.Hour && diffMinute > 10)
            {
                return firstInterval;
            }

            foreach (var interval in itList) 
            {
                diffMinute = interval.Value.End.Minute - nowTime.Minute;

                if (interval.Key >= nowTime.Hour) 
                {
                    return interval.Value;
                }
                else if (interval.Value.End.Hour > nowTime.Hour ||
                    interval.Value.End.Hour == nowTime.Hour && diffMinute > 10)
                {
                    return interval.Value;
                }
            }
            return null;

        }
        public static SortedList<int, TimeInterval> getSortedListTimeInterval(List<TimeInterval> intervals) 
        {
            SortedList<int, TimeInterval> intervalList = new SortedList<int, TimeInterval>();
            foreach(var interval in intervals) 
            {
                if (!intervalList.ContainsKey(interval.Start.Hour)) 
                {
                    intervalList.Add(interval.Start.Hour, interval);
                }
            }
            return intervalList;
        }

        public override string ToString() { return $"{Start} - {End}"; }
    }
}
