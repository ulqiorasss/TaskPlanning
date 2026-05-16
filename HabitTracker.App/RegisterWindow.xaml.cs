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
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtName.Text;
            string login = TxtLogin.Text;
            string password = PbPassword.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new SmartTracker.HabitTrackerContext()) 
                {
                    if (db.Persons.Any(p => p.Login.ToLower() == login.ToLower()))
                    {
                        MessageBox.Show("Этот логин уже занят!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var newPerson = new Person
                    {
                        Name = name,
                        Login = login,
                        Password = password,
                        Role = UserRole.User 
                    };

                    db.Persons.Add(newPerson);
                    db.SaveChanges();
                }

                MessageBox.Show("Регистрация успешно завершена! Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

    

