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
        public String avatarURL;
        public CommentInfo()
        {
        }
        public CommentInfo(int _threadID, int _commentID, String _comment, String _owner, String _avatarURL)
        {
            threadID = _threadID;
            commentID = _commentID;
            comment = _comment;
            owner = _owner;
            avatarURL = _avatarURL;
        }
        public CommentInfo(Dictionary<string, object> m)
        {
            threadID = Convert.ToInt32(m["ThreadID"]);
            commentID = Convert.ToInt32(m["CommentID"]);
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
            m["AvatarURL"] = avatarURL;
            return m;
        }
    }
}