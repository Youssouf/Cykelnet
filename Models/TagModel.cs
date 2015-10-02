using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Cykelnet.Models
{
    public class TagModel
    {

        static CykelnetDBDataContext _db = new CykelnetDBDataContext();
        private static String _connectionString = _db.Connection.ConnectionString;

        private String tag;
        private int tagId;

        public int TagId
        {
            get { return tagId; }
            set { tagId = value; }
        }

        public String Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public TagModel()
        {

        }

        public TagModel(String t)
        {
            this.tag = t;
        }

        public TagModel(Tag t)
        {
            this.tag = t.TagName;
            this.tagId = t.Tag_ID;
        }

        public static void deleteAllTags(int RouteID)
        {
            var tags = from t in _db.Routetags
                       where t.Route_ID == RouteID
                       select t;
            _db.Routetags.DeleteAllOnSubmit(tags);
        }

        public static List<TagModel> createTags(List<String> st)
        {
            List<TagModel> tags = new List<TagModel>();
            foreach (String s in st)
                tags.Add(new TagModel(s));
            return tags;
        }


        public static List<Tag> insertTags(List<TagModel> tags)
        {
            if (tags != null)
            {

                List<Tag> tList = new List<Tag>();
                List<Tag> insertedTagList = new List<Tag>();

                foreach (TagModel tm in tags)
                {
                    Tag ext = (from t in _db.Tags
                               where t.TagName == tm.Tag
                               select t).SingleOrDefault();

                    if (ext == null)
                    {
                        ext = new Tag();
                        ext.TagName = tm.Tag;

                        tList.Add(ext);
                    }

                    insertedTagList.Add(ext);
                }
                _db.Tags.InsertAllOnSubmit(tList);
                _db.SubmitChanges();

                return insertedTagList;
            }
            return null;
        }

        /// <summary>
        /// Deletes a tag from DB
        /// </summary>
        /// <param name="tagId">ID of the tag to be deleted</param>
        public static void deleteTag(int tagID)
        {
            // Finally delete the route itself
            Tag tag = (from t in _db.Tags
                       where t.Tag_ID == tagID
                       select t).SingleOrDefault();
            _db.Tags.DeleteOnSubmit(tag);
            _db.SubmitChanges();
        }

        public static List<TagModel> getAllTags()
        {
            List<Tag> tList = (from t in _db.Tags
                                 select t).ToList();

            List<TagModel> tmList = new List<TagModel>();
            foreach (Tag t in tList)
            {
                tmList.Add(new TagModel(t));
            }
            return tmList;
        }

        public static List<TagModel> getRouteTags(RouteModel rm)
        {
            List<Tag> tList = (from t in _db.Tags
                               join rt in _db.Routetags on t.Tag_ID equals rt.Tag_ID
                               where rt.Route_ID == rm.routeID
                               select t).ToList();

            List<TagModel> tmList = new List<TagModel>();
            foreach (Tag t in tList)
            {
                tmList.Add(new TagModel(t));
            }
            return tmList;
        }
    }
}