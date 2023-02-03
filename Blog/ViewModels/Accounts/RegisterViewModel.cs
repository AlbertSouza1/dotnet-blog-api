using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}