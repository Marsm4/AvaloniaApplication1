using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System;
using System.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class BasketView : UserControl
    {
        public BasketView()
        {
            InitializeComponent();
            LoadBasket();
        }

        private void LoadBasket()
        {
            try
            {
                var currentUser = ContextData.CurrentLoggedInUser;
                if (currentUser == null) return;

                // ���������� ���� �������� ��� ���� ��������
                using var context = new AppDbContext();

                var basketItems = context.Baskets
                    .Where(b => b.UserId == currentUser.Id)
                    .ToList();

                var movies = context.Movies.ToList();

                var basketData = basketItems.Select(basket =>
                {
                    var movie = movies.FirstOrDefault(m => m.Id == basket.MovieId);
                    var localDate = basket.AddedDate.ToLocalTime();

                    return new
                    {
                        Id = basket.Id,
                        MovieTitle = movie?.Title ?? "����������� �����",
                        MovieGenre = movie?.Genre ?? "�� ������",
                        MovieDirector = movie?.Director ?? "�� ������",
                        Quantity = basket.Quantity,
                        AddedDate = localDate.ToString("dd.MM.yyyy HH:mm")
                    };
                }).ToList();

                BasketDataGrid.ItemsSource = basketData;

                BasketDataGrid.Columns.Clear();
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "�����", Binding = new Avalonia.Data.Binding("MovieTitle") });
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "����", Binding = new Avalonia.Data.Binding("MovieGenre"), });
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "��������", Binding = new Avalonia.Data.Binding("MovieDirector") });
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "����������", Binding = new Avalonia.Data.Binding("Quantity") });
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "���������", Binding = new Avalonia.Data.Binding("AddedDate") });

                UpdateTotalInfo();
            }
            catch (Exception ex)
            {
                ShowMessage($"������ �������� �������: {ex.Message}");
            }
        }

        private void UpdateTotalInfo()
        {
            var currentUser = ContextData.CurrentLoggedInUser;
            if (currentUser == null) return;

            using var context = new AppDbContext();

            var totalItems = context.Baskets
                .Count(b => b.UserId == currentUser.Id);

            var totalQuantity = context.Baskets
                .Where(b => b.UserId == currentUser.Id)
                .Sum(b => b.Quantity);

            TotalText.Text = $"�����: {totalItems} �������, {totalQuantity} ��.";
        }

        private void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            var selectedItem = BasketDataGrid.SelectedItem;
            if (selectedItem == null) return;

            var idProperty = selectedItem.GetType().GetProperty("Id");
            if (idProperty == null) return;

            var basketId = (int)idProperty.GetValue(selectedItem);

            using var context = new AppDbContext();
            var basketItem = context.Baskets.FirstOrDefault(b => b.Id == basketId);

            if (basketItem != null)
            {
                context.Baskets.Remove(basketItem);
                context.SaveChanges();
                LoadBasket();
            }
        }

        private void ClearBasket_Click(object? sender, RoutedEventArgs e)
        {
            var currentUser = ContextData.CurrentLoggedInUser;
            if (currentUser == null) return;

            using var context = new AppDbContext();

            var userBasketItems = context.Baskets
                .Where(b => b.UserId == currentUser.Id)
                .ToList();

            context.Baskets.RemoveRange(userBasketItems);
            context.SaveChanges();
            LoadBasket();
        }

        private void CheckoutButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var currentUser = ContextData.CurrentLoggedInUser;
                if (currentUser == null) return;

                using var context = new AppDbContext();

                var basketItems = context.Baskets
                    .Where(b => b.UserId == currentUser.Id)
                    .ToList();

                if (!basketItems.Any())
                {
                    ShowMessage("������� �����");
                    return;
                }

                context.Baskets.RemoveRange(basketItems);
                context.SaveChanges();

                ShowMessage($"����� ������� ��������! �������: {basketItems.Count}");
                LoadBasket();
            }
            catch (Exception ex)
            {
                ShowMessage($"������ ��� ���������� ������: {ex.Message}");
            }
        }

        private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var selectedItem = BasketDataGrid.SelectedItem;
            if (selectedItem == null) return;

            var idProperty = selectedItem.GetType().GetProperty("Id");
            if (idProperty == null) return;

            var basketId = (int)idProperty.GetValue(selectedItem);

            using var context = new AppDbContext();
            var basketItem = context.Baskets.FirstOrDefault(b => b.Id == basketId);

            if (basketItem != null)
            {
                ContextData.CurrentBasketItem = basketItem;
                var basketEditView = new BasketEditView();
                var window = new Window
                {
                    Content = basketEditView,
                    Title = "��������� ����������",
                    Width = 300,
                    Height = 200,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                await window.ShowDialog((Window)this.VisualRoot);
                LoadBasket();
            }
        }

        private void ShowMessage(string message)
        {
            var messageBox = new Window
            {
                Title = "����������",
                Content = new TextBlock { Text = message },
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            messageBox.ShowDialog((Window)this.VisualRoot);
        }
    }
}