using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotTest
{
    class UserController
    {
        private List<User> _userRegistrationList;
        private List<User> _waitUserRegInputList;
        private List<User> _waitUserStarostRegClickList;

        private List<User> _waitStarostaSelectDscList;
        private List<User> _waitStarostaSelectLinkTypeClickList;
        private List<User> _waitStarostaLinkYesNoClick;
        private List<User> _waitStarostaLinkEnter;
        private List<User> _waitStarostaEnterZoomIdLink;
        private List<User> _waitStarostaEnterZoomPass;

        private List<long> _InCreateAltPair;
        private List<long> _waitEnterDateAltPair;

        /**
         * <summary>
         *  Contain string DATE_REG of AltPair
         *  for user with userId
         * </summary>
         */
        private SortedList<long, string> _waitEnterNumberAltPair;
        /**
         * <summary>
         * For contain AltPair for User 
         * with long userId
         * </summary>
         */
        private SortedList<long, AltPair> _waitEnterInfoAltPair;

        private SortedList<long,PairLink> _waitStarostaDonePairLinkList;
        private SortedList<long, PairLink> _waitStarostaYesNoZoomIdLink;
        private SortedList<long, PairLink> _waitStarostaYesNoZoomPass;

        private static UserController s_instance;

        private UserController() 
        { 
            _userRegistrationList = new List<User>();
            _waitUserRegInputList = new List<User>();
            _waitUserStarostRegClickList = new List<User>();

            _waitStarostaSelectDscList = new List<User>();
            _waitStarostaSelectLinkTypeClickList = new List<User>();
            _waitStarostaLinkYesNoClick = new List<User>();
            _waitStarostaLinkEnter = new List<User>();
            _waitStarostaEnterZoomIdLink = new List<User>();
            _waitStarostaEnterZoomPass = new List<User>();

            _InCreateAltPair = new List<long>();
            _waitEnterDateAltPair = new List<long>();
            _waitEnterNumberAltPair = new SortedList<long, string>();
            _waitEnterInfoAltPair = new SortedList<long, AltPair>();

            _waitStarostaDonePairLinkList = new SortedList<long, PairLink>();
            _waitStarostaYesNoZoomIdLink = new SortedList<long, PairLink>();
            _waitStarostaYesNoZoomPass = new SortedList<long, PairLink>();
        }
        public static UserController getInstance() { return s_instance ??= new UserController(); }

        #region Get Index
        public int getUserRegistrationIndex(long userId) { return _userRegistrationList.FindIndex(x => x.Id == userId); }
        public int getUserRegistrationIndex(User user)
        {
            if (user == null) return -1;
            return getUserRegistrationIndex(user.Id);
        }

        public int getWaitUserInputIndex(long userId) { return _waitUserRegInputList.FindIndex(x => x.Id == userId); }
        public int getWaitUserInputIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitUserInputIndex(user.Id);
        }

        public int getWaitUserStarostaRegClickIndex(long userId) { return _waitUserStarostRegClickList.FindIndex(x => x.Id == userId); }
        public int getWaitUserStarostaRegClickIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitUserStarostaRegClickIndex(user.Id);
        }

        public int getWaitStarostaSelectDscIndex(long userId) { return _waitStarostaSelectDscList.FindIndex(x => x.Id == userId); }
        public int getWaitStarostaSelectDscIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitStarostaSelectDscIndex(user.Id);
        }

        public int getWaitStarostaSelectLinkTypeClickIndex(long userId) { return _waitStarostaSelectLinkTypeClickList.FindIndex(x => x.Id == userId); }
        public int getWaitStarostaSelectLinkTypeClickIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitStarostaSelectLinkTypeClickIndex(user.Id);
        }

        public int getWaitStarostaLinkYesNoClickIndex(long userId) { return _waitStarostaLinkYesNoClick.FindIndex(x => x.Id == userId); }
        public int getWaitStarostaLinkYesNoClickIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitStarostaLinkYesNoClickIndex(user.Id);
        }

        public int getWaitStarostaLinkEnterIndex(long userId) { return _waitStarostaLinkEnter.FindIndex(x => x.Id == userId); }
        public int getWaitStarostaLinkEnterIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitStarostaLinkEnterIndex(user.Id);
        }

        public int getWaitStarostaDonePairLinkIndex(long userId) 
        {
            return _waitStarostaDonePairLinkList.IndexOfKey(userId);
        }
        public int getWaitStarostaDonePairLinkIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitStarostaDonePairLinkIndex(user.Id);
        }

        public int getWaitStarostaYesNoZoomIdLinkIndex(long userId) { return _waitStarostaYesNoZoomIdLink.IndexOfKey(userId); }
        public int getWaitStarostaYesNoZoomIdLinkIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitStarostaYesNoZoomIdLinkIndex(user.Id);
        }

        public int getWaitStarostaEnterZoomIdLinkIndex(long userId) { return _waitStarostaEnterZoomIdLink.FindIndex(x => x.Id == userId); }
        public int getWaitStarostaEnterZoomIdLinkIndex(User user)
        {
            if (user == null) return -1;
            return getWaitStarostaEnterZoomIdLinkIndex(user.Id);
        }

        public int getWaitStarostaYesNoZoomPassIndex(long userId) { return _waitStarostaYesNoZoomPass.IndexOfKey(userId); }
        public int getWaitStarostaYesNoZoomPassIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitStarostaYesNoZoomPassIndex(user.Id);
        }

        public int getWaitStarostaEnterZoomPassIndex(long userId) { return _waitStarostaEnterZoomPass.FindIndex(x => x.Id == userId); }
        public int getWaitStarostaEnterZoomPassIndex(User user) 
        {
            if (user == null) return -1;
            return getWaitStarostaEnterZoomPassIndex(user.Id);
        }

        public int getInCreateAltPairIndex(long userId) { return _InCreateAltPair.FindIndex(x => x == userId); }
        public int getWaitEnterDateAltPairIndex(long userId) { return _waitEnterDateAltPair.FindIndex(x => x == userId); }
        public int getWaitEnterNumberAltPairIndex(long userId) { return _waitEnterNumberAltPair.IndexOfKey(userId); }
        public int getWaitEnterInfoAltPairIndex(long userId) { return _waitEnterInfoAltPair.IndexOfKey(userId); }
        #endregion

        #region Is In
        public bool IsInRegistration(User user) { return getUserRegistrationIndex(user) != -1; }
        public bool IsInWaitUserInput(User user) { return getWaitUserInputIndex(user) != -1; }
        public bool IsInWaitUserStarostaClick(User user) { return getWaitUserStarostaRegClickIndex(user) != -1; }
        public bool IsInWaitStarostaSelectDsc(User user) { return getWaitStarostaSelectDscIndex(user) != -1; }
        public bool IsInWaitStarostaSelectLinkTypeClick(User user) { return getWaitStarostaSelectLinkTypeClickIndex(user) != -1; }
        public bool IsInWaitStarostaLinkYesNoClick(User user) { return getWaitStarostaLinkYesNoClickIndex(user) != -1; }
        public bool IsInWaitStarostaLinkEnter(User user) { return getWaitStarostaLinkEnterIndex(user) != -1; }

        public bool IsInWaitStarostaDonePairLink(long userId) { return getWaitStarostaDonePairLinkIndex(userId) != -1; }
        public bool IsInWaitStarostaDonePairLink(User user) { return getWaitStarostaDonePairLinkIndex(user.Id) != -1; }
        public bool IsInWaitStarostaYesNoZoomIdLink(long userId) { return getWaitStarostaYesNoZoomIdLinkIndex(userId) != -1; }
        public bool IsInWaitStarostaYesNoZoomIdLink(User user) { return getWaitStarostaYesNoZoomIdLinkIndex(user) != -1; }
        public bool IsInWaitStarostaEnterZoomIdLink(long userId) { return getWaitStarostaEnterZoomIdLinkIndex(userId) != -1; }
        public bool IsInWaitStarostaEnterZoomIdLink(User user) { return getWaitStarostaEnterZoomIdLinkIndex(user) != -1; }
        public bool IsInWaitStarostaYesNoZoomPass(long userId) { return getWaitStarostaYesNoZoomPassIndex(userId) != -1; }
        public bool IsInWaitStarostaYesNoZoomPass(User user) { return getWaitStarostaYesNoZoomPassIndex(user) != -1; }
        public bool IsInWaitStarostaEnterZoomPass(long userId) { return getWaitStarostaEnterZoomPassIndex(userId) != -1; }
        public bool IsInWaitStarostaEnterZoomPass(User user) { return getWaitStarostaEnterZoomPassIndex(user) != -1; }

        public bool IsInCreateAltPair(long userId) { return getInCreateAltPairIndex(userId) != -1; }
        public bool IsInWaitEnterDateAltPair(long userId) { return getWaitEnterDateAltPairIndex(userId) != -1; }
        public bool IsInWaitEnterNumberAltPair(long userId) { return getWaitEnterNumberAltPairIndex(userId) != -1; }
        public bool IsInWaitEnterInfoAltPair(long userId) { return getWaitEnterInfoAltPairIndex(userId) != -1; }
        #endregion

        #region Add
        public bool AddToRegistrationUser(User user) 
        {
            if (user == null || IsInRegistration(user)) return false;

            _userRegistrationList.Add(user);
            return true;
        }
        public bool AddToWaitUserInput(User user) 
        {
            if (user == null || IsInWaitUserInput(user)) return false;

            _waitUserRegInputList.Add(user);
            return true;
        }
        public bool AddToWaitUserStarostaClick(User user) 
        {
            if (user == null || IsInWaitUserStarostaClick(user)) return false;

            _waitUserStarostRegClickList.Add(user);
            return true;
        }
        public bool AddToWaitStarostaSelectDsc(User user) 
        {
            if (user == null || IsInWaitStarostaSelectDsc(user)) return false;

            _waitStarostaSelectDscList.Add(user);
            return true;
        }
        public bool AddToWaitStarostaSelectLinkClick(User user) 
        {
            if (user == null || IsInWaitStarostaSelectLinkTypeClick(user)) return false;

            _waitStarostaSelectLinkTypeClickList.Add(user);
            return true;
        }
        public bool AddToWaitStarostaLinkYesNoClick(User user) 
        {
            if (user == null || IsInWaitStarostaLinkYesNoClick(user)) return false;

            _waitStarostaLinkYesNoClick.Add(user);
            return true;
        }
        public bool AddToWaitStarostaLinkEnter(User user) 
        {
            if (user == null || IsInWaitStarostaLinkEnter(user)) return false;

            _waitStarostaLinkEnter.Add(user);
            return true;
        }
        public bool AddToWaitStarostaEnterZoomIdLink(User user)
        {
            if (user == null || IsInWaitStarostaEnterZoomIdLink(user)) return false;

            _waitStarostaEnterZoomIdLink.Add(user);
            return true;
        }
        public bool AddToWaitStarostaEnterZoomPass(User user) 
        {
            if (user == null || IsInWaitStarostaEnterZoomPass(user)) return false;

            _waitStarostaEnterZoomPass.Add(user);
            return true;
        }

        public bool AddToWaitStarostaDonePairLink(long userId,  PairLink pairLink) 
        {
            if (pairLink == null || IsInWaitStarostaDonePairLink(userId)) return false;

            _waitStarostaDonePairLinkList.Add(userId, pairLink);
            return true;
        }
        public bool AddToWaitStarostaYesNoZoomIdLink(long userId, PairLink pairLink) 
        {
            if (pairLink == null || IsInWaitStarostaYesNoZoomIdLink(userId)) return false;

            _waitStarostaYesNoZoomIdLink.Add(userId, pairLink);
            return true;
        }
        public bool AddToWaitStarostaYesNoZoomPass(long userId, PairLink pairLink) 
        {
            if (pairLink == null || IsInWaitStarostaYesNoZoomPass(userId)) return false;

            _waitStarostaYesNoZoomPass.Add(userId, pairLink);
            return true;
        }

        public bool AddToInCreateAltPair(long userId) 
        {
            if (IsInCreateAltPair(userId)) return false;
            _InCreateAltPair.Add(userId);
            return true;
        }
        public bool AddToWaitEnterDateAltPair(long userId) 
        {
            if (IsInWaitEnterDateAltPair(userId)) return false;
            _waitEnterDateAltPair.Add(userId);
            return true;
        }

        public bool AddToWaitEnterNumberAltPair(long userId, string date) 
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrWhiteSpace(date)
                || IsInWaitEnterNumberAltPair(userId))
            { 
                return false; 
            }
            _waitEnterNumberAltPair.Add(userId, date);
            return true;
        }
        public bool AddToWaitEnterInfoAltPair(long userId, AltPair altPair) 
        {
            if (altPair == null || IsInWaitEnterInfoAltPair(userId)) return false;
            _waitEnterInfoAltPair.Add(userId, altPair);
            return true;
        }
        #endregion

        #region Un Add
        public bool UnRegistrationUser(User user) 
        {
            int index = getUserRegistrationIndex(user);

            if (index == -1) return false;
            _userRegistrationList.RemoveAt(index);
            return true;
        }
        public bool UnWaitUserInput(User user) 
        {
            int index = getWaitUserInputIndex(user);

            if (index == -1) return false;
            _waitUserRegInputList.RemoveAt(index);
            return true;
        }
        public bool UnWaitUserStarostaClick(User user) 
        {
            int index = getWaitUserStarostaRegClickIndex(user);

            if (index == -1) return false;
            _waitUserStarostRegClickList.RemoveAt(index);
            return true;
        }
        public bool UnWaitStarostaSelectDsc(User user) 
        {
            int index = getWaitStarostaSelectDscIndex(user);

            if (index == -1) return false;
            _waitStarostaSelectDscList.RemoveAt(index);
            return true;
        }
        public bool UnWaitStarostaSelectLinkTypeClick(User user) 
        {
            int index = getWaitStarostaSelectLinkTypeClickIndex(user);

            if (index == -1) return false;
            _waitStarostaSelectLinkTypeClickList.RemoveAt(index);
            return true;
        }
        public bool UnWaitStarostaLinkYesNoClick(User user) 
        {
            int index = getWaitStarostaLinkYesNoClickIndex(user);

            if (index == -1) return false;
            _waitStarostaLinkYesNoClick.RemoveAt(index);
            return true;
        }
        public bool UnWaitStarostaLinkEnter(User user) 
        {
            int index = getWaitStarostaLinkEnterIndex(user);

            if (index == -1) return false;
            _waitStarostaLinkEnter.RemoveAt(index);
            return true;
        }

        public bool UnWaitStarostaDonePairLink(User user) 
        {
            int index = getWaitStarostaDonePairLinkIndex(user);

            if (index == -1) return false;
            _waitStarostaDonePairLinkList.RemoveAt(index);
            return true;
        }
        public bool UnWaitStarostaYesNoZoomIdLink(User user) 
        {
            int index = getWaitStarostaYesNoZoomIdLinkIndex(user);

            if (index == -1) return false;
            _waitStarostaYesNoZoomIdLink.RemoveAt(index);
            return true;
        }
        public bool UnWaitStarostaEnterZoomIdLink(User user) 
        {
            int index = getWaitStarostaEnterZoomIdLinkIndex(user);

            if (index == -1) return false;
            _waitStarostaEnterZoomIdLink.RemoveAt(index);
            return true;
        }
        public bool UnWaitStarostaYesNoZoomPass(User user) 
        {
            int index = getWaitStarostaYesNoZoomPassIndex(user);

            if (index == -1) return false;
            _waitStarostaYesNoZoomPass.RemoveAt(index);
            return true;
        }
        public bool UnWaitStarostaEnterZoomPass(User user) 
        {
            int index = getWaitStarostaEnterZoomPassIndex(user);

            if (index == -1) return false;
            _waitStarostaEnterZoomPass.RemoveAt(index);
            return true;
        }

        public bool UnInCreateAltPair(long userId) 
        {
            int index = getInCreateAltPairIndex(userId);

            if (index == -1) return false;
            _InCreateAltPair.RemoveAt(index);
            return true;
        }
        public bool UnWaitEnteDaterAltPair(long userId) 
        {
            int index = getWaitEnterDateAltPairIndex(userId);

            if (index == -1) return false;
            _waitEnterDateAltPair.RemoveAt(index);
            return true;
        }

        public bool UnWaitEnterNumberAltPair(long userId) 
        {
            int index = getWaitEnterNumberAltPairIndex(userId);

            if (index == -1) return false;
            _waitEnterNumberAltPair.RemoveAt(index);
            return true;
        }
        public bool UnWaitEnterInfoAltPair(long userId)
        {
            int index = getWaitEnterInfoAltPairIndex(userId);

            if (index == -1) return false;
            _waitEnterInfoAltPair.RemoveAt(index);
            return true;
        }
        #endregion

        #region GET USER
        public User getUserInWaitUserStarostaClick(long userId) { return _waitUserStarostRegClickList.Find(x => x.Id == userId); }
        public User getUserInWaitStarostaLinkYesNoClick(long userId) { return _waitStarostaLinkYesNoClick.Find(x => x.Id == userId); }
        public User getUserInWaitStarostaSelectDsc(long userId) { return _waitStarostaSelectDscList.Find(x => x.Id == userId); }
        public User getUserInWaitStarostaSelectLinkTypeClick(long userId) { return _waitStarostaSelectLinkTypeClickList.Find(x => x.Id == userId); }
        #endregion

        #region GET PAIRLINK
        public PairLink getPairLinkInWaitStarostaDonePairLink(long userId) 
        {
            int index = getWaitStarostaDonePairLinkIndex(userId);

            if (index == -1) return null;
            return _waitStarostaDonePairLinkList.ElementAt(index).Value;
        }
        public PairLink getPairLinkInWaitStarostaYesNoZoomIdLink(long userId) 
        {
            int index = getWaitStarostaYesNoZoomIdLinkIndex(userId);

            if (index == -1) return null;
            return _waitStarostaYesNoZoomIdLink.ElementAt(index).Value;
        }
        public PairLink getPairLinkInWaitStarostaYesNoZoomPass(long userId) 
        {
            int index = getWaitStarostaYesNoZoomPassIndex(userId);

            if (index == -1) return null;
            return _waitStarostaYesNoZoomPass.ElementAt(index).Value;
        }
        #endregion

        /**
         * <returns>
         * Return string DATE_REG of AltPair 
         * if good; otherwise null
         * </returns>
         */
        public string getStrWaitEnterNumberAltPair(long userId) 
        {
            int index = getWaitEnterNumberAltPairIndex(userId);

            if (index == -1) return null;
            return _waitEnterNumberAltPair.ElementAt(index).Value;
        }
        public AltPair getAltPairWaitEnterInfoAltPair(long userId) 
        {
            int index = getWaitEnterInfoAltPairIndex(userId);

            if (index == -1) return null;
            return _waitEnterInfoAltPair.ElementAt(index).Value;

        }
        public bool UpdateRegistrationUser(User user) 
        {
            if (!IsInRegistration(user)) return false;
            
            _userRegistrationList[getUserRegistrationIndex(user)] = user;
            return true;
        }

        public bool UpdateWaitStarostaDonePairLink(long userId, PairLink pairLink) 
        {
            int index = getWaitStarostaDonePairLinkIndex(userId);

            if (index == -1) return false;

            _waitStarostaDonePairLinkList[index] = pairLink;
            return true;
        }

    }
}
