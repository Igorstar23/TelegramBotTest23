using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class AltPair
    {
        public string DATE_REG { get; set; }
        public NUMBER_PAIR NUM_PAIR { get; set; }
        public long GRP_ID { get; set; }
        public string INFO { get; set; }
        

        public AltPair(string date) 
        {
            if (!Pair.isNormalDate(date)) date = Pair.getNormalDate(date);
            DATE_REG = date;
            NUM_PAIR = NUMBER_PAIR.NONE;
        }
        public AltPair(string date, NUMBER_PAIR numPair):this(date) 
        {
            NUM_PAIR = numPair;
        }
        public AltPair(string date, NUMBER_PAIR numPair, long grpId):this(date, numPair) 
        {
            GRP_ID = grpId;
        }
        public AltPair(string date, NUMBER_PAIR numPair, long grpId,
            string info) : this(date, numPair, grpId)
        {
            INFO = (string.IsNullOrEmpty(info) || string.IsNullOrWhiteSpace(info))? "NONE" : info;
        }
        /**
         * <summary>Zero-based number of pair</summary>
         */
        public enum NUMBER_PAIR
        {
            NONE = -1,
            FIRST,
            SECOND,
            THIRD,
            FOURTH,
            FIFTH,
            SIXTH,
            SEVENTH,
            EIGHTH
        }
        public static int NumberPairToIntBasedOne(NUMBER_PAIR num) { return ((int)num) + 1; }
        public static NUMBER_PAIR ToNumberPair(string num) 
        {
            if (string.IsNullOrEmpty(num) || string.IsNullOrWhiteSpace(num)) return NUMBER_PAIR.NONE;

            switch(num.ToLower()) 
            {
                case "1 пара": return NUMBER_PAIR.FIRST;
                case "2 пара": return NUMBER_PAIR.SECOND;
                case "3 пара": return NUMBER_PAIR.THIRD;
                case "4 пара": return NUMBER_PAIR.FOURTH;
                case "5 пара": return NUMBER_PAIR.FIFTH;
                case "6 пара": return NUMBER_PAIR.SIXTH;
                case "7 пара": return NUMBER_PAIR.SEVENTH;
                case "8 пара": return NUMBER_PAIR.EIGHTH;
                default: return NUMBER_PAIR.NONE;
            }
        }
        /**
         * <summary>
         * get Zero-base enum
         * </summary>
         * <param name="num">must be from 0 to 8</param>
         * <returns>NUMBER_PAIR if <paramref name="num"/> is from 1 to 8; otherwise NUMBER_PAIR.NONE</returns>
         */
        public static NUMBER_PAIR ToNumberPair(int num) 
        {
            if (num < 1 || num > 8) return NUMBER_PAIR.NONE;
            return (NUMBER_PAIR)(num - 1);
        }
        public static string NumberPairToString(NUMBER_PAIR num) 
        {
            if (num == NUMBER_PAIR.NONE) return NUMBER_PAIR.NONE.ToString();

            switch (num) 
            {
                case NUMBER_PAIR.FIRST: return "1 пара";
                case NUMBER_PAIR.SECOND: return "2 пара";
                case NUMBER_PAIR.THIRD: return "3 пара";
                case NUMBER_PAIR.FOURTH: return "4 пара";
                case NUMBER_PAIR.FIFTH: return "5 пара";
                case NUMBER_PAIR.SIXTH: return "6 пара";
                case NUMBER_PAIR.SEVENTH: return "7 пара";
                case NUMBER_PAIR.EIGHTH: return "8 пара";
                default: return NUMBER_PAIR.NONE.ToString();
            }
        }
        public static TimeInterval NumberPairToTimeInterval(NUMBER_PAIR num) 
        {
            if (num == NUMBER_PAIR.NONE) return null;
            switch(num) 
            {
                case NUMBER_PAIR.FIRST: return new TimeInterval("08:30-09:50");
                case NUMBER_PAIR.SECOND: return new TimeInterval("10:05-11:25");
                case NUMBER_PAIR.THIRD: return new TimeInterval("11:40-13:00");
                case NUMBER_PAIR.FOURTH: return new TimeInterval("14:00-15:20");
                case NUMBER_PAIR.FIFTH: return new TimeInterval("15:35-16:55");
                case NUMBER_PAIR.SIXTH: return new TimeInterval("17:10-18:30");
                case NUMBER_PAIR.SEVENTH: return new TimeInterval("18:45-20:00");
                case NUMBER_PAIR.EIGHTH: return new TimeInterval("20:15-21:35");
                default: return null;
            }
        }
        public static List<TimeInterval> NumberPairLToTimeIntervalL(List<NUMBER_PAIR> nums) 
        {
            if (nums == null || nums.Count <= 0) return null;

            List<TimeInterval> intervals = new List<TimeInterval>();
            foreach(var num in nums) 
            {
                intervals.Add(NumberPairToTimeInterval(num));
            }
            return intervals;
        }
        public static List<NUMBER_PAIR> getNumberPairAllList() 
        {
            List<NUMBER_PAIR> list = new List<NUMBER_PAIR>();
            foreach (int i in Enum.GetValues(typeof(NUMBER_PAIR))) 
            {
                NUMBER_PAIR num = AltPair.ToNumberPair(i);
                if (num != NUMBER_PAIR.NONE) list.Add(num);

            }
            return list;
        }
        public static List<TimeInterval> getAllTimeIntervalsPairs() 
        {
            return NumberPairLToTimeIntervalL(getNumberPairAllList());
        }
    }
}
