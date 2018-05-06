using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace ROIDForumServer
{
    public class LoginController
    {
        ServerController server;
        public LoginController(ServerController s)
        {
            server = s;
        }
        public void logic()
        {
        }
        public void onMessage(User u, Dictionary<string, object> message)
        {
            if ((string)message["Title"] == "Set Avatar" && u.account != null)
            {
                u.account.avatarURL = (string)message["AvatarURL"];
            }
            if ((string)message["Title"] == "Get Avatar" && u.account != null)
            {
                Dictionary<string, object> m = new Dictionary<string, object>();
                m["Controller"] = "Login";
                m["Title"] = "Get Avatar";
                m["AvatarURL"] = u.account.avatarURL;
                String s = JsonConvert.SerializeObject(m);
                u.sendString(s);
            }
            if ((string)message["Title"] == "Login" && u.account == null)
            {
                if (server.accountController.accountExists((string)message["Name"], (string)message["Password"]))
                {
                    u.account = server.accountController.getAccount((string)message["Name"], (string)message["Password"]);
                    //Send login notification
                    Dictionary<string, object> m = new Dictionary<string, object>();
                    m["Controller"] = "Login";
                    m["Title"] = "Logged In";
                    m["Name"] = u.account.name;
                    m["Password"] = u.account.password;
                    String s = JsonConvert.SerializeObject(m);
                    u.sendString(s);

                    server.accountLoggedIn(u);
                }
                else
                {
                    //Send login failure
                    Dictionary<string, object> m = new Dictionary<string, object>();
                    m["Controller"] = "Login";
                    m["Title"] = "Login Failed";
                    String s = JsonConvert.SerializeObject(m);
                    u.sendString(s);
                }
            }
            if ((string)message["Title"] == "Logout" && u.account != null)
            {
                //Perhaps an account method can be called for saving or other logic
                u.account = null;
                //Send logout notification
                Dictionary<string, object> m = new Dictionary<string, object>();
                m["Controller"] = "Login";
                m["Title"] = "Logged Out";
                String s = JsonConvert.SerializeObject(m);
                u.sendString(s);
                server.accountLoggedOut(u);
            }
            if ((string)message["Title"] == "Register" && u.account == null)
            {
                if (server.accountController.accountNameExists((string)message["Name"]))
                {
                    //Send register failure
                    Dictionary<string, object> m = new Dictionary<string, object>();
                    m["Controller"] = "Login";
                    m["Title"] = "Register Failed";
                    String s = JsonConvert.SerializeObject(m);
                    u.sendString(s);
                }
                else
                {
                    u.account = server.accountController.createAccount((string)message["Name"], (string)message["Password"], (string)message["Email"]);
                    //Send login notification
                    Dictionary<string, object> m = new Dictionary<string, object>();
                    m["Controller"] = "Login";
                    m["Title"] = "Logged In";
                    m["Name"] = u.account.name;
                    m["Password"] = u.account.password;
                    String s = JsonConvert.SerializeObject(m);
                    u.sendString(s);
                    server.accountLoggedIn(u);
                }
            }
        }
    }
}
