namespace ProjectBase.Models
{
    public class BlogsModel
    {
        public long ID { get; set; }
        public long userID { get; set; }
        public string title { get; set; } = null!;
        public string body { get; set; } = null!;
        public string description { get; set; } = null!;
        public bool status { get; set; }
        public DateTime publishAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string blog_picture { get; set; } = null!;
        public string link_media { get; set; } = null!;
        public string url { get; set; } = null!;
    }
}
