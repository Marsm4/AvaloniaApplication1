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
        public int Quantity { get; set; } = 1;
        public DateTimeOffset AddedDate { get; set; } = DateTimeOffset.UtcNow;
        // Навигационные свойства
        public virtual User? User { get; set; }
        public virtual Movie? Movie { get; set; }
    }
}
