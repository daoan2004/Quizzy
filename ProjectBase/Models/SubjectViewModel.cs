namespace ProjectBase.Models
{
    public class SubjectViewModel
    {
        public IEnumerable<SubjectsModel> Subjects { get; set; }
        public RecipeModel UserRegistration { get; set; }
        public long UserID { get; set; }
    }
}
