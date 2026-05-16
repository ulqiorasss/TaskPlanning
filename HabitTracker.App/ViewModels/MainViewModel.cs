using Microsoft.EntityFrameworkCore;
using SmartTracker;
using SmartTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using TaskPlanning.App;
using TaskPlanning.Core.Models;

namespace HabitTracker.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private HabitTrackerContext _db;
        private readonly Person _currentPerson;
        private ObservableCollection<HabitTask> _tasks = new ObservableCollection<HabitTask>();
        private HabitTask _selectedTask;
        private string _selectedPeriod = "Все";
        private Tag _selectedTag;
        private ObservableCollection<Tag> _availableTags = new ObservableCollection<Tag>();

        public Person CurrentPerson => _currentPerson;

        public bool IsAdmin => _currentPerson?.Role == UserRole.Admin;
        public bool IsUser => _currentPerson?.Role == UserRole.User;

        public ObservableCollection<HabitTask> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }

        public HabitTask SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        public double Progress
        {
            get
            {
                if (_tasks == null || !_tasks.Any()) return 0;
                double completed = _tasks.Count(t => t.IsCompleted);
                double total = _tasks.Count;
                return (completed / total) * 100;
            }
        }

        public List<string> Periods { get; } = new() { "Все", "Сегодня", "Неделя" };

        public string SelectedPeriod
        {
            get => _selectedPeriod;
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    LoadTasks();
                }
            }
        }

        public Tag SelectedTag
        {
            get => _selectedTag;
            set
            {
                if (SetProperty(ref _selectedTag, value))
                {
                    LoadTasks();
                }
            }
        }

        public ObservableCollection<Tag> AvailableTags
        {
            get => _availableTags;
            set => SetProperty(ref _availableTags, value);
        }

        public RelayCommand DeleteTaskCommand { get; }
        public RelayCommand ToggleTaskCommand { get; }
        public RelayCommand AddTaskCommand { get; }
        public RelayCommand EditTaskCommand { get; }

        private RelayCommand _openDetailsCommand;
        public RelayCommand OpenDetailsCommand
        {
            get
            {
                return _openDetailsCommand ??= new RelayCommand(param =>
                {
                    if (param is HabitTask task)
                    {
                        var infoWindow = new TaskInfoWindow();
                        infoWindow.DataContext = task;
                        infoWindow.ShowDialog();
                    }
                });
            }
        }

        public MainViewModel(Person authorizedUser)
        {
            _currentPerson = authorizedUser ?? throw new ArgumentNullException(nameof(authorizedUser));

            try
            {
                _db = new HabitTrackerContext();
                _db.Database.EnsureCreated();

                LoadTasks();

                var tagsFromDb = _db.Tags.ToList();
                tagsFromDb.Insert(0, new Tag { Id = 0, Name = "Все теги" });
                AvailableTags = new ObservableCollection<Tag>(tagsFromDb);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}");
            }

            ToggleTaskCommand = new RelayCommand(obj =>
            {
                _db.SaveChanges();
                OnPropertyChanged(nameof(Progress));
            });

            AddTaskCommand = new RelayCommand(obj =>
            {
                AddTaskWindow addTaskWindow = new AddTaskWindow();
                addTaskWindow.Owner = Application.Current.MainWindow;
                addTaskWindow.Title = "Создание новой задачи";
                addTaskWindow.TitleTextBox.Text = string.Empty;
                addTaskWindow.DescriptionTextBox.Text = string.Empty;
                addTaskWindow.DueDatePicker.SelectedDate = DateTime.Today;

                if (addTaskWindow.ShowDialog() == true)
                {
                    var newTask = addTaskWindow.CreatedTask;
                    newTask.PersonalId = _currentPerson.Id;
                    string tagName = addTaskWindow.EnteredTagName;

                    if (!string.IsNullOrWhiteSpace(tagName))
                    {
                        var existingTag = _db.Tags.FirstOrDefault(t => t.Name.ToLower() == tagName.ToLower());
                        if (existingTag != null)
                        {
                            newTask.Tags = new List<Tag> { existingTag };
                        }
                        else
                        {
                            var newTag = new Tag { Name = tagName };
                            newTask.Tags = new List<Tag> { newTag };
                        }
                    }

                    _db.HabitTasks.Add(newTask);
                    _db.SaveChanges();

                    var tagsFromDb = _db.Tags.ToList();
                    tagsFromDb.Insert(0, new Tag { Name = "Все теги" });
                    LoadTasks();
                }
            });

            EditTaskCommand = new RelayCommand(obj =>
            {
                HabitTask selectedTask = (obj as HabitTask) ?? SelectedTask;
                if (selectedTask != null)
                {
                    AddTaskWindow editWindow = new AddTaskWindow();
                    editWindow.Owner = Application.Current.MainWindow;
                    editWindow.Title = "Редактирование задачи";
                    editWindow.TitleTextBox.Text = selectedTask.Title;
                    editWindow.DueDatePicker.SelectedDate = selectedTask.DueDate;
                    editWindow.DescriptionTextBox.Text = selectedTask.Description;

                    if (selectedTask.Tags != null && selectedTask.Tags.Any())
                    {
                        editWindow.TagTextBox.Text = selectedTask.Tags.First().Name;
                    }

                    if (editWindow.ShowDialog() == true)
                    {
                        selectedTask.Title = editWindow.CreatedTask.Title;
                        selectedTask.DueDate = editWindow.CreatedTask.DueDate;
                        selectedTask.Description = editWindow.CreatedTask.Description;
                        string tagName = editWindow.EnteredTagName;

                        if (!string.IsNullOrWhiteSpace(tagName))
                        {
                            var existingTag = _db.Tags.FirstOrDefault(t => t.Name.ToLower() == tagName.ToLower());
                            if (existingTag != null)
                            {
                                selectedTask.Tags = new List<Tag> { existingTag };
                            }
                            else
                            {
                                var newTag = new Tag { Name = tagName };
                                selectedTask.Tags = new List<Tag> { newTag };
                            }
                        }

                        _db.HabitTasks.Update(selectedTask);
                        _db.SaveChanges();
                        LoadTasks();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите задачу из списка для редактирования!",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            DeleteTaskCommand = new RelayCommand(obj =>
            {
                HabitTask selectedTask = (obj as HabitTask) ?? SelectedTask;
                if (selectedTask != null)
                {
                    var result = MessageBox.Show($"Вы действительно хотите удалить задачу \"{selectedTask.Title}\"?",
                        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _db.HabitTasks.Remove(selectedTask);
                        _db.SaveChanges();
                        LoadTasks();
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите задачу из списка для удаления!",
                        "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            SelectedPeriod = "Все";
        }

        private void LoadTasks()
        {
            try
            {
                IQueryable<HabitTask> query = _db.HabitTasks.Include(t => t.Tags).AsQueryable();

                if (!IsAdmin && _currentPerson != null)
                {
                    query = query.Where(t => t.PersonalId == _currentPerson.Id);
                }

                var today = DateTime.Today;
                if (!string.IsNullOrEmpty(SelectedPeriod) && SelectedPeriod != "Все")
                {
                    if (SelectedPeriod == "Сегодня")
                    {
                        query = query.Where(t => t.DueDate.Date == today);
                    }
                    else if (SelectedPeriod == "Неделя")
                    {
                        var nextWeek = today.AddDays(7);
                        query = query.Where(t => t.DueDate.Date >= today && t.DueDate.Date <= nextWeek);
                    }
                    else if (SelectedPeriod == "Месяц") 
                    {
                        var nextMonth = today.AddMonths(1);
                        query = query.Where(t => t.DueDate.Date >= today && t.DueDate.Date <= nextMonth);
                    }
                }

                if (SelectedTag != null && SelectedTag.Id != 0)
                {
                    int searchTagId = SelectedTag.Id;
                    query = query.Where(t => t.Tags.Any(tag => tag.Id == searchTagId));
                }

                _tasks.Clear();
                foreach (var task in query.ToList())
                {
                    _tasks.Add(task);
                }
                OnPropertyChanged(nameof(Tasks));
                OnPropertyChanged(nameof(Progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}");
            }
        }
    }
}


