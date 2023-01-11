using System.Collections.Generic;

namespace Blog.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public IList<Post> Posts { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            return !string.IsNullOrEmpty(Slug);
        }
    }
}