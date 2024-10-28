namespace ProjectBase.Models
{
    public class BlogsModel
    {
        public long ID { get; set; }
        public long userID { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string description { get; set; }
        public bool status { get; set; }
        public DateTime publishAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string blog_picture { get; set; }
        public string link_media { get; set; }
        public string url { get; set;}
    }
}
