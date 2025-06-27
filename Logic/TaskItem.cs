// Matteo Nusca ST10440432
using System;

namespace St10440432CyberBotGUI
{
    public class TaskItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }

        public override string ToString()
        {
            string status = IsCompleted ? "✅ Completed" : "🕒 Pending";
            string reminder = ReminderDate.HasValue ? $"⏰ Remind on {ReminderDate.Value:dd MMM yyyy}" : "No reminder set";
            return $"• {Title}: {Description}\n  {status} — {reminder}";
        }
    }
}