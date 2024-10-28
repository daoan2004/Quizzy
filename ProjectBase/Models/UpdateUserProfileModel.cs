namespace ProjectBase.Models
{
    public class UpdateUserProfileModel
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public DateOnly DOB { get; set; }
        public string Description { get; set; }
        public IFormFile Avatar { get; set; } // Thêm thuộc tính Avatar
    }

}
