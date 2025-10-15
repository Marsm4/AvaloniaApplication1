using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class OrdersView : UserControl
    {
        public OrdersView()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            var currentUser = ContextData.CurrentLoggedInUser;
            if (currentUser == null) return;

            // Получаем заказы с информацией о количестве товаров
            var orders = App.dbContext.Orders
                .Where(o => o.UserId == currentUser.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var ordersData = orders.Select(order => new
            {
                Id = order.Id,
                OrderDate = order.OrderDate.ToString("dd.MM.yyyy HH:mm"),
                Status = order.Status,
                ItemsCount = App.dbContext.OrderItems.Count(oi => oi.OrderId == order.Id),
                TotalQuantity = App.dbContext.OrderItems.Where(oi => oi.OrderId == order.Id).Sum(oi => oi.Quantity)
            }).ToList();

            OrdersDataGrid.ItemsSource = ordersData;

            OrdersDataGrid.Columns.Clear();
            OrdersDataGrid.Columns.Add(new DataGridTextColumn { Header = "№ Заказа", Binding = new Avalonia.Data.Binding("Id") });
            OrdersDataGrid.Columns.Add(new DataGridTextColumn { Header = "Дата заказа", Binding = new Avalonia.Data.Binding("OrderDate") });
            OrdersDataGrid.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new Avalonia.Data.Binding("Status") });
            OrdersDataGrid.Columns.Add(new DataGridTextColumn { Header = "Кол-во позиций", Binding = new Avalonia.Data.Binding("ItemsCount") });
            OrdersDataGrid.Columns.Add(new DataGridTextColumn { Header = "Общее кол-во", Binding = new Avalonia.Data.Binding("TotalQuantity") });
        }

        private void ViewDetails_Click(object? sender, RoutedEventArgs e)
        {
            var selectedOrder = OrdersDataGrid.SelectedItem;
            if (selectedOrder == null)
            {
                ShowMessage("Выберите заказ для просмотра деталей");
                return;
            }

            var idProperty = selectedOrder.GetType().GetProperty("Id");
            if (idProperty == null) return;

            var orderId = (int)idProperty.GetValue(selectedOrder);
            var order = App.dbContext.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order != null)
            {
                ContextData.CurrentOrder = order;
                var orderDetailsView = new OrderDetailsView();
                var window = new Window
                {
                    Content = orderDetailsView,
                    Title = $"Детали заказа №{order.Id}",
                    Width = 600,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                window.ShowDialog((Window)this.VisualRoot);
                LoadOrders(); // Перезагружаем список после закрытия окна деталей
            }
        }

        private void RefreshButton_Click(object? sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            ViewDetails_Click(sender, e);
        }

        private void ShowMessage(string message)
        {
            var messageBox = new Window
            {
                Title = "Информация",
                Content = new TextBlock { Text = message },
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            messageBox.ShowDialog((Window)this.VisualRoot);
        }
    }
}