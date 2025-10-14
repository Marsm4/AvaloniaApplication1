using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Data
{
    public partial class Category
    {
        public int Id { get; set; }

        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public virtual ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}

