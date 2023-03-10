using System.Collections.Generic;

namespace Blog.Models
{
    public class Category
    {
        public Category(int id, string name, string slug)
        {
            Id = id;
            Name = name;
            Slug = slug;
            Posts = new List<Post>();
        }

        public Category() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public IList<Post> Posts { get; set; }
    }
}