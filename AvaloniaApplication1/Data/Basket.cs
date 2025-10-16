using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Data
{
    public partial class Basket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow; // Измените на UTC

        public virtual User? User { get; set; }
        public virtual Movie? Movie { get; set; }
    }
}
