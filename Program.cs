using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Npgsql;

namespace TelegramBotTest
{
    class Program
    {
        static TelegramBotClient Bot;
        static IConfigurationRoot logConfig;
        static ILoggerFactory loggerFactory;
        static DBWorker DB;
        static string UrlSourceJson = ConfigurationManager.AppSettings.Get("UrlJsonSource");
        static ILogger<Program> s_logger;

        static void logInit() 
        {
            logConfig = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false, true)
                                .Build();
            loggerFactory = LoggerFactory.Create(
                builder =>
                {
                    builder.AddConfiguration(logConfig.GetSection("Logging"));
                    builder.AddConsole();
                }
            );
        }
        static void DBInit()
        {
            DB = DBWorker.getInstance(
                ConfigurationManager.AppSettings.Get("DBHost"),
                ConfigurationManager.AppSettings.Get("DBUser"),
                ConfigurationManager.AppSettings.Get("DBName"),
                ConfigurationManager.AppSettings.Get("DBPort"),
                ConfigurationManager.AppSettings.Get("DBPass"),
                getLogger<DBWorker>()
            );
        }
        static void TbotInit() 
        {
            Bot = new TelegramBotClient(ConfigurationManager.AppSettings.Get("BotToken"));
            Bot.OnMessage += botOnMessage;
            Bot.OnCallbackQuery += botOnCallbackQuery;
            Bot.StartReceiving(new UpdateType[] { UpdateType.Message, UpdateType.CallbackQuery });
        }
        
        
        static ILogger<T> getLogger<T>() { return loggerFactory.CreateLogger<T>(); }

        static void Main(string[] args)
        {
            if (true)
            {
                Console.OutputEncoding = Encoding.Unicode;
                logInit();
                s_logger = getLogger<Program>();
                s_logger.Log(LogLevel.Information, "Start initialization DBworker");
                DBInit();
                s_logger.Log(LogLevel.Information, "Start initialization Telegram Bot");
                TbotInit();
                s_logger.Log(LogLevel.Information, "Start initialization UserController");
            }
            else
            {
                TEST();
            }
            Console.ReadLine();
        }
        static void TEST() 
        {
            logInit();
            DBInit();
            Console.OutputEncoding = Encoding.Unicode;
            Scheduler.getInstance(DB, getLogger<Scheduler>()).UpdateGroup(UrlSourceJson);
        }

        static void botOnMessage(object? sender, MessageEventArgs e)
        {
            s_logger.Log(LogLevel.Information, "Bot has been get message");
            Console.WriteLine($"[\n From: {e.Message.From}\n IsBot:{e.Message.From.IsBot}\n UserName:{e.Message.From.Username}\n Time:{e.Message.Date.ToLocalTime()}\n Text:{e.Message.Text}\n]");

            if (isInvalidMessage(e)) return;
            bool isPrivateChat = e.Message.Chat.Type == ChatType.Private 
                || e.Message.Chat.Type == ChatType.Sender;

            if (isPrivateChat)
            {
                OnPrivatChatHandler(e);
            }
            else 
            {
                OnGroupChatHandler(e);
            }
        }

