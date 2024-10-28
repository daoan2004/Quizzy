using System.Reflection.Metadata;

namespace ProjectBase.Models
{
    public class HomeViewModel
    {
        public List<SliderModel> Sliders { get; set; }
        public List<BlogsModel> LatestPosts { get; set; }
        public List<SubjectsModel> HotSubjects { get; set; }

        public List<SubjectsModel> Subjects { get; set; }
        public List<BlogsModel> BlogView { get; set; }
        
    }
}
