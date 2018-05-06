using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class CommentInfo
    {
        public int threadID;
        public int commentID;
        public String comment;
        public String owner;
        public CommentInfo()
        {
        }
        public CommentInfo(int _threadID, int _commentID, String _comment, String _owner)
        {
            threadID = _threadID;
            commentID = _commentID;
            comment = _comment;
            owner = _owner;
        }
        public CommentInfo(Dictionary<string, object> m)
        {
            threadID = (int)m["ThreadID"];
            commentID = (int)m["CommentID"];
            comment = (string)m["Comment"];
            owner = (string)m["Owner"];
        }
        public Dictionary<string, object> toMap()
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["ThreadID"] = threadID;
            m["CommentID"] = commentID;
            m["Comment"] = comment;
            m["Owner"] = owner;
            return m;
        }
    }
}