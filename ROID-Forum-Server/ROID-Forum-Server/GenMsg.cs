using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace ROIDForumServer
{
    public class GenMsg
    {
        public static String Login(bool success)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Title"] = "Login";
            m["Status"] = success;
            return JsonConvert.SerializeObject(m);
        }
        public static String Register(bool success)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Title"] = "Register";
            m["Status"] = success;
            return JsonConvert.SerializeObject(m);
        }
        public static String Name(String status)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Title"] = "Name";
            m["Status"] = status;
            return JsonConvert.SerializeObject(m);
        }
        public static String Enter(String game)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Title"] = "Enter";
            m["Game"] = game;
            return JsonConvert.SerializeObject(m);
        }
    }
}
