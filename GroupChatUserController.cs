using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class GroupChatUserController
    {
        private static GroupChatUserController s_instance;

        private GroupChatUserController() 
        {

        }

        public static GroupChatUserController getInstance() { return s_instance ??= new GroupChatUserController(); }


    }
}
