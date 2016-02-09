using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterTracker.Core
{

    public class User
    {
        public long? id { get; set; }
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public bool? @protected { get; set; }
        public bool? verified { get; set; }
        public int? followers_count { get; set; }
        public int? friends_count { get; set; }
        public int? listed_count { get; set; }
        public int? favourites_count { get; set; }
        public int? statuses_count { get; set; }
        public string created_at { get; set; }
        public int? utc_offset { get; set; }
        public string time_zone { get; set; }
        public bool? geo_enabled { get; set; }
        public string lang { get; set; }
        public bool? contributors_enabled { get; set; }
        public bool? is_translator { get; set; }
        public string profile_background_color { get; set; }
        public string profile_background_image_url { get; set; }
        public string profile_background_image_url_https { get; set; }
        public bool? profile_background_tile { get; set; }
        public string profile_link_color { get; set; }
        public string profile_sidebar_border_color { get; set; }
        public string profile_sidebar_fill_color { get; set; }
        public string profile_text_color { get; set; }
        public bool? profile_use_background_image { get; set; }
        public string profile_image_url { get; set; }
        public string profile_image_url_https { get; set; }
        public bool? default_profile { get; set; }
        public bool? default_profile_image { get; set; }
        public bool? following { get; set; }
        public bool? follow_request_sent { get; set; }
        public bool? notifications { get; set; }
    }

    public class Medium2
    {
        public int? w { get; set; }
        public int? h { get; set; }
        public string resize { get; set; }
    }

    public class Thumb
    {
        public int? w { get; set; }
        public int? h { get; set; }
        public string resize { get; set; }
    }

    public class Small
    {
        public int? w { get; set; }
        public int? h { get; set; }
        public string resize { get; set; }
    }

    public class Large
    {
        public int? w { get; set; }
        public int? h { get; set; }
        public string resize { get; set; }
    }

    public class Sizes
    {
        public Medium2 medium { get; set; }
        public Thumb thumb { get; set; }
        public Small small { get; set; }
        public Large large { get; set; }
    }

    public class Medium
    {
        public long? id { get; set; }
        public string id_str { get; set; }
        public List<int> indices { get; set; }
        public string media_url { get; set; }
        public string media_url_https { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string expanded_url { get; set; }
        public string type { get; set; }
        public Sizes sizes { get; set; }
        public long? source_status_id { get; set; }
        public string source_status_id_str { get; set; }
        public long? source_user_id { get; set; }
        public string source_user_id_str { get; set; }
    }

    public class UserMention
    {
        public long? id { get; set; }
        public string id_str { get; set; }
        public int[] indices { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
    }

    public class Url
    {
        public string display_url { get; set; }
        public string expanded_url { get; set; }
        public int[] indices { get; set; }
        public string url { get; set; }
    }

    public class Hashtag
    {
        public int[] indices { get; set; }
        public string text { get; set; }
    }

    public class Entities
    {
        public List<Hashtag> hashtags { get; set; }
        public List<Url> urls { get; set; }
        public List<UserMention> user_mentions { get; set; }
        public List<object> symbols { get; set; }
        public List<Medium> media { get; set; }
    }

    public class Medium4
    {
        public int? w { get; set; }
        public int? h { get; set; }
        public string resize { get; set; }
    }

    public class Thumb2
    {
        public int? w { get; set; }
        public int? h { get; set; }
        public string resize { get; set; }
    }

    public class Small2
    {
        public int? w { get; set; }
        public int? h { get; set; }
        public string resize { get; set; }
    }

    public class Large2
    {
        public int? w { get; set; }
        public int? h { get; set; }
        public string resize { get; set; }
    }

    public class Sizes2
    {
        public Medium4 medium { get; set; }
        public Thumb2 thumb { get; set; }
        public Small2 small { get; set; }
        public Large2 large { get; set; }
    }

    public class Medium3
    {
        public long? id { get; set; }
        public string id_str { get; set; }
        public List<int> indices { get; set; }
        public string media_url { get; set; }
        public string media_url_https { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string expanded_url { get; set; }
        public string type { get; set; }
        public Sizes2 sizes { get; set; }
        public long? source_status_id { get; set; }
        public string source_status_id_str { get; set; }
        public long? source_user_id { get; set; }
        public string source_user_id_str { get; set; }
    }

    public class ExtendedEntities
    {
        public List<Medium3> media { get; set; }
    }

    public class Coordinates
    {
        public float[] coordinates { get; set; }
        public string type { get; set; }
    }


    public class Attributes
    {
        public string street_address { get; set; }
        public string sixtwothreeid { get; set; }
        public string twitter { get; set; }
    }

    public class BoundingBox
    {
        public List<List<List<double>>> coordinates { get; set; }
        public string type { get; set; }
    }

    public class Attributes2
    {
    }

    public class BoundingBox2
    {
        public List<List<List<double>>> coordinates { get; set; }
        public string type { get; set; }
    }

    public class ContainedWithin
    {
        public string name { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public Attributes2 attributes { get; set; }
        public string url { get; set; }
        public BoundingBox2 bounding_box { get; set; }
        public string id { get; set; }
        public string full_name { get; set; }
        public string place_type { get; set; }
    }

    public class Geometry
    {
        public List<double> coordinates { get; set; }
        public string type { get; set; }
    }

    public class Place
    {
        public string name { get; set; }
        public List<object> polylines { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public Attributes attributes { get; set; }
        public string url { get; set; }
        public BoundingBox bounding_box { get; set; }
        public string id { get; set; }
        public List<ContainedWithin> contained_within { get; set; }
        public string full_name { get; set; }
        public Geometry geometry { get; set; }
        public string place_type { get; set; }
    }

    public class Contributor
    {
        public long? id { get; set; }
        public string id_str { get; set; }
        public string screen_name { get; set; }
    }

    public class Status
    {
        public string created_at { get; set; }
        public long? id { get; set; }
        public string id_str { get; set; }
        public string text { get; set; }
        public string source { get; set; }
        public bool? truncated { get; set; }
        public long? in_reply_to_status_id { get; set; }
        public string in_reply_to_status_id_str { get; set; }
        public long? in_reply_to_user_id { get; set; }
        public string in_reply_to_user_id_str { get; set; }
        public string in_reply_to_screen_name { get; set; }
        public User user { get; set; }
        public object geo { get; set; }
        public Coordinates coordinates { get; set; }
        public Place place { get; set; }
        public List<Contributor> contributors { get; set; }
        public bool? is_quote_status { get; set; }
        public int? retweet_count { get; set; }
        public int? favorite_count { get; set; }
        public Entities entities { get; set; }
        public ExtendedEntities extended_entities { get; set; }
        public bool? favorited { get; set; }
        public bool? retweeted { get; set; }
        public bool? possibly_sensitive { get; set; }
        public string filter_level { get; set; }
        public string lang { get; set; }
        public string timestamp_ms { get; set; }
        public Status retweeted_status { get; set; }

        public override string ToString()
        {
            return Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(this)));
        }

        public static Status FromBase64String(string base64)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Status>(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(base64)));
        }
    }

}
