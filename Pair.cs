using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class Pair
    {
        public string DateReg { get; set; }
        public string NumPair { get; set; }
        public TimeInterval PairTime { get; set; }
        public string TypePair { get; set; }
        public string IsOnline { get; set; }
        public string TeacherName { get; set; } 
        public string ShortNamePair { get; set; }
        public string KOD_DISC { get; set; }
        public string Auditoria { get; set; }

        public PairLink WorkLink { get; set; }

        public Pair(JsonPair convertObj) // for Convert from JsonPAIR to PAIR
        {
            this.DateReg = convertObj.DATE_REG;
            this.NumPair = convertObj.NAME_PAIR;
            this.TypePair = convertObj.NAME_STUD;
            this.IsOnline = convertObj.REASON;
            this.TeacherName = convertObj.NAME_FIO;
            this.ShortNamePair = convertObj.ABBR_DISC;
            this.Auditoria = convertObj.NAME_AUD;
            this.PairTime = new TimeInterval(convertObj.TIME_PAIR);
            this.KOD_DISC = convertObj.KOD_DISC;

            if (this.KOD_DISC == "0" && (string.IsNullOrEmpty(this.ShortNamePair) 
                || string.IsNullOrWhiteSpace(this.ShortNamePair)) ) 
            {
                this.ShortNamePair = "Дисципліна за вибором. Див розклад дин. групи";
            }
            this.WorkLink = new PairLink();
        }
        public static bool isNormalDate(string date) 
        {
            if (!date.Contains('-')) return false;
            if (date.Split('-') == null || date.Split('-').Length < 3) return false;
            return true;
        }
        /**
         * <summary>
         * Get Data format for DataBase
         * </summary>
         */
        public static string getNormalDate(string date) 
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date)) return null;
            date = date.Trim(' ');
            date = date.Contains(' ') ? date.Split(' ')[0] : date;
            string day = date.Split('.')[0];
            string month = date.Split('.')[1];
            string year = date.Split('.')[2];
            return $"{year}-{month}-{day}";
        }
        public static string getNormalDate(DateTime date) 
        {
            if (date == null) return null;
            return getNormalDate(date.ToString());
        }
        /**
         * <summary>
         * Get Data format for Internet Source
         * </summary>
         */
        public static string getUnNormalDate(string date) 
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date)) return null;
            date = date.Trim(' ');
            date = date.Contains(' ') ? date.Split(' ')[0] : date;
            string day = date.Split('-')[2];
            string month = date.Split('-')[1];
            string year = date.Split('-')[0];
            return $"{day}.{month}.{year}";
        }
        public static string getUnNormalDate(DateTime date) 
        {
            if (date == null) return null;
            return getUnNormalDate(date.ToString());
        }
    }
}
