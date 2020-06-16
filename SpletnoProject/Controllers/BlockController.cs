using Newtonsoft.Json;
using SpletnoProject.BlockchainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace SpletnoProject.Controllers
{
    public class BlockController : ApiController
    {

        public string Get(string command)
        {

            GlobalClass.Log.Add(HttpContext.Current.Request.UserHostAddress + ": " + command  );

            Random rng = new Random();
            if (command == "new_block")
            {
                // return GlobalClass.BlockChain.Length.ToString();
                if (GlobalClass.BlockChain.chain.Count > 0)
                {
                    Block block1 = new Block(GlobalClass.BlockChain.chain.Count, "WHATS UP " + rng.Next(1000), GlobalClass.BlockChain.getLastBlock().hash);
                    GlobalClass.BlockChain.addBlock(block1, GlobalClass.Diff);
                    return block1.blockToJson();


                }
                Blockchain blockchain1 = new Blockchain();

                Block block = new Block(1, "WHATS UP " + rng.Next(1000), blockchain1.getLastBlock().hash);
                blockchain1.addBlock(block, GlobalClass.Diff);
                GlobalClass.BlockChain.updateChain(blockchain1);
                return blockchain1.getLastBlock().blockToJson();
            }
            else if (command == "last_block")
            {
               
                if (GlobalClass.BlockChain.chain.Count > 0)
                {
                    return GlobalClass.BlockChain.getLastBlock().blockToJson();
                }
                else
                    return "No";
            }
            else if (command == "whole_blockchain")
            {
                return JsonConvert.SerializeObject(GlobalClass.BlockChain);
            }
            else
                return "Command Entered: " + command;

            return "how";
        }

        public string Get(string command, int num)
        {
            GlobalClass.Log.Add(HttpContext.Current.Request.UserHostAddress + ": " + command + ":" + num);

            if (command == "change_diff")
            {
                GlobalClass.Diff = num;
                return "New Difficulty Set: "+ num;
            }


            return "No";
        }


        public string Get(string command, string username)
        {
            if (GlobalClass.UserWallets.ContainsKey(username))
            {

                if (command == "acc_all")
                {
                    if (!GlobalClass.UserWallets.ContainsKey(username))
                        return "You do not own a Wallet, please connect first";

                    if (GlobalClass.UserWallets[username].transactionLog.Count > 0)
                    {
                        List<Transaction> unanswered = new List<Transaction>();

                        foreach (Transaction transaction in GlobalClass.UserWallets[username].transactionLog)
                        {
                            if (transaction.IsSending)
                            {


                                string hashedUser = "";
                                using (var sha256 = new SHA256Managed())
                                {
                                    hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(transaction.OtherUser))).Replace("-", "");
                                }
                                if ((GlobalClass.UserWallets[hashedUser].Gold - transaction.Amount) >= 0)
                                {

                                    GlobalClass.UserWallets[hashedUser].Gold -= transaction.Amount;
                                    GlobalClass.UserWallets[username].Gold += transaction.Amount;
                                    string data = GlobalClass.UserWallets[hashedUser].Owner + " is Sending " + GlobalClass.UserWallets[username].Owner + " " + transaction.Amount + " Gold";

                                    Block block = new Block(GlobalClass.BlockChain.chain.Count, data, "");

                                    GlobalClass.BlockChain.addBlock(block, GlobalClass.Diff);
                                    GlobalClass.UserWallets[hashedUser].myTransactionLog.Remove(transaction);
                                }
                                else
                                {
                                    unanswered.Add(transaction);

                                    string data = GlobalClass.UserWallets[hashedUser].Owner + " couldn't pay " + GlobalClass.UserWallets[username].Owner + " " + transaction.Amount + " Gold";

                                    Block block = new Block(GlobalClass.BlockChain.chain.Count, data, "");

                                    GlobalClass.BlockChain.addBlock(block, GlobalClass.Diff);
                                }
                            }
                            else if (!transaction.IsSending)
                            {
                                if ((GlobalClass.UserWallets[username].Gold - transaction.Amount) >= 0)
                                {
                                    GlobalClass.UserWallets[username].Gold -= transaction.Amount;

                                    string hashedUser = "";
                                    using (var sha256 = new SHA256Managed())
                                    {
                                        hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(transaction.OtherUser))).Replace("-", "");
                                    }

                                    GlobalClass.UserWallets[hashedUser].Gold += transaction.Amount;

                                    string data = GlobalClass.UserWallets[username].Owner + " is Sending " + GlobalClass.UserWallets[hashedUser].Owner + " " + transaction.Amount + " Gold";

                                    Block block = new Block(GlobalClass.BlockChain.chain.Count, data, "");
                                    GlobalClass.UserWallets[hashedUser].myTransactionLog.Remove(transaction);
                                    GlobalClass.BlockChain.addBlock(block, GlobalClass.Diff);
                                }
                                else
                                {
                                    unanswered.Add(transaction);
                                    string data = GlobalClass.UserWallets[username].Owner + " couldn't pay " + transaction.OtherUser + " " + transaction.Amount + " Gold";

                                    Block block = new Block(GlobalClass.BlockChain.chain.Count, data, "");

                                    GlobalClass.BlockChain.addBlock(block, GlobalClass.Diff);
                                }
                            }


                        }
                        GlobalClass.UserWallets[username].transactionLog = unanswered;
                        return JsonConvert.SerializeObject(GlobalClass.BlockChain);
                    }
                    else
                    {
                        return "No Transactions";
                    }
                }
                else if (command == "dec_all")
                {
                    if (!GlobalClass.UserWallets.ContainsKey(username))
                        return "You do not own a Wallet, please connect first";

                    if (GlobalClass.UserWallets[username].transactionLog.Count > 0)
                    {
                        foreach (Transaction transaction in GlobalClass.UserWallets[username].transactionLog)
                        {

                            string data = GlobalClass.UserWallets[username].Owner + " declined " + transaction.OtherUser + "'s transaction for " + transaction.Amount + " Gold";

                            Block block = new Block(GlobalClass.BlockChain.chain.Count, data, "");

                            GlobalClass.BlockChain.addBlock(block, GlobalClass.Diff);


                            string hashedUser = "";
                            using (var sha256 = new SHA256Managed())
                            {
                                hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(transaction.OtherUser))).Replace("-", "");
                            }

                            GlobalClass.UserWallets[hashedUser].myTransactionLog.Remove(transaction);

                        }
                        GlobalClass.UserWallets[username].transactionLog = new List<Transaction>();
                        return JsonConvert.SerializeObject(GlobalClass.BlockChain);

                    }
                    else
                    {
                        return "No Transactions";
                    }
                }

            }
                return "no";
        }

       
        public string Get(string command, string username, string number)
        {

            bool active = false;
            bool active2 = false;
            RSAParameters privateKey;
            /*
            if (GlobalClass.UserWallets.ContainsKey(username))
            {
                active = true;
                privateKey = GlobalClass.UserWallets[username].Key;
                number = Encoding.UTF8.GetString(Decrypt(Encoding.UTF8.GetBytes(number), privateKey));

            }
            */

            if (GlobalClass.UserWallets.ContainsKey(username))
            {
                active = true;
                privateKey = GlobalClass.UserWallets[username].Key;



                String[] arr = number.Replace("\"", "").Split('-');
                byte[] array = new byte[arr.Length];
                for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);


                var str = Decrypt(array, GlobalClass.UserWallets[username].Key);

                number = Encoding.UTF8.GetString(str);

               

            }

            if (GlobalClass.UserWallets[username].transactionLog.Count > Math.Abs(int.Parse(number)))
                active2 = true;

            if (active && active2)
            {
                if (command == "acc")
                {
                    if (GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].IsSending)
                    {
                       

                        string hashedUser = "";
                        using (var sha256 = new SHA256Managed())
                        {
                            hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].OtherUser))).Replace("-", "");
                        }
                        if ((GlobalClass.UserWallets[hashedUser].Gold -= GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount) >= 0)
                        {
                            GlobalClass.UserWallets[hashedUser].Gold -= GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount;
                            GlobalClass.UserWallets[username].Gold += GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount;
                            string data = GlobalClass.UserWallets[hashedUser].Owner + " is Sending " + GlobalClass.UserWallets[username].Owner + " " + GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount + " Gold";

                            Block block = new Block(GlobalClass.BlockChain.chain.Count, data, "");

                            GlobalClass.BlockChain.addBlock(block, GlobalClass.Diff);
                            GlobalClass.UserWallets[hashedUser].myTransactionLog.Remove(GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))]);
                            GlobalClass.UserWallets[username].transactionLog.RemoveAt(Math.Abs(int.Parse(number)));
                            return GlobalClass.BlockChain.getLastBlock().blockToJson();
                        }
                        else
                            return "They";
                    }else
                    {
                        if ((GlobalClass.UserWallets[username].Gold - GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount) >= 0)
                        {
                            GlobalClass.UserWallets[username].Gold -= GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount;

                            string hashedUser = "";
                            using (var sha256 = new SHA256Managed())
                            {
                                hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].OtherUser))).Replace("-", "");
                            }

                            GlobalClass.UserWallets[hashedUser].Gold += GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount;

                            string data = GlobalClass.UserWallets[username].Owner + " is Sending " + GlobalClass.UserWallets[hashedUser].Owner + " " + GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount + " Gold";

                            Block block = new Block(GlobalClass.BlockChain.chain.Count, data, "");

                            GlobalClass.BlockChain.addBlock(block, GlobalClass.Diff);
                            GlobalClass.UserWallets[hashedUser].myTransactionLog.Remove(GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))]);
                            GlobalClass.UserWallets[username].transactionLog.RemoveAt(Math.Abs(int.Parse(number)));
                            return GlobalClass.BlockChain.getLastBlock().blockToJson();
                        }
                        else
                            return "You";
                    }

                }
                else if (command == "dec")
                {
                   
                    string data = GlobalClass.UserWallets[username].Owner + " declined " + GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].OtherUser + "'s " + GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].Amount + " Gold";

                    Block block = new Block(GlobalClass.BlockChain.chain.Count, data, "");

                    GlobalClass.BlockChain.addBlock(block, GlobalClass.Diff);
                    string hashedUser = "";
                    using (var sha256 = new SHA256Managed())
                    {
                        hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))].OtherUser))).Replace("-", "");
                    }
                    GlobalClass.UserWallets[hashedUser].myTransactionLog.Remove(GlobalClass.UserWallets[username].transactionLog[Math.Abs(int.Parse(number))]);
                    GlobalClass.UserWallets[username].transactionLog.RemoveAt(Math.Abs(int.Parse(number)));
                    return GlobalClass.BlockChain.getLastBlock().blockToJson();
                }
            }
            return "no";
        }

        private byte[] Decrypt(byte[] input, RSAParameters key)
        {
            byte[] decrypted;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(key);
                decrypted = rsa.Decrypt(input, true);
            }

            return decrypted;
        }
    }
}
