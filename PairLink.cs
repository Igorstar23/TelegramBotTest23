using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class PairLink
    {
        public long DSC_ID { get; set; }
        public long GRP_ID { get; set; }
        public DSC_TYPES DSC_TYPE { get; set; }
        
        public string Link { get; set; }
        public string LinkId { get; set; } // Zoom Id
        public string Pass { get; set; } // Zoom Pass

        public PairLink() 
        {
            Link = "NONE";
            LinkId = "NONE";
            Pass = "NONE";
        }

        public PairLink(long dSC_ID, long gRP_ID, DSC_TYPES dSC_TYPE) : this()
        {
            DSC_ID = dSC_ID;
            GRP_ID = gRP_ID;
            DSC_TYPE = dSC_TYPE;
        }
        public PairLink(long dSC_ID, long gRP_ID, DSC_TYPES dSC_TYPE, string link) 
            : this(dSC_ID, gRP_ID, dSC_TYPE)
        {
            Link = link;
        }
        public PairLink(long dSC_ID, long gRP_ID, DSC_TYPES dSC_TYPE, 
            string link, string linkId, string pass) 
            : this(dSC_ID, gRP_ID, dSC_TYPE, link)
        {
            LinkId = linkId;
            Pass = pass;
        }



        public enum DSC_TYPES { NONE = -1, LECTION = 0, LAB = 1, ATEST = 2, EXAM = 3 }
        public static DSC_TYPES getDscType(string type) 
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrWhiteSpace(type)) return DSC_TYPES.NONE;

            switch(type.ToLower()) 
            {
                case var text when text.Contains("лекція")||text.Contains("lec"):
                    return DSC_TYPES.LECTION;
                break;

                case var text when text.Contains("лаб") || text.Contains("прак")||text.Contains("lab"):
                    return DSC_TYPES.LAB;
                break;

                case var text when text.Contains("атестація")||text.Contains("atest"):
                    return DSC_TYPES.ATEST;
                break;
                    
                case var text when text.Contains("ісп")||text.Contains("exam"):
                    return DSC_TYPES.EXAM;
                break;

                default: return DSC_TYPES.NONE;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (Link != "NONE") stringBuilder.Append($"Link: {Link}\n");
            if (LinkId != "NONE") stringBuilder.Append($"Connect ID: {LinkId}\n");
            if (Pass != "NONE") stringBuilder.Append($"Password: {Pass}");

            return stringBuilder.ToString();
        }
    }

}
