using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class User
    {
        public long Id { get; set; }
        public long GrpID { get; set; }

        public UserStatus Status { get; set; }

        public User() 
        {
            GrpID = Group.nullGrpId;
        }
        public User(long Id, long grpId = Group.nullGrpId, UserStatus status = UserStatus.NeedRegistration) 
        {
            this.Id = Id;
            this.GrpID = grpId;
            this.Status = status;
        }
        public User(UserStatus status) 
        {
            Status = status;
        }
        public User(long id, UserStatus status) 
        {
            Id = id;
            Status = status;
        }

        public enum UserStatus
        {
            NeedRegistration = 0,
            StartRegistration = 1,
            InRegistrationProcess = 2,
            IsRegisteredGroup = 3,
            IsStartRegisterStarosta = 4,
            IsRegisteredStarosta = 5
        }
    }
}
