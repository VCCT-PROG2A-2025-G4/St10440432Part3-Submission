// ST10440432

// DECLARATION OF AI

// Some AI used to assist with complex string manipulation
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace St10440432CyberBotGUI
{
    public class TaskManager
    {
        private readonly List<TaskItem> _tasks = new(); // Stores user tasks

        // Main handler for user task input
        public string HandleTaskInput(string input, out TaskItem createdTask)
        {
            createdTask = null;
            input = input.ToLower().Trim();

            // ensures correct task selection
            if (input.StartsWith("add task") || input.StartsWith("remind me"))
                return AddTask(input, out createdTask);
            else if (input.StartsWith("delete task") || input.StartsWith("remove task"))
                return DeleteTask(input);
            else if (input.StartsWith("complete task") || input.StartsWith("mark task"))
                return CompleteTask(input);
            else if (input.Contains("show tasks") || input.Contains("list tasks"))
                return ShowAllTasks();

            // error handling
            return "⚠️ I didn’t understand your task request. Try:\n• 'Add task: Review privacy settings in 3 days'\n• 'Delete task 1'";
        }

        // Adds a new task
        private string AddTask(string input, out TaskItem createdTask)
        {
            createdTask = new TaskItem();

            // Regex to capture title and date phrase (AI used)
            string pattern = @"(?<title>.+?)( in (?<days>\d+) days| on (?<date>\d{1,2} \w+))?$";

            // Clean input
            string cleaned = input.Replace("add task", "")
                                  .Replace("remind me to", "")
                                  .Replace("remind me", "")
                                  .Trim(':', ' ', '-');

            Match match = Regex.Match(cleaned, pattern, RegexOptions.IgnoreCase);

            if (!match.Success)
                return "⚠️ Couldn't extract task details. Try: 'Add task: Enable 2FA in 3 days'.";

         
            createdTask.Title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(match.Groups["title"].Value.Trim());
            createdTask.Description = createdTask.Title;

            // Parse reminder by days or specific date
            if (int.TryParse(match.Groups["days"].Value, out int days))
                createdTask.ReminderDate = DateTime.Today.AddDays(days);
            else if (DateTime.TryParseExact(match.Groups["date"].Value, "d MMMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime specificDate))
                createdTask.ReminderDate = specificDate;

            createdTask.IsCompleted = false;
            _tasks.Add(createdTask);

            return $"🗒️ Task Added:\n{createdTask}";
        }

        // Lists all tasks with details and status
        public string ShowAllTasks()
        {
            if (_tasks.Count == 0)
                return "📭 You have no tasks.";

            string output = "📋 Your Tasks:\n";

            for (int i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                output += $"#{i + 1}: {task.Title}\n   {task.Description}\n   {(task.IsCompleted ? "✅ Completed" : "🕒 Pending")}";
                if (task.ReminderDate.HasValue)
                    output += $" — ⏰ {task.ReminderDate.Value:dd MMM yyyy}";
                output += "\n\n";
            }

            return output.Trim();
        }

        // Deletes task by number
        private string DeleteTask(string input)
        {
            string keyword = input.Replace("delete task", "").Replace("remove task", "").Trim();

            if (int.TryParse(keyword, out int index) && index >= 1 && index <= _tasks.Count)
            {
                var task = _tasks[index - 1];
                _tasks.RemoveAt(index - 1);
                return $"🗑️ Task #{index} '{task.Title}' deleted.";
            }

            return "❌ Task not found. Use 'show tasks' to view task numbers.";
        }

        // Marks a task as completed by number
        private string CompleteTask(string input)
        {
            string keyword = input.Replace("complete task", "").Replace("mark task", "").Trim();

            if (int.TryParse(keyword, out int index) && index >= 1 && index <= _tasks.Count)
            {
                var task = _tasks[index - 1];
                if (task.IsCompleted)
                    return $"✔️ Task #{index} is already completed.";

                task.IsCompleted = true;
                return $"✅ Task #{index} '{task.Title}' marked as completed.";
            }

            return "❌ Task not found. Use 'show tasks' to see the list.";
        }
    }
}
