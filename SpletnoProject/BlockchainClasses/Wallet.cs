using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace SpletnoProject.BlockchainClasses
{
    public class Wallet
    {
        public Wallet(string owner , int gold, RSAParameters key, RSAParameters signatureKey)
        {
            _owner = owner;
            _gold = gold;
            _key = key;
            _signatureKey = signatureKey;
            transactionLog = new List<Transaction>();
            myTransactionLog = new List<Transaction>();

        }

        private int _gold;
        private RSAParameters _key;
        private string _owner;
        private RSAParameters _signatureKey;


        public string Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public List<Transaction> transactionLog;
        public List<Transaction> myTransactionLog;
        public int Gold { get {return _gold; } set { _gold = value; } }

        public RSAParameters Key { get { return _key; } set { _key = value; } }

        public RSAParameters SignatureKey { get { return _signatureKey; } set { _signatureKey = value; } }

    }
}