using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class OrderDetailsView : UserControl
    {
        public OrderDetailsView()
        {
            InitializeComponent();
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            if (ContextData.CurrentOrder == null) return;

            var order = ContextData.CurrentOrder;

            // Получаем информацию о заказе через OrderItems
            var orderDetails = App.dbContext.OrderItems
                .Where(oi => oi.OrderId == order.Id)
                .Join(App.dbContext.Movies,
                      oi => oi.MovieId,
                      m => m.Id,
                      (oi, m) => new
                      {
                          OrderId = order.Id,
                          OrderDate = order.OrderDate,
                          Status = order.Status,
                          MovieTitle = m.Title,
                          MovieGenre = m.Genre,
                          MovieDirector = m.Director,
                          Quantity = oi.Quantity
                      })
                .ToList();

            if (orderDetails.Any())
            {
                var firstItem = orderDetails.First();
                OrderInfoText.Text = $"Заказ №{firstItem.OrderId} от {firstItem.OrderDate:dd.MM.yyyy HH:mm} - {firstItem.Status}";
                OrderItemsDataGrid.ItemsSource = orderDetails;

                OrderItemsDataGrid.Columns.Clear();
                OrderItemsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Фильм", Binding = new Avalonia.Data.Binding("MovieTitle") });
                OrderItemsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Жанр", Binding = new Avalonia.Data.Binding("MovieGenre") });
                OrderItemsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Режиссер", Binding = new Avalonia.Data.Binding("MovieDirector") });
                OrderItemsDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Avalonia.Data.Binding("Quantity") });
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            var window = (Window)this.VisualRoot;
            window?.Close();
        }
    }
}