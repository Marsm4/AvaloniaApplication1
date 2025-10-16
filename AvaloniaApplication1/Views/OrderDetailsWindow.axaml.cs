using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1
{
    public partial class OrderDetailsWindow : Window
    {
        private Order _order;

        public OrderDetailsWindow(Order order)
        {
            InitializeComponent();
            _order = order;
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            using var context = new AppDbContext();

            var orderDetails = context.OrderItems
                .Where(oi => oi.OrderId == _order.Id)
                .Join(context.Movies,
                      oi => oi.MovieId,
                      m => m.Id,
                      (oi, m) => new
                      {
                          MovieTitle = m.Title,
                          MovieGenre = m.Genre,
                          MovieDirector = m.Director,
                          Quantity = oi.Quantity
                      })
                .ToList();

            tbOrderInfo.Text = $"Заказ №{_order.Id} от {_order.OrderDate:dd.MM.yyyy HH:mm} - {_order.Status}";
            dgOrderItems.ItemsSource = orderDetails;
        }

        private void btnClose_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}