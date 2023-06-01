using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class Group
    {
        public long Id { get; set; }
        public string GrpName  { get; set; }

        public const int nullGrpId = -1;
        public const string nullGrpName = "NONE";

        public Group() 
        { 
            Id = nullGrpId;
            GrpName = nullGrpName;
        }
        public Group(long id, string grpName)
        {
            Id = id;
            GrpName = grpName;
        }
        public Group(string strValues)
        {
            string []res = strValues.Split(':');
            res[0] = res[0].Trim(' ');
            Id = long.Parse(res[0]);
            GrpName = string.IsNullOrEmpty(res[1]) || string.IsNullOrWhiteSpace(res[1]) ? "NONE" : (res[1].Trim(' '));
        }

        public override string ToString()
        {
            return $"GRP[{Id}:{GrpName}]";
        }
    }
}
