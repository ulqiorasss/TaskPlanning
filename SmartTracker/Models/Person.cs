using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Text;
using TaskPlanning.Core.Models;

namespace SmartTracker.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set;}
        public string Password { get; set;}    
        public UserRole Role { get; set;}
        public DateTime CreatedAt { get; set; }
        public override string ToString()
        {
            return $"{Name}"; 
        }
        public ICollection<HabitTask> Tasks { get; set; }
    }
}
