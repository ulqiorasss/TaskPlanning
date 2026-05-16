using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTracker.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public override string ToString()
        {
            return $"{Name}"; 
        }
        public ICollection<HabitTask> Tasks { get; set; }
    }
}
