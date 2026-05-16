using HabitTracker.App;
using System.Windows.Media;
using HabitTracker.App.ViewModels;
using SmartTracker;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SmartTracker.Models;
using TaskPlanning.Core.Models;

namespace TaskPlanning.App.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            try
            {
                _db.Database.EnsureCreated();

                if (!_db.Persons.Any(p => p.Role == UserRole.Admin))
                {
                    var admin = new Person
                    {
                        Name = "Главный Администратор",
                        Login = "admin",
                        Password = "123", 
                        Role = UserRole.Admin
                    };

                    _db.Persons.Add(admin);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации первого входа: {ex.Message}");
            }
        }

        private readonly HabitTrackerContext _db = new HabitTrackerContext();

        public string Login { get; set; }
        public string Message { get; set; }
        public Brush MessageColor { get; set; } = Brushes.Red;

        private RelayCommand _loginCommand;
        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ??= new RelayCommand(_ =>
                {
                    var passwordBox = GetPasswordBox();

                    if (passwordBox != null)
                    {
                        string actualPassword = passwordBox.Password;

                        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(actualPassword))
                        {
                            ShowMessage("Заполните логин и пароль!", Brushes.Red);
                            return;
                        }

                        var user = _db.Persons.FirstOrDefault(p => p.Login == Login && p.Password == actualPassword);

                        if (user != null)
                        {
                            var mainWindow = new MainWindow();
                            mainWindow.DataContext = new MainViewModel(user);
                            mainWindow.Show();

                            CloseLoginWindow();
                        }
                        else
                        {
                            ShowMessage("Неверный логин или пароль!", Brushes.Red);
                        }
                    }
                });
            }
        }

        private RelayCommand _registerCommand;
        public RelayCommand RegisterCommand
        {
            get
            {
                return _registerCommand ??= new RelayCommand(_ =>
                {
                    var registerWin = new RegisterWindow();

                    registerWin.Owner = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().FirstOrDefault(w => w.DataContext == this);

                    registerWin.ShowDialog();
                });
            }
        }

        private void ShowMessage(string text, Brush color)
        {
            Message = text;
            MessageColor = color;
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(MessageColor));
        }

        private PasswordBox GetPasswordBox()
        {
            var loginWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            return loginWindow?.FindName("PbPassword") as PasswordBox;
        }

        private void CloseLoginWindow()
        {
            var loginWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            loginWindow?.Close();
        }
    }
}

