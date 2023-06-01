using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotTest
{
    class BotController
    {
        private TelegramBotClient _Bot;
        private static BotController s_instance;

        private BotController(TelegramBotClient bot) { _Bot = bot; }
        public static BotController getInstance(TelegramBotClient bot) { return s_instance ??= new BotController(bot); }

        public int getNumberButtonRows(int count)
        {
            if (count == 0) return 0;
            int numberRows = (count) / 5;
            numberRows += (count % 5 != 0) ? 1 : 0;
            return numberRows;
        }
        public void sendRegisterText(User user, long chatId)
        {
            string text = "Enter Group Name.(may one letter):";
            _Bot?.SendTextMessageAsync(chatId, text);
        }
        public void sendDscList(long chatId, SortedList<long, string> disciplines, string message)
        {
            List<List<InlineKeyboardButton>> rowsList = new List<List<InlineKeyboardButton>>();
            foreach (var dsc in disciplines)
            {
                InlineKeyboardButton keyboardButton = new InlineKeyboardButton();
                keyboardButton.Text = dsc.Value;
                keyboardButton.CallbackData = dsc.Key.ToString();
                List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
                row.Add(keyboardButton);
                rowsList.Add(row);
            }
            InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup(rowsList);
            sendReplyMarkup(chatId, message, keyboardMarkup);
        }
        public bool sendGroupTgListButtons(Group[] grpList, long chatId)
        {
            if (grpList == null || grpList.Length == 0)
            {
                _Bot.SendTextMessageAsync(chatId, "Not Found Any Group! Repeat Input");
                return false;
            }

            int numberRows = getNumberButtonRows(grpList.Length);

            List<List<InlineKeyboardButton>> rowsList = new List<List<InlineKeyboardButton>>();
            for (int i = 0; i < numberRows; i++)
            {
                List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
                rowsList.Add(keyboardButtons);
            }

            for (int i = 0; i < grpList.Length; i++)
            {
                InlineKeyboardButton inlineKeyboardButton = new InlineKeyboardButton();
                inlineKeyboardButton.Text = grpList[i].GrpName;
                inlineKeyboardButton.CallbackData = $"{grpList[i].Id}";

                int index = i + 1;
                index = index / 5;
                index = (i + 1) % 5 == 0 ? --index : index;

                rowsList[index].Add(inlineKeyboardButton);
            }
            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardMarkup(rowsList);
            sendReplyMarkup(chatId, "Select:", inlineKeyboardMarkup);
            return true;
        }
        public void sendYesNo(long chatId, string question, string specYes = null, string specNo = null)
        {
            InlineKeyboardButton keyYes = new InlineKeyboardButton();
            InlineKeyboardButton keyNo = new InlineKeyboardButton();
            keyYes.Text = "Yes";
            keyYes.CallbackData = specYes ??= "/yes";
            keyNo.Text = "No";
            keyNo.CallbackData = specNo ??= "/no";
            List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
            row.Add(keyYes);
            row.Add(keyNo);
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(row);
            sendReplyMarkup(chatId, question, inlineKeyboard);
        }
        public void sendPair(long chatId, Pair pair)
        {
            string text = $"{pair.NumPair} [{pair.TypePair}]\n{pair.ShortNamePair}\nWho : {pair.TeacherName}\n";
            text = $"{text}\nTime : {pair.PairTime}\n";

            if (!string.IsNullOrEmpty(pair.Auditoria)||!string.IsNullOrWhiteSpace(pair.Auditoria))
            {
                text = $"{text}\nAuditoria: {pair.Auditoria}\n";
            }
            else
            {
                //text = $"{text}\n";

                if (pair.WorkLink != null) 
                {
                    text = $"{text}{pair.WorkLink}";
                }
            }
            _Bot.SendTextMessageAsync(chatId, text);
        }
        public void sendAltPair(long chatId, AltPair altPair) 
        {
            string text = $"\nFound Alternative Pair:\n[{AltPair.NumberPairToString(altPair.NUM_PAIR)}]\nInfo: {altPair.INFO}\n";
            text = $"{text}Time: {AltPair.NumberPairToTimeInterval(altPair.NUM_PAIR)}\n";
            text = $"{text}Date: {Pair.getUnNormalDate(altPair.DATE_REG)}";
            _Bot.SendTextMessageAsync(chatId, text);
        }

        public void sendLinkDscTypeListButtons(long chatId, long dscId, string message) 
        {
            string[] types = { "лекція", "лабораторна\n/практична", "атестація", "іспит" };
            List<InlineKeyboardButton> keyboardButtons = new List<InlineKeyboardButton>();
            foreach(var type in types) 
            {
                keyboardButtons.Add(getButtonDscType(dscId, type));
            }
            InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup(keyboardButtons);
            sendReplyMarkup(chatId, message, keyboardMarkup);
        }

        public void sendReplyMarkup(long chatId, string message, IReplyMarkup replyMarkup) 
        {
            _Bot.SendTextMessageAsync(chatId, message, ParseMode.Default, 
                null, false, false, 0, false, replyMarkup
            );
        }

        public InlineKeyboardButton getButtonDscType(long dscId, string type) 
        {
            InlineKeyboardButton keyboardButton = new InlineKeyboardButton();
            keyboardButton.Text = type;
            keyboardButton.CallbackData = $"{dscId}={PairLink.getDscType(type)}";
            return keyboardButton;
        }
        public static Time getTimeInMessage(string message) 
        {
            string strhour = message.Split("/now")[1].Split(":")[0];
            string strminute = message.Split("/now")[1].Split(":")[1];

            int hour;
            int.TryParse(string.Join("", strhour.Where(c => char.IsDigit(c))), out hour);

            int minute;
            int.TryParse(string.Join("", strminute.Where(c => char.IsDigit(c))), out minute);

            Time responseTime = new Time(hour, minute);
            return responseTime;
        }
    }
}
