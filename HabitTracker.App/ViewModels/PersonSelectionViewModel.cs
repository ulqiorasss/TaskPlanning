using Microsoft.EntityFrameworkCore;
using SmartTracker;
using SmartTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace HabitTracker.App.ViewModels
{
    public class PersonSelectionViewModel : ViewModelBase
    {
        private readonly HabitTrackerContext _db;
        private Person? _selectedPerson;
        private string _newPersonName = string.Empty;
        public ObservableCollection<Person> People { get; set; }
        public Person? SelectedPerson
        {
            get => _selectedPerson;
            set => SetProperty(ref _selectedPerson, value);
        }
        public string NewPersonName
        {
            get => _newPersonName;
            set => SetProperty(ref _newPersonName, value);
        }
        public RelayCommand AddPersonCommand { get; }
        public RelayCommand LoginCommand { get; }
        public PersonSelectionViewModel()
        {
            _db = new HabitTrackerContext();
            _db.Persons.Load();
            People = _db.Persons.Local.ToObservableCollection();
            AddPersonCommand = new RelayCommand(obj =>
            {
                if (string.IsNullOrWhiteSpace(NewPersonName)) return;

                var newPerson = new Person { Name = NewPersonName };
                _db.Persons.Add(newPerson);
                _db.SaveChanges();

                NewPersonName = string.Empty; 
            });
            LoginCommand = new RelayCommand(obj =>
            {
                try
                {
                    if (SelectedPerson == null) return;
                    var mainWindow = new MainWindow();
                    mainWindow.DataContext = new MainViewModel(SelectedPerson);
                    mainWindow.Show();
                    if (obj is Window window)
                    {
                        window.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при инициализации главного окна: {ex.Message}");
                }
            }, obj => SelectedPerson != null);

        }
    }
}

