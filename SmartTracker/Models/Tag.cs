using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTracker.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<HabitTask> Tasks { get; set; }

    }
}
