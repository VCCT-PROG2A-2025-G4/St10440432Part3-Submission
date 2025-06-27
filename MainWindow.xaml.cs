// Matteo Nusca ST10440432
// DECLARATION OF AI
// Artificial intelligence was used in this project for layout design and some helper methods

using St10440432CyberBotGUI;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace St10440432CyberBotGUI
{
    public partial class MainWindow : Window
    {
        private readonly Speech _speech; // Text-to-speech engine
        private readonly Bot _bot;       // Core chatbot logic

        public MainWindow()
        {
            InitializeComponent(); // loads GUI
            _speech = new Speech(); // init speech engine
            _bot = new Bot(_speech); // init bot with speech

            ShowBotGreeting(); // run greeting on launch
        }

        // Shows the welcome greeting when the app loads
        private async void ShowBotGreeting()
        {
            _speech.PlayVoiceGreeting(); // plays WAV file
            await AddBotMessageAnimated("👋 Welcome! I am your Cybersecurity Awareness Bot.");
            await AddBotMessageAnimated("💬 Type a message below and press Send to begin.");
        }

        // Button click event → processes user input
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserInput.Text.Trim();

            // input empty
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                await AddBotMessageAnimated("❗ Please type something for me to respond to.");
                return;
            }

            AddUserMessage(userMessage); // show user text
            string response = _bot.GetResponse(userMessage); // get bot reply
            await AddBotMessageAnimated(response); // show bot response with typing + speech

            UserInput.Clear(); // clear input box
        }

        // Shows user message in chat panel
        private void AddUserMessage(string text)
        {
            TextBlock userBlock = new TextBlock
            {
                Text = $"🧑 You: {text}",
                Foreground = Brushes.LightBlue,
                Margin = new Thickness(5),
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };
            ChatPanel.Children.Add(userBlock);
            ScrollToBottom();
        }

        // Shows bot message with typing animation and speech
        private async Task AddBotMessageAnimated(string text)
        {
            TextBlock botBlock = new TextBlock
            {
                Text = "🤖 Bot: ",
                Foreground = Brushes.LightGreen,
                Margin = new Thickness(5),
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };
            ChatPanel.Children.Add(botBlock);
            ScrollToBottom();

            _speech.Speak(text); // 🔊 Start speaking while typing

            // simulate typing
            foreach (char c in text)
            {
                botBlock.Text += c;
                ScrollToBottom();
                await Task.Delay(30); // delay between each character
            }
        }

        // scrolls down automatically
        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToEnd();
        }
    }
}
