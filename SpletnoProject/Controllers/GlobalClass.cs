using SpletnoProject.BlockchainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SpletnoProject.Controllers
{
    public class GlobalClass
    {
        //static BlockchainClasses.Blockchain _block1 = new BlockchainClasses.Blockchain();
        static string _block;
        // static string[] _blockchainArray = new string[0];/*= _block1.toStringArray()*/
        static int _diff = 2;
        static Blockchain _blockchain;

        static Dictionary<string, int> _list = new Dictionary<string, int>();
      //  static List<Tuple<string, int>> _list = new List<Tuple<string, int>>();
        static List<string> _log = new List<string>();


        static Dictionary<string, Wallet> _userWaffles  = new   Dictionary<string,Wallet>();
       
        public static String Block
        {
            get
            {
                return _block;
            }
            set
            {
                _block = value;
            }
        }

        public static Blockchain BlockChain
        {
            get
            {
                return _blockchain;

            }
            set
            {
                _blockchain = value;

            }
        }

        public static int Diff
        {
            get
            {
                return _diff;
            }
            set
            {
                _diff = value;
            }
        }

        public static Dictionary<string, int> ListIP
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }
        public static List<string> Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        public static Dictionary<string, Wallet> UserWallets
        {
            get
            {
                return _userWaffles;
            }
            set
            {
                _userWaffles = value;
            }
        }

    }
}