using HabitTracker.App;
using SmartTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskPlanning.Core.Models;

namespace TaskPlanning.App
{
    /// <summary>
    /// Логика взаимодействия для TaskInfoWindow.xaml
    /// </summary>
    public partial class TaskInfoWindow : Window
    {
        private HabitTask _currentTask;

        public TaskInfoWindow()
        {
            InitializeComponent();
            this.Loaded += TaskInfoWindow_Loaded;
        }

        private void TaskInfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _currentTask = this.DataContext as HabitTask;

            if (_currentTask == null) return;

            try
            {
                using (var db = new SmartTracker.HabitTrackerContext())
                {
                    var mainWin = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    var mainVm = mainWin?.DataContext as HabitTracker.App.ViewModels.MainViewModel;

                    if (mainVm != null && mainVm.IsAdmin)
                    {
                        AdminPanel.Visibility = Visibility.Visible;

                        var users = db.Persons.Where(p => p.Role == UserRole.User).ToList();
                        CbUsers.ItemsSource = users;
                    }
                    else
                    {
                        AdminPanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации списка пользователей: {ex.Message}");
            }
        }
        private void BtnShare_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = CbUsers.SelectedItem as Person;

            if (selectedUser == null)
            {
                MessageBox.Show("Пожалуйста, выберите пользователя из списка!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new SmartTracker.HabitTrackerContext())
                {
                    var sharedTask = new HabitTask
                    {
                        Title = _currentTask.Title,
                        Description = _currentTask.Description,
                        DueDate = _currentTask.DueDate,
                        IsCompleted = false, 
                        PersonalId = selectedUser.Id 
                    };

                    db.HabitTasks.Add(sharedTask);
                    db.SaveChanges();
                }

                MessageBox.Show($"Задача успешно отправлена пользователю {selectedUser.Name}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось отправить задачу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

