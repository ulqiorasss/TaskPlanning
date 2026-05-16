using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SmartTracker.Models
{
    public class HabitTask
    {
        public int Id { get; set; }
        public int PersonalId{ get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public override string ToString()
        {
            if (IsCompleted == true)
                return $"Название:{Title} Задача:{Description} Выполнено";
            else
                return $"Название:{Title} Задача:{Description} Не выполнено";
        }
        public Person? Person { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}
