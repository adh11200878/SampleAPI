namespace SampleAPI.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public IFormFile? Img { get; set; }
    }
}
