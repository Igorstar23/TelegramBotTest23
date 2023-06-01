using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class PairDays
    {
        //<DataReg, PairDay>
        public SortedList<string, PairDay> Days;

        public PairDays(List<Pair> pairs) 
        {
            Days = new SortedList<string, PairDay>();

            if (pairs != null)
            {
                List<string> dateList = new List<string>();
                List<PairDay> pairDayList = new List<PairDay>();
                foreach(var pair in pairs)
                {
                    if (dateList.Find(x=>x.Contains(pair.DateReg)) == null) 
                    {
                        dateList.Add(pair.DateReg);
                    }
                }
                foreach(var date in dateList) 
                {
                    List<Pair> pairList = new List<Pair>();
                    foreach(var pair in pairs) 
                    {
                        if (date.Equals(pair.DateReg)) 
                        {
                            pairList.Add(pair);
                        }
                    }
                    PairDay pairDay = new PairDay(pairList);
                    Days.Add(date, pairDay);
                }
            }
        }

        //format <KOD_DISC, ABR_DISC>
        public SortedList<long, string> getDesciplines()
        {
            SortedList<long, string> disceplines = new SortedList<long, string>();

            foreach(var day in Days)
            {
                SortedList<long, string> list = day.Value.getDesciplines();
                foreach(var dsc in list) 
                {
                    if (!disceplines.ContainsKey(dsc.Key)) 
                    {
                        disceplines.Add(dsc.Key, dsc.Value);
                    }
                }
            }
            return disceplines;
            
        }
    }
}
