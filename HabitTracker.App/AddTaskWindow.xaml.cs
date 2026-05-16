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

namespace TaskPlanning.App
{
    /// <summary>
    /// Логика взаимодействия для AddTaskWindow.xaml
    /// </summary>
    public partial class AddTaskWindow : Window
    {
        public HabitTask CreatedTask { get; set; }
        public string EnteredTagName => TagTextBox.Text.Trim();
        public AddTaskWindow()
        {
            InitializeComponent();
            DueDatePicker.SelectedDate = DateTime.Today;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните поле Название!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CreatedTask = new HabitTask
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text,
                DueDate = DueDatePicker.SelectedDate ?? DateTime.Today,
                IsCompleted = false,
                CreatedAt = DateTime.Now
            };

            DialogResult = true;
        }
    }
}

