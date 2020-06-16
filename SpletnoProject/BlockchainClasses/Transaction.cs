using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpletnoProject.BlockchainClasses
{
    public class Transaction
    {

        public Transaction(int amount, bool isSending, string otherUser, string user)
        {
            this.amount = amount;
            this.isSending = isSending;
            this.otherUser = otherUser;
            this.user = user;
        }

        override
        public string ToString()
        {
            string data = "";

            data = otherUser;
            if (isSending)
                data += " want to send " + amount + " Gold to " + user;
            else
                data += " is requesting " + amount + " Gold from " + user;

         

            return data;
        }

        private int amount;

        private string user;

        private bool isSending;

        private string otherUser;

        public bool IsSending 
        {
            get => isSending; 
            set => isSending = value;
        }

        public int Amount
        {
            get => amount; set => amount = value;
        }

        public string OtherUser
        {
            get => otherUser; set => otherUser = value;
        }

        public string User
        {
            get => user; set => user = value;
        }

    }
}