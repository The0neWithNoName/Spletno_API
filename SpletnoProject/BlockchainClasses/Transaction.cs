using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpletnoProject.BlockchainClasses
{
    public class Transaction
    {

        public Transaction(int amount, bool isSending, string otherUser)
        {
            this.amount = amount;
            this.isSending = isSending;
            this.otherUser = otherUser;
        }

        override
        public string ToString()
        {
            string data = "";

            data = otherUser;
            if (isSending)
                data += " want to send ";
            else
                data += " is requesting ";

            data += amount + " Gold";

            return data;
        }

        private int amount;

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


    }
}