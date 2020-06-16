using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using SpletnoProject.BlockchainClasses;

namespace SpletnoProject.Controllers
{
    public class DefaultController : ApiController
    {
        public string Get()
        {
            return "Welcome To this weird af Api";
        }

        public string Get(string command)
        {
           
            GlobalClass.Log.Add(HttpContext.Current.Request.UserHostAddress + ": " + command);
            
            if (command == "sync")
               
            {
                return JsonConvert.SerializeObject(GlobalClass.BlockChain);
            }
            else if (command == "check")
            {
                try
                {
                    GlobalClass.BlockChain.getLastBlock();


                    return "yes";
                }
                catch
                {
                    return "no";
                }

              
            }
            else if (command == "init")
            {
                // GlobalClass.BlockChain.updateChain( new Blockchain());

                Blockchain temp = new Blockchain();

                GlobalClass.BlockChain = temp;

                return JsonConvert.SerializeObject(GlobalClass.BlockChain);
            }
            else if (command == "quit")
            {
                if (GlobalClass.ListIP.ContainsKey(HttpContext.Current.Request.UserHostAddress))
                {

                    GlobalClass.ListIP[HttpContext.Current.Request.UserHostAddress]--;
                    if (GlobalClass.ListIP[HttpContext.Current.Request.UserHostAddress] == 0)
                    {
                        GlobalClass.ListIP.Remove(HttpContext.Current.Request.UserHostAddress);
                    }
                }

                return HttpContext.Current.Request.UserHostAddress;
            }
            else if (command == "get_client_num")
            {
                int num = 0;

                foreach (KeyValuePair<string, int> entry in GlobalClass.ListIP)
                {
                    //
                    // do something with entry.Value or entry.Key

                    num += entry.Value;
                }

                return num.ToString();

            }
            else if (command == "get_log")
            {
                return JsonConvert.SerializeObject(GlobalClass.Log);
            }
            else if (command == "clr_log")
            {
                GlobalClass.Log = new List<string>();
                return "ok";
            }
            
            return "Unknown Command: " + command;



        }

        public string Get(string command, string username)
        {
           

            GlobalClass.Log.Add(HttpContext.Current.Request.UserHostAddress + ": " + command +": " + username);

             if (command == "check_gold")
            {
                return "Current amount of Gold on your Wallet: " + GlobalClass.UserWallets[username].Gold;
            }
            else if (command == "check_trans")
            {
                string data="";

                /*
                foreach(Transaction transaction in GlobalClass.UserWallets[username].transactionLog)
                {
                    data += transaction.ToString() + "\n";
                }
                */

                for ( int i = 0; i<GlobalClass.UserWallets[username].transactionLog.Count; i++)
                {
                    data += "id " + i + ": " + GlobalClass.UserWallets[username].transactionLog[i].ToString() + "\n";
                }
                return data;
            }
            else if (command == "check_mytrans")
            {
                string data = "";

                /*
                foreach(Transaction transaction in GlobalClass.UserWallets[username].transactionLog)
                {
                    data += transaction.ToString() + "\n";
                }
                */

                for (int i = 0; i < GlobalClass.UserWallets[username].myTransactionLog.Count; i++)
                {
                    data += GlobalClass.UserWallets[username].myTransactionLog[i].ToString() + "\n";
                }
                return data;
            }

            return "No";
        }

        public string Get(string command, string username, string key)
        {
          
            GlobalClass.Log.Add(HttpContext.Current.Request.UserHostAddress + ": " + command + ": " + username );

           

            if (command == "connect")
            {
                if (GlobalClass.ListIP.ContainsKey(HttpContext.Current.Request.UserHostAddress))
                {
                    GlobalClass.ListIP[HttpContext.Current.Request.UserHostAddress]++;
                }
                else
                    GlobalClass.ListIP.Add(HttpContext.Current.Request.UserHostAddress, 1);

                string hashedUser;
                string publicKeySend;


                var rsa = new RSACryptoServiceProvider(2048);
                var rsa2 = new RSACryptoServiceProvider(2048);

                rsa.PersistKeyInCsp = false;

                RSAParameters privateKey = rsa.ExportParameters(true);

                publicKeySend = rsa.ToXmlString(false);

                rsa2.FromXmlString(key);

                RSAParameters signatureKey = rsa2.ExportParameters(false);

                using (var sha256 = new SHA256Managed())
                {
                    hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(username))).Replace("-", "");
                }

                if (!GlobalClass.UserWallets.ContainsKey(hashedUser))
                {
                    Wallet temp = new Wallet(username, 50, privateKey, signatureKey);

                    GlobalClass.UserWallets.Add(hashedUser, temp);
                }
                else
                {
                    GlobalClass.UserWallets[hashedUser].Key = privateKey;
                    GlobalClass.UserWallets[hashedUser].SignatureKey = signatureKey;
                }



                string sending = /*Uri.EscapeDataString(*/publicKeySend;
                string[] content = new string[2];
                content[0] = hashedUser;
                content[1] = sending;
                return JsonConvert.SerializeObject(content);
            }
           
           return "Noo";
        }

        public string Get(string command, string username, string data, string signature)
        {

            
            bool active = false;
            RSAParameters privateKey;
            //return "AAAAAAAAA";

            string number;
            string otheruser;

            if(GlobalClass.UserWallets.ContainsKey(username))
            {
                active = true;
                privateKey = GlobalClass.UserWallets[username].Key;


                

                

                String[] arr = data.Replace("\"", "").Split('-');
                byte[] array = new byte[arr.Length];
                for (int i = 0; i < arr.Length; i++) array[i] = Convert.ToByte(arr[i], 16);


                var str = Decrypt(array, GlobalClass.UserWallets[username].Key);

                data = Encoding.UTF8.GetString(str);


               

              


            }



            if (active)
            {
                string[] content = JsonConvert.DeserializeObject<string[]>(data);

                otheruser = content[0];
                number = content[1];
               // return ValidateSignature(signature, otheruser + number, GlobalClass.UserWallets[username].SignatureKey).ToString();
                if (ValidateSignature(signature, otheruser+number, GlobalClass.UserWallets[username].SignatureKey))
                {
                    if (command == "send_gold")
                    {
                        string hashedUser;
                        using (var sha256 = new SHA256Managed())
                        {
                            hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(otheruser))).Replace("-", "");
                        }

                        if (GlobalClass.UserWallets.ContainsKey(hashedUser))
                        {
                            Transaction transaction = new Transaction(Math.Abs(int.Parse(number)), true, GlobalClass.UserWallets[username].Owner, otheruser);
                            GlobalClass.UserWallets[hashedUser].transactionLog.Add(transaction);
                            GlobalClass.UserWallets[username].myTransactionLog.Add(transaction);
                            return "Transaction Requested";
                        }
                        else
                        {
                            return "User Doesn't Exist";
                        }
                    }
                    else if (command == "req_gold")
                    {
                        string hashedUser;
                        using (var sha256 = new SHA256Managed())
                        {
                            hashedUser = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(otheruser))).Replace("-", "");
                        }

                        if (GlobalClass.UserWallets.ContainsKey(hashedUser))
                        {
                            Transaction transaction = new Transaction(Math.Abs(int.Parse(number)), false, GlobalClass.UserWallets[username].Owner, otheruser);
                            GlobalClass.UserWallets[hashedUser].transactionLog.Add(transaction);
                            GlobalClass.UserWallets[username].myTransactionLog.Add(transaction);
                            return "Transaction Requested";
                        }
                        else
                        {
                            return "User Doesn't Exist";
                        }
                    }
                }
                else
                    return "Invalid Signature";
            }
          
            return "No";
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

        private byte[] Encrypt(byte[] input, RSAParameters key)
        {
            byte[] encrypted;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(key);
                encrypted = rsa.Encrypt(input, true);
            }

            return encrypted;
        }

        private bool ValidateSignature(string signature, string data, RSAParameters key)
        {

            byte[] signedBytes = Convert.FromBase64String(signature);
            byte[] originalMessage = Encoding.UTF8.GetBytes(data);
            bool success = true;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(key);
                success = rsa.VerifyData(originalMessage, CryptoConfig.MapNameToOID("SHA512"), signedBytes);
               
            }


            return success;
        }

    }
}
