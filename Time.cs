using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class Time
    {
        public int Hour { get; set; }
        public int Minute { get; set; }

        public Time() { this.UpdateTime(); }
        public Time(int hour, int minute) 
        {
            Hour = hour;
            Minute = minute;
        }

        public void UpdateTime() 
        {
            Hour = DateTime.Now.Hour;
            Minute = DateTime.Now.Minute;
        }

        public override string ToString()
        {
            string zeroHour = (Hour < 10) ? ("0" + Hour.ToString()) : Hour.ToString(); // for show correct format time hour < 10
            string zeroMinute = (Minute < 10) ? ("0" + Minute.ToString()) : Minute.ToString(); // for show correct format time minute < 10

            return $"{zeroHour} : {zeroMinute}";
        }
    }
}
