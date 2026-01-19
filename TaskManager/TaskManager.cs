using System.Collections.Generic;
using System.Linq;

namespace TaskManager
{
    public class TaskManager
    {
        private List<Task> tasks = new List<Task>();

        public void AddTask(string title)
        {
            tasks.Add(new Task(title));
        }

        public bool RemoveTask(string title)
        {
            var task = tasks.FirstOrDefault(t => t.Title == title);
            if (task != null)
            {
                tasks.Remove(task);
                return true;
            }
            return false;
        }

        public List<Task> GetTasks()
        {
            return new List<Task>(tasks);
        }
    }
}