        static void OnPrivatChatHandler(MessageEventArgs e) 
        {
            var text = e.Message.Text;
            UserController userController = UserController.getInstance();
            BotController botController = BotController.getInstance(Bot);

            //YesNo->Enter=>UnWaitEner&&UnWaitYesNo
            if (userController.IsInCreateAltPair(e.Message.From.Id)) 
            {
                OnPrivatChatCreateAltPair(e, DB.readUser(e.Message.From.Id).GrpID);
                return;
            }
            else if (userController.IsInWaitStarostaEnterZoomPass(e.Message.From.Id))
            {
                User _user = DB.readUser(e.Message.From.Id);

                if (string.IsNullOrEmpty(text)||string.IsNullOrWhiteSpace(text))
                {
                    text = new string("NONE");
                }
                PairLink pairLink = userController.getPairLinkInWaitStarostaYesNoZoomPass(_user.Id);
                pairLink.Pass = text;

                userController.UnWaitStarostaYesNoZoomPass(_user);
                userController.UnWaitStarostaEnterZoomPass(_user);

                DB.InsertOrUpdatePairLink(pairLink);
                return;
            }
            else if (userController.IsInWaitStarostaEnterZoomIdLink(e.Message.From.Id)) 
            {
                User _user = DB.readUser(e.Message.From.Id);

                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    text = new string("NONE");
                }
                PairLink pairLink = userController.getPairLinkInWaitStarostaYesNoZoomIdLink(_user.Id);
                pairLink.LinkId = text;
                userController.UnWaitStarostaYesNoZoomIdLink(_user);
                userController.UnWaitStarostaEnterZoomIdLink(_user);
                userController.AddToWaitStarostaYesNoZoomPass(_user.Id, pairLink);
                botController.sendYesNo(e.Message.Chat.Id, "Assign Password to link ?");
                return;

            } 
            else if (userController.IsInWaitStarostaDonePairLink(e.Message.From.Id))
            {
                User _user = DB.readUser(e.Message.From.Id);

                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    text = new string("NONE");
                }
                PairLink pairLink = userController.getPairLinkInWaitStarostaDonePairLink(e.Message.From.Id);
                pairLink.Link = text;
                userController.UnWaitStarostaDonePairLink(_user);
                botController.sendYesNo(e.Message.Chat.Id, "Assign Conference Id(Zoom)?");
                userController.AddToWaitStarostaYesNoZoomIdLink(e.Message.From.Id, pairLink);
                return;
            }

            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)) return;

            var fromId = e.Message.From.Id;
            var chat = e.Message.Chat;

            User user = DB.readUser(fromId);
            user ??= new User(fromId);

            if (user.GrpID == Group.nullGrpId || userController.IsInRegistration(user))
            {
                OnRegistrationHandler(e, user);
            } 
            else 
            {
                switch (text) 
                {
                    case var message when message.Contains("/alt"):
                        
                        if (DB.isStarosta(fromId)) 
                        {
                            userController.AddToInCreateAltPair(fromId);
                            Bot.SendTextMessageAsync(chat, "Enter date for alternative pair in format(dd.mm.YYYY).\nExample: \'31.05.23\'");
                            userController.AddToWaitEnterDateAltPair(fromId);
                            return;
                        } 
                        else 
                        {
                            Bot.SendTextMessageAsync(chat, "Command disabled.\nThis command able only for User type Starosta!");
                            return;
                        }
                    break;

                    case var message when message.Contains("/now") && message.Contains(":"):
                        Time time = BotController.getTimeInMessage(message);
                        Pair pair = Scheduler.getInstance(DB, getLogger<Scheduler>()).getPairDayNow(user, UrlSourceJson, "method=getSchedules&id_grp=")
                            .getPairNow(time);

                        if (pair != null)
                        {
                            PairLink link = DB.readParLink(long.Parse(pair.KOD_DISC), user.GrpID, PairLink.getDscType(pair.TypePair));
                            pair.WorkLink = link;
                            AltPair altPair = DB.readAltPair(pair.DateReg, AltPair.ToNumberPair(pair.NumPair), user.GrpID);

                            if (altPair == null)
                            {
                                botController.sendPair(chat.Id, pair);
                            } else 
                            {
                                //send Alt Pair
                                botController.sendAltPair(chat.Id, altPair);
                            }
                        }
                        else
                        {
                            //FInd all alt Pair
                            List<AltPair> altPairs = DB.readAltPairs(Pair.getNormalDate(e.Message.Date), user.GrpID);

                            if (altPairs != null && altPairs.Count > 0)
                            {
                                List<TimeInterval> intervals = new List<TimeInterval>();
                                foreach (var altPair in altPairs)
                                {
                                    TimeInterval _interval = AltPair.NumberPairToTimeInterval(altPair.NUM_PAIR);

                                    if (_interval != null) intervals.Add(_interval);
                                }
                                TimeInterval interval = TimeInterval.getCloserTimeInterval(
                                    TimeInterval.getSortedListTimeInterval(intervals), time);

                                if (interval != null)
                                {
                                    AltPair res = altPairs.Find(x => AltPair.NumberPairToTimeInterval(x.NUM_PAIR).Start.Hour == interval.Start.Hour);

                                    if (res != null)
                                    {
                                        botController.sendAltPair(chat.Id, res);
                                    }
                                    else
                                    {
                                        Bot.SendTextMessageAsync(chat.Id, "Pair didn't Found!");
                                    }
                                }
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(chat.Id, "Pair didn't Found!");
                            }
                        }
                    break;

                    case var message when message.Contains("/now"):
                        var timeLoc = e.Message.Date.ToLocalTime();
                        Time _time = new Time(timeLoc.Hour, timeLoc.Minute);
                        Pair _pair = Scheduler.getInstance(DB, getLogger<Scheduler>()).getPairDayNow(user, UrlSourceJson, "method=getSchedules&id_grp=")
                            .getPairNow(_time);

                        if (_pair != null)
                        {
                            PairLink link = DB.readParLink(long.Parse(_pair.KOD_DISC), user.GrpID, PairLink.getDscType(_pair.TypePair));
                            _pair.WorkLink = link;
                            AltPair altPair = DB.readAltPair(_pair);

                            if (altPair == null)
                            {
                                botController.sendPair(chat.Id, _pair);
                            }
                            else
                            {
                                //send Alt Pair
                                botController.sendAltPair(chat.Id, altPair);
                            }
                        }
                        else
                        {
                            //FInd all alt Pair
                            List<AltPair> altPairs = DB.readAltPairs(Pair.getNormalDate(e.Message.Date), user.GrpID);

                            if (altPairs != null && altPairs.Count > 0)
                            {
                                List<TimeInterval> intervals = new List<TimeInterval>();
                                foreach (var altPair in altPairs)
                                {
                                    TimeInterval _interval = AltPair.NumberPairToTimeInterval(altPair.NUM_PAIR);

                                    if (_interval != null) intervals.Add(_interval);
                                }
                                TimeInterval interval = TimeInterval.getCloserTimeInterval(
                                    TimeInterval.getSortedListTimeInterval(intervals), _time);

                                if (interval != null)
                                {
                                    AltPair res = altPairs.Find(x => AltPair.NumberPairToTimeInterval(x.NUM_PAIR).Start.Hour == interval.Start.Hour);

                                    if (res != null)
                                    {
                                        botController.sendAltPair(chat.Id, res);
                                    }
                                    else
                                    {
                                        Bot.SendTextMessageAsync(chat.Id, "Pair didn't Found!");
                                    }
                                }
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(chat.Id, "Pair didn't Found!");
                            }
                        }
                        break;

                    case var message when message.Contains("/link"):

                        if (DB.isStarosta(user)) 
                        {
                            string[] date = {
                                Pair.getUnNormalDate(Pair.getNormalDate(DateTime.Now)),
                                Pair.getUnNormalDate(Pair.getNormalDate(DateTime.Now.AddMonths(1)))
                            };
                            PairDays pd = Scheduler.getInstance(DB, getLogger<Scheduler>()).getPairDays(user, UrlSourceJson, "method=getSchedules&id_grp=", date);
                            userController.AddToWaitStarostaSelectDsc(user);
                            botController.sendDscList(chat.Id, pd.getDesciplines(), "Select Discipline from Automatic Search list:");
                        } 
                        else 
                        {
                            Bot.SendTextMessageAsync(chat, "You aren't Starosta, so this function is disabled!");
                        }
                    break;

                    case var message when message.Contains("/reg"):

                        if (!DB.isStarosta(user)) 
                        {
                            if (DB.readCountStarosta(user.GrpID) < 2)
                            {
                                botController.sendYesNo(chat.Id, "You realy wunted be Starosta ?");
                                userController.AddToWaitUserStarostaClick(user);
                            } 
                            else 
                            {
                                Bot.SendTextMessageAsync(chat, "Your group already has enough starosta!");
                            }
                        } 
                        else 
                        {
                            Bot.SendTextMessageAsync(chat, "You are Starosta already!");
                        }

                    break;

                    default: Bot.SendTextMessageAsync(chat, "This User is Registered");
                    break;
                }
            }
        }
        static void OnGroupChatHandler(MessageEventArgs e) 
        {
            var chat = e.Message.Chat;
            var text = e.Message.Text;
            BotController botController = BotController.getInstance(Bot);

            if (!DB.isExistGroupChat(chat.Id)) 
            {
                OnGroupChatRegistrationHandler(e, chat.Id);
            } 
            else 
            {
                long grpId = DB.getGrpIdOfGroupChat(chat.Id);

                switch(text.ToLower()) 
                {
                    case var message when message.Contains("/now") && message.Contains(":"):
                        Time time = BotController.getTimeInMessage(message);
                        Pair pair = Scheduler.getInstance(DB, getLogger<Scheduler>()).getPairDayNow(grpId, UrlSourceJson, "method=getSchedules&id_grp=")
                            .getPairNow(time);

                        if (pair != null)
                        {
                            PairLink link = DB.readParLink(long.Parse(pair.KOD_DISC), grpId, PairLink.getDscType(pair.TypePair));
                            pair.WorkLink = link;
                            AltPair altPair = DB.readAltPair(pair.DateReg, AltPair.ToNumberPair(pair.NumPair), grpId);

                            if (altPair == null)
                            {
                                botController.sendPair(chat.Id, pair);
                            }
                            else
                            {
                                //send Alt Pair
                                botController.sendAltPair(chat.Id, altPair);
                            }
                        }
                        else
                        {
                            //FInd all alt Pair
                            List<AltPair> altPairs = DB.readAltPairs(Pair.getNormalDate(e.Message.Date), grpId);

                            if (altPairs != null && altPairs.Count > 0)
                            {
                                List<TimeInterval> intervals = new List<TimeInterval>();
                                foreach (var altPair in altPairs)
                                {
                                    TimeInterval _interval = AltPair.NumberPairToTimeInterval(altPair.NUM_PAIR);

                                    if (_interval != null) intervals.Add(_interval);
                                }
                                TimeInterval interval = TimeInterval.getCloserTimeInterval(
                                    TimeInterval.getSortedListTimeInterval(intervals), time);

                                if (interval != null)
                                {
                                    AltPair res = altPairs.Find(x => AltPair.NumberPairToTimeInterval(x.NUM_PAIR).Start.Hour == interval.Start.Hour);

                                    if (res != null)
                                    {
                                        botController.sendAltPair(chat.Id, res);
                                    }
                                    else
                                    {
                                        Bot.SendTextMessageAsync(chat.Id, "Pair didn't Found!");
                                    }
                                }
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(chat.Id, "Pair didn't Found!");
                            }
                        }
                        break;

                    case var message when message.Contains("/now"):
                        var timeLoc = e.Message.Date.ToLocalTime();
                        Time _time = new Time(timeLoc.Hour, timeLoc.Minute);
                        Pair _pair = Scheduler.getInstance(DB, getLogger<Scheduler>()).getPairDayNow(grpId, UrlSourceJson, "method=getSchedules&id_grp=")
                            .getPairNow(_time);

                        if (_pair != null)
                        {
                            PairLink link = DB.readParLink(long.Parse(_pair.KOD_DISC), grpId, PairLink.getDscType(_pair.TypePair));
                            _pair.WorkLink = link;
                            AltPair altPair = DB.readAltPair(_pair);

                            if (altPair == null)
                            {
                                botController.sendPair(chat.Id, _pair);
                            }
                            else
                            {
                                //send Alt Pair
                                botController.sendAltPair(chat.Id, altPair);
                            }
                        }
                        else
                        {
                            //FInd all alt Pair
                            List<AltPair> altPairs = DB.readAltPairs(Pair.getNormalDate(e.Message.Date), grpId);

                            if (altPairs != null && altPairs.Count > 0)
                            {
                                List<TimeInterval> intervals = new List<TimeInterval>();
                                foreach (var altPair in altPairs)
                                {
                                    TimeInterval _interval = AltPair.NumberPairToTimeInterval(altPair.NUM_PAIR);

                                    if (_interval != null) intervals.Add(_interval);
                                }
                                TimeInterval interval = TimeInterval.getCloserTimeInterval(
                                    TimeInterval.getSortedListTimeInterval(intervals), _time);

                                if (interval != null)
                                {
                                    AltPair res = altPairs.Find(x => AltPair.NumberPairToTimeInterval(x.NUM_PAIR).Start.Hour == interval.Start.Hour);

                                    if (res != null)
                                    {
                                        botController.sendAltPair(chat.Id, res);
                                    }
                                    else
                                    {
                                        Bot.SendTextMessageAsync(chat.Id, "Pair didn't Found!");
                                    }
                                }
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(chat.Id, "Pair didn't Found!");
                            }
                        }
                        break;

                    case var messge when messge.Contains("/menu"):
                        Bot.SendTextMessageAsync(chat, "\'/now\' -get Pair now.\n'/now 00:00' - get Pair for any time.");
                    break;

                    default: 
                        Bot.SendTextMessageAsync(chat, "Unknown command! here list command \'/menu\'");
                    break;
                }
            }

        }
        static void OnRegistrationHandler(MessageEventArgs e, User user) 
        {
            var fromId = e.Message.From.Id;
            var chat = e.Message.Chat;
            var text = e.Message.Text;

            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)) return;
            
            UserController userController = UserController.getInstance();
            BotController botController = BotController.getInstance(Bot);

            switch(text.ToLower()) 
            {
                case var str when str.Contains("/reg"):
                    userController.AddToRegistrationUser(user);
                    userController.AddToWaitUserInput(user);
                    botController.sendRegisterText(user, chat.Id);
                break;

                case var str when !str.Contains("/") && userController.IsInWaitUserInput(user):
                    if (botController.sendGroupTgListButtons(DB.getGroupsByShortName(text), chat.Id))
                    {
                        userController.UnWaitUserInput(user);
                    } 
                break;

                default:
                    Bot.SendTextMessageAsync(chat, "Menu don't useability. Need reg(use \'/reg\')");
                break;

            }
        }

        static void OnPrivatChatCreateAltPair(MessageEventArgs e, long stdGrpId) 
        {
            var chat = e.Message.Chat;
            var fromId = e.Message.From.Id;
            var text = e.Message.Text;
            UserController userController = UserController.getInstance();

            if (userController.IsInWaitEnterInfoAltPair(fromId)) 
            {
                if (string.IsNullOrEmpty(text)||string.IsNullOrWhiteSpace(text)||text == "NONE") 
                {
                    Bot.SendTextMessageAsync(chat, "Entered Info is empty. Enter correct info:");
                    return;
                } 
                else 
                {
                    AltPair altPair = userController.getAltPairWaitEnterInfoAltPair(fromId);
                    altPair.INFO = text;
                    userController.UnWaitEnterInfoAltPair(fromId);
                    DB.InsertOrUpdateAltPair(altPair);
                    userController.UnInCreateAltPair(fromId);
                }
            }
            else if (userController.IsInWaitEnterNumberAltPair(fromId)) 
            {
                int num = -1;

                try { num = int.Parse(text); } catch { num = -1; };

                AltPair.NUMBER_PAIR numP = AltPair.ToNumberPair(num);

                if (numP == AltPair.NUMBER_PAIR.NONE) 
                {
                    Bot.SendTextMessageAsync(chat, "Wrong number of Pair! Repeat Enter:");
                    return;
                } 
                else 
                {
                    AltPair altPair = new AltPair(userController.getStrWaitEnterNumberAltPair(fromId),
                        numP, stdGrpId
                    );
                    Bot.SendTextMessageAsync(chat, "Enter info, name or short comment of what's it:");
                    userController.UnWaitEnterNumberAltPair(fromId);
                    userController.AddToWaitEnterInfoAltPair(fromId, altPair);
                    return;
                }
            }
            else if (userController.IsInWaitEnterDateAltPair(fromId)) 
            {
                AltPair altPair = new AltPair(text);
                if (altPair == null||string.IsNullOrEmpty(altPair.DATE_REG)
                    ||string.IsNullOrWhiteSpace(altPair.DATE_REG)) 
                {
                    Bot.SendTextMessageAsync(chat, "Uncorrect date format! Repeat Enter:");
                    return;
                }
                else 
                {
                    userController.AddToWaitEnterNumberAltPair(fromId, altPair.DATE_REG);
                    Bot.SendTextMessageAsync(chat, "Enter Number of the Pair from 1 to 8:");
                    userController.UnWaitEnteDaterAltPair(fromId);
                }
            }
        }

        static void OnGroupChatRegistrationHandler(MessageEventArgs e, long chatId) 
        {
            var text = e.Message.Text;
            GroupChatUserController grpController = GroupChatUserController.getInstance();

            switch(text.ToLower()) 
            {
                case var message when message.Contains("/reg") : 

                    if (DB.isStarosta(e.Message.From.Id))
                    {
                        User user = DB.readUser(e.Message.From.Id);
                        DB.InsertGroupChat(chatId, user.GrpID);
                        Bot.SendTextMessageAsync(chatId, "Group has been registered!");
                    } 
                    else 
                    {
                        Bot.SendTextMessageAsync(chatId, "You aren't Starosta.\nYou cann't register this Group!");
                    }
                break;

                case var message when message.Contains("/alt") :
                    Bot.SendTextMessageAsync(chatId, "This command only for Privat Chat!");
                break;

                default: 
                    Bot.SendTextMessageAsync(chatId, "Commands and menu don't ability. Registr this Group!");
                break;
            }
        }

        static void botOnCallbackQuery(object? sender, CallbackQueryEventArgs e) 
        {
            s_logger.LogInformation($"\nbotOnCallbackQuery: from {e.CallbackQuery.From.Id}, data: {e.CallbackQuery.Data}");
            var data = e.CallbackQuery.Data;
            var fromId = e.CallbackQuery.From.Id;
            var message = e.CallbackQuery.Message;
            UserController userController = UserController.getInstance();
            User user = DB.readUser(fromId);
            user ??= new User(fromId, Group.nullGrpId);

            //YesNo->Enter=>UnWaitEner&&UnWaitYesNo

            if (userController.IsInWaitStarostaYesNoZoomPass(fromId))
            {
                Bot.DeleteMessageAsync(message.Chat, message.MessageId);

                if (data.Contains("/yes")) 
                {
                    Bot.SendTextMessageAsync(message.Chat, "Enter Zoom Password:");
                    userController.AddToWaitStarostaEnterZoomPass(user);
                } 
                else 
                {
                    // go to insert Pairlink to DB
                    PairLink pairLink = userController.getPairLinkInWaitStarostaYesNoZoomPass(user.Id);

                    //pairLink.Pass = "NONE";
                    userController.UnWaitStarostaYesNoZoomPass(user);
                    DB.InsertOrUpdatePairLink(pairLink);
                }
            }
            else if (userController.IsInWaitStarostaYesNoZoomIdLink(fromId)) 
            {
                Bot.DeleteMessageAsync(message.Chat, message.MessageId);

                if (data.Contains("/yes")) 
                {
                    Bot.SendTextMessageAsync(message.Chat, "Enter Zoom Conference Id:");
                    userController.AddToWaitStarostaEnterZoomIdLink(user);
                }
                else 
                {
                    //go to pass

                    PairLink pairLink = userController.getPairLinkInWaitStarostaYesNoZoomIdLink(user.Id);

                    if (DB.readCountPairLink(pairLink.DSC_ID, user.GrpID, pairLink.DSC_TYPE) < 0)
                    {
                        pairLink.LinkId = "NONE";
                    }
                    else 
                    {
                        PairLink _pairLink = DB.readParLink(pairLink.DSC_ID, user.GrpID, pairLink.DSC_TYPE);
                        if (_pairLink != null) pairLink.LinkId = _pairLink.LinkId;
                    }
                    userController.UnWaitStarostaYesNoZoomIdLink(user);
                    userController.AddToWaitStarostaYesNoZoomPass(user.Id, pairLink);
                    BotController.getInstance(Bot).sendYesNo(message.Chat.Id, "Assign Password to link ?");
                }
            }
            else if (userController.IsInWaitStarostaLinkYesNoClick(user))
            {
                Bot.DeleteMessageAsync(message.Chat, message.MessageId);
                userController.UnWaitStarostaLinkYesNoClick(user);

                long dscId = long.Parse(data.Split("=")[1]);
                PairLink.DSC_TYPES type = PairLink.getDscType(data.Split("=")[2]);
                PairLink pairLink = new PairLink(dscId, user.GrpID, type);

                if (data.Contains("/yes"))
                {
                    Bot.SendTextMessageAsync(message.Chat, "Enter Link");
                    userController.AddToWaitStarostaDonePairLink(fromId, pairLink);
                }
                else
                {
                    //Go to Enter Zoom Link Id and Pass
                    Bot.SendTextMessageAsync(message.Chat, "No");

                    if (DB.readCountPairLink(dscId, user.GrpID, type) < 0)
                    {
                        pairLink.Link = "NONE";
                    }
                    else 
                    {
                        pairLink = DB.readParLink(dscId, user.GrpID, type);
                    }
                    BotController.getInstance(Bot).sendYesNo(message.Chat.Id, "Assign Conference Id(Zoom)?");
                    userController.AddToWaitStarostaYesNoZoomIdLink(fromId, pairLink);
                }
            }
            else if (userController.IsInWaitStarostaSelectLinkTypeClick(user))
            {
                long dscId = long.Parse(data.Split("=")[0]);
                PairLink.DSC_TYPES type = PairLink.getDscType(data.Split("=")[1]);
               
                BotController.getInstance(Bot).sendYesNo(message.Chat.Id, "Assign link ?", $"/yes={dscId}={type}", $"/no={dscId}={type}");
                Bot.DeleteMessageAsync(message.Chat, message.MessageId);
                userController.AddToWaitStarostaLinkYesNoClick(user);
                userController.UnWaitStarostaSelectLinkTypeClick(user);
            }
            else if (userController.IsInWaitStarostaSelectDsc(user))
            {
                Bot.DeleteMessageAsync(message.Chat, message.MessageId);
                userController.UnWaitStarostaSelectDsc(user);
                BotController.getInstance(Bot).sendLinkDscTypeListButtons(message.Chat.Id,
                    long.Parse(data), "Select Type Link:"
                );
                userController.AddToWaitStarostaSelectLinkClick(user);
            }
            else if (userController.IsInWaitUserStarostaClick(user))
            {
                Bot.DeleteMessageAsync(message.Chat, message.MessageId);

                if (data.Contains("/yes")) 
                {
                    if (DB.readCountStarosta(user.GrpID) > 2)
                    {
                        Bot.SendTextMessageAsync(message.Chat, "You was late! Starosta has been choosed early.");
                    } 
                    else 
                    {
                        DB.insertStarosta(user);
                        Bot.SendTextMessageAsync(message.Chat, "Now, You are Starosta of your Group.\nYou have some ability for controll.");
                    }
                } 
                else
                {
                    Bot.SendTextMessageAsync(message.Chat, "You refused be Starosta!");
                }
                userController.UnWaitUserStarostaClick(user);
                userController.UnRegistrationUser(user);
            } 
            else if (userController.IsInRegistration(user))
            {
                long grpId = Convert.ToInt64(data);
                User _user = new User(fromId, grpId);
                DB.insertUser(_user);

                Group g = DB.readGroup(grpId);

                Bot.SendTextMessageAsync(message.Chat, $"You has selected: {g?.GrpName}");
                Bot.DeleteMessageAsync(message.Chat, message.MessageId);

                if (DB.readCountStarosta(grpId) < 2)
                {
                    userController.AddToWaitUserStarostaClick(_user);
                    BotController.getInstance(Bot).sendYesNo(message.Chat.Id, "Do you wunted be Starosta ?");
                } else 
                {
                    userController.UnRegistrationUser(_user);
                }
            }
        }
        static bool isInvalidMessage(MessageEventArgs e) 
        {
            if (e == null || e.Message == null || e.Message.Text == null
                || string.IsNullOrEmpty(e.Message.Text) || string.IsNullOrWhiteSpace(e.Message.Text))
            {
                return true;
            }
            return false;
        }
    }
}
