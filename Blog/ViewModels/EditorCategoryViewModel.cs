namespace Blog.ViewModels
{
    public class EditorCategoryViewModel
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            return !string.IsNullOrEmpty(Slug);
        }
    }
}