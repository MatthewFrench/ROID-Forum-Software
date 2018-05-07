using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class ThreadInfo
    {
        public int id;
        public String title;
        public String description;
        public String owner;
        public List<CommentInfo> comments;
        public int commentIDs = 0;
        public ThreadInfo(String _owner, int _id, String t, String d)
        {
            owner = _owner;
            id = _id;
            title = t;
            description = d;
            comments = new List<CommentInfo>();
        }
        public ThreadInfo(Dictionary<string, object> m)
        {
            owner = (string)m["Owner"];
            id = Convert.ToInt32(m["ID"]);
            title = (string)m["Title"];
            description = (string)m["Description"];
            List<Dictionary<string, object>> commentArray = (List<Dictionary<string, object>>)m["Comments"];
            comments = new List<CommentInfo>();
            if (m["CommentIDs"] != null)
            {
                commentIDs = Convert.ToInt32(m["CommentIDs"]);
            }
            //Repair comments when loading them just in case their IDs are wrong.
            for (int i = 0; i < commentArray.Count; i++)
            {
                CommentInfo c = new CommentInfo(commentArray[i]);
                if (getCommentForID(c.commentID) != null)
                {
                    c.commentID = commentIDs++;
                }
                comments.Add(c);
            }
        }
        public Dictionary<string, object> toMap()
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Owner"] = owner;
            m["ID"] = id;
            m["Title"] = title;
            m["Description"] = description;
            m["CommentIDs"] = commentIDs;
            List<Dictionary<string, object>> commentArray = new List<Dictionary<string, object>>();
            for (int i = 0; i < comments.Count; i++)
            {
                commentArray.Add(comments[i].toMap());
            }
            m["Comments"] = commentArray;
            return m;
        }
        public CommentInfo getCommentForID(int commentID)
        {
            for (int i = 0; i < comments.Count; i++)
            {
                if (comments[i].commentID == commentID)
                {
                    return comments[i];
                }
            }
            return null;
        }
    }
}
