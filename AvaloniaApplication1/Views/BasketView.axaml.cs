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

                using var context = new AppDbContext();

                // Загружаем корзину и фильмы отдельно
                var basketItems = context.Baskets
                    .Where(b => b.UserId == currentUser.Id)
                    .ToList();

                var movies = context.Movies.ToList();

                var basketData = basketItems.Select(basket =>
                {
                    var movie = movies.FirstOrDefault(m => m.Id == basket.MovieId);

                    // Конвертируем UTC время в локальное для отображения
                    var localDate = basket.AddedDate.ToLocalTime();

                    return new
                    {
                        Id = basket.Id,
                        MovieTitle = movie?.Title ?? "Неизвестный фильм",
                        MovieGenre = movie?.Genre ?? "Не указан",
                        MovieDirector = movie?.Director ?? "Не указан",
                        Quantity = basket.Quantity,
                        AddedDate = localDate.ToString("dd.MM.yyyy HH:mm")
                    };
                }).ToList();

                BasketDataGrid.ItemsSource = basketData;

                // Настройка колонок



                BasketDataGrid.Columns.Clear();
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "Фильм", Binding = new Avalonia.Data.Binding("MovieTitle")});
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "Жанр", Binding = new Avalonia.Data.Binding("MovieGenre"),});
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "Режиссер", Binding = new Avalonia.Data.Binding("MovieDirector") });
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new Avalonia.Data.Binding("Quantity") });
                BasketDataGrid.Columns.Add(new DataGridTextColumn { Header = "Добавлено", Binding = new Avalonia.Data.Binding("AddedDate") });







                UpdateTotalInfo();
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка загрузки корзины: {ex.Message}");
            }
        }

        private void UpdateTotalInfo()
        {
            var currentUser = ContextData.CurrentLoggedInUser;
            if (currentUser == null) return;

            var totalItems = App.dbContext.Baskets
                .Count(b => b.UserId == currentUser.Id);

            var totalQuantity = App.dbContext.Baskets
                .Where(b => b.UserId == currentUser.Id)
                .Sum(b => b.Quantity);

            TotalText.Text = $"Всего: {totalItems} позиций, {totalQuantity} шт.";
        }

        private void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            var selectedItem = BasketDataGrid.SelectedItem;
            if (selectedItem == null) return;

            // Получаем ID через reflection
            var idProperty = selectedItem.GetType().GetProperty("Id");
            if (idProperty == null) return;

            var basketId = (int)idProperty.GetValue(selectedItem);
            var basketItem = App.dbContext.Baskets.FirstOrDefault(b => b.Id == basketId);

            if (basketItem != null)
            {
                App.dbContext.Baskets.Remove(basketItem);
                App.dbContext.SaveChanges();
                LoadBasket();
            }
        }

        private void ClearBasket_Click(object? sender, RoutedEventArgs e)
        {
            var currentUser = ContextData.CurrentLoggedInUser;
            if (currentUser == null) return;

            var userBasketItems = App.dbContext.Baskets
                .Where(b => b.UserId == currentUser.Id)
                .ToList();

            App.dbContext.Baskets.RemoveRange(userBasketItems);
            App.dbContext.SaveChanges();
            LoadBasket();
        }



        private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var selectedItem = BasketDataGrid.SelectedItem;
            if (selectedItem == null) return;

            var idProperty = selectedItem.GetType().GetProperty("Id");
            if (idProperty == null) return;

            var basketId = (int)idProperty.GetValue(selectedItem);
            var basketItem = App.dbContext.Baskets.FirstOrDefault(b => b.Id == basketId);

            if (basketItem != null)
            {
                ContextData.CurrentBasketItem = basketItem;
                var basketEditView = new BasketEditView();
                var window = new Window
                {
                    Content = basketEditView,
                    Title = "Изменение количества",
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