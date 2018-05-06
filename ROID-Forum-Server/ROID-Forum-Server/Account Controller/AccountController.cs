using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class AccountController
    {
        List<Account> accounts = new List<Account>();
        ServerController server;
        AccountIOController accountIOController;
        public AccountController(ServerController s)
        {
            server = s;
            accountIOController = new AccountIOController(this);
            accountIOController.loadAllAccounts();
        }
        public void logic()
        {
            accountIOController.logic();
        }
        public string getAvatarForAccount(String name)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                Account a = accounts[i];
                if (a.name == name)
                {
                    return a.avatarURL;
                }
            }
            return "";
        }
        public bool accountExists(String name, String password)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                Account a = accounts[i];
                if (a.name == name && a.password == password)
                {
                    return true;
                }
            }
            return false;
        }
        public bool accountNameExists(String name)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                Account a = accounts[i];
                if (a.name == name)
                {
                    return true;
                }
            }
            return false;
        }
        public Account getAccount(String name, String password)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                Account a = accounts[i];
                if (a.name == name && a.password == password)
                {
                    return a;
                }
            }
            return null;
        }
        public Account createAccount(String name, String password, String email)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("Name", name);
            dictionary.Add("Password", password);
            dictionary.Add("Email", email);
            Console.WriteLine($"Created account: {name}, {password}, {email}");
            Account a = new Account(dictionary);
            accounts.Add(a);
            return a;
        }
    }
}
