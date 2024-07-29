using System.ComponentModel.DataAnnotations;

namespace AnimeLibrary.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class UserProfileViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
    }
    public class EditProfileViewModel
    {
        public string UserName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Bio { get; set; }
    }
    public class ViewedContentViewModel
    {
        public List<AnimeViewModel> ViewedAnimes { get; set; }
        public List<MangaViewModel> ViewedMangas { get; set; }
    }

}
