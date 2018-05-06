using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class Account
    {
        public String name = "";
        public String password = "";
        public String email = "";
        public String avatarURL = "";
        public Account(Dictionary<string, object> m)
        {
            name = (string)m.GetValueOrDefault("Name");
            password = (string)m.GetValueOrDefault("Password");
            email = (string)m.GetValueOrDefault("Email");
            if (m.ContainsKey("AvatarURL"))
            {
                avatarURL = (string)m.GetValueOrDefault("AvatarURL");
            }
        }
    }
}