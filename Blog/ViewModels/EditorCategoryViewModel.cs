using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class EditorCategoryViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Slug { get; set; }
    }
}