// Matteo Nusca ST10440432
using System;
using System.Collections.Generic;
using System.Linq;

namespace St10440432CyberBotGUI
{
    public class QuizManager
    {
        private int _score = 0;
        private int _currentIndex = 0;
        private List<QuizQuestion> _allQuestions;   // 30 questions
        private List<QuizQuestion> _quizQuestions;  // 10 selected questions for quiz
        private bool _inProgress = false;           // Whether a quiz is currently active

        public bool IsInProgress
        {
            get { return _inProgress; }
        }

        public QuizManager()
        {
            // Initialize the full pool of questions
            _allQuestions = new List<QuizQuestion>
            {
                new QuizQuestion("What does 2FA stand for?", new[] { "Two-Factor Authentication", "Two-Firewall Access", "Trusted Fallback Account", "None" }, 'A', "2FA adds an extra layer of login security."),
                new QuizQuestion("Is it safe to reuse passwords?", new[] { "Yes", "Only on trusted sites", "No", "Sometimes" }, 'C', "Reusing passwords increases risk of compromise."),
                new QuizQuestion("What is phishing?", new[] { "Email scam", "Firewall update", "Cloud service", "Safe practice" }, 'A', "Phishing tries to trick users into revealing data."),
                new QuizQuestion("What should you do before clicking a link in an email?", new[] { "Click immediately", "Hover to preview", "Ignore it", "Forward to a friend" }, 'B', "Hovering shows where the link leads."),
                new QuizQuestion("True or False: Antivirus should be updated regularly.", new[] { "True", "False" }, 'A', "Updates include new virus definitions."),
                new QuizQuestion("Which password is strongest?", new[] { "123456", "password123", "S8#lT!z9", "qwerty" }, 'C', "Strong passwords use symbols and unpredictability."),
                new QuizQuestion("What’s a secure way to connect on public Wi-Fi?", new[] { "Just browse normally", "Use VPN", "Use Bluetooth", "Turn off firewall" }, 'B', "VPN encrypts your traffic."),
                new QuizQuestion("Should you open email attachments from unknown senders?", new[] { "Yes", "Only images", "Never", "If it looks clean" }, 'C', "Attachments may contain malware."),
                new QuizQuestion("True or False: Software updates improve security.", new[] { "True", "False" }, 'A', "They patch vulnerabilities."),
                new QuizQuestion("Which is a form of malware?", new[] { "Trojan", "2FA", "Firewall", "VPN" }, 'A', "Trojans are harmful disguised programs."),
                new QuizQuestion("What is a firewall used for?", new[] { "Cooking", "Blocking unauthorized access", "Speeding up internet", "Deleting data" }, 'B', "Firewalls block harmful traffic."),
                new QuizQuestion("How often should you back up important data?", new[] { "Never", "Monthly", "Only when needed", "Regularly" }, 'D', "Frequent backups prevent data loss."),
                new QuizQuestion("What is social engineering?", new[] { "Building software", "Manipulating people to gain info", "Encrypting files", "A design concept" }, 'B', "It tricks users into giving access."),
                new QuizQuestion("Which of these is a strong password?", new[] { "banana", "Password123", "Lk@7#pQ2!", "admin" }, 'C', "Strong passwords are long and random."),
                new QuizQuestion("Why is public Wi-Fi risky?", new[] { "It’s slow", "It can be intercepted", "It’s secure", "None of the above" }, 'B', "Unencrypted traffic can be sniffed."),
                new QuizQuestion("What is the function of antivirus software?", new[] { "Create viruses", "Protect against malware", "Slow down computer", "Improve graphics" }, 'B', "It detects and removes malicious software."),
                new QuizQuestion("How can you recognize a phishing email?", new[] { "Strange links", "Grammar errors", "Unexpected sender", "All of the above" }, 'D', "Phishing emails often have these signs."),
                new QuizQuestion("Should you update your operating system?", new[] { "No", "Only if slow", "Yes", "Only for new features" }, 'C', "Updates often fix security holes."),
                new QuizQuestion("What is encryption?", new[] { "Data destruction", "Data conversion for security", "Data backup", "Data duplication" }, 'B', "Encryption secures data from unauthorized access."),
                new QuizQuestion("Which of these is safest for storing passwords?", new[] { "Browser", "Notebook", "Password manager", "Sticky note" }, 'C', "Password managers encrypt and store securely."),
                new QuizQuestion("What is ransomware?", new[] { "Security update", "Data recovery tool", "Malware that locks data", "Firewall" }, 'C', "It encrypts data and demands ransom."),
                new QuizQuestion("What is a VPN?", new[] { "Virus Protection Network", "Virtual Private Network", "Video Performance Network", "None" }, 'B', "VPN secures your connection."),
                new QuizQuestion("Should passwords be shared over email?", new[] { "Yes", "Only with coworkers", "Never", "If encrypted" }, 'C', "Passwords should never be shared."),
                new QuizQuestion("How can you prevent identity theft?", new[] { "Ignore updates", "Use strong passwords", "Share details", "Post login info online" }, 'B', "Strong credentials help prevent identity theft."),
                new QuizQuestion("Is using the same password everywhere secure?", new[] { "Yes", "No", "Only at home", "If it’s strong" }, 'B', "One breach = all accounts vulnerable."),
                new QuizQuestion("What is multi-factor authentication?", new[] { "Using many passwords", "2+ verification steps", "Multiple logins", "None" }, 'B', "It adds extra login security."),
                new QuizQuestion("What should you do with suspicious emails?", new[] { "Open it", "Report or delete", "Reply", "Forward to friends" }, 'B', "Avoid engaging with them."),
                new QuizQuestion("Is HTTPS more secure than HTTP?", new[] { "Yes", "No", "Same", "Only at night" }, 'A', "HTTPS encrypts communication."),
                new QuizQuestion("What’s the risk of weak passwords?", new[] { "None", "Easier to guess", "Better for memory", "Safer" }, 'B', "Weak passwords can be easily cracked."),
                new QuizQuestion("What should you do after a data breach?", new[] { "Ignore it", "Change passwords", "Use the same password", "Tell no one" }, 'B', "Change credentials immediately.")
            };
        }

        // selecting 10 random
        public string StartQuiz()
        {
            _score = 0;
            _currentIndex = 0;
            _inProgress = true;

            // Shuffle and select 10 questions
            var rand = new Random();
            _quizQuestions = _allQuestions.OrderBy(q => rand.Next()).Take(10).ToList();

            return GetNextQuestion();
        }

        // Returns the next question or final score
        public string GetNextQuestion()
        {
            if (_currentIndex >= _quizQuestions.Count)
            {
                _inProgress = false;
                return $"🎓 Quiz complete! You scored {_score}/{_quizQuestions.Count}.";
            }

            var q = _quizQuestions[_currentIndex];
            return $"❓ Q{_currentIndex + 1}: {q.Question}\n" +
                   $"A) {q.Options[0]}\n" +
                   $"B) {q.Options[1]}\n" +
                   $"C) {q.Options[2]}\n" +
                   (q.Options.Length == 4 ? $"D) {q.Options[3]}\n" : "") +
                   "✍️ Please answer with A, B, C or D.";
        }

        // validates answer
        public string SubmitAnswer(char answer)
        {
            if (!_inProgress)
                return "❗ Quiz is not in progress.";

            var q = _quizQuestions[_currentIndex];
            string userAnswer = answer.ToString().Trim().ToUpper();
            string correctAnswer = q.CorrectAnswer.ToString().ToUpper();

            // Validate input
            if (!IsValidOption(userAnswer, q.Options.Length))
            {
                return $"⚠️ Invalid option. Please choose a valid answer (A{(q.Options.Length > 1 ? $", B" : "")}{(q.Options.Length > 2 ? ", C" : "")}{(q.Options.Length > 3 ? ", D" : "")}).";
            }

            // if correct
            bool isCorrect = userAnswer == correctAnswer;
            string feedback = isCorrect
                ? "✅ Correct!"
                : $"❌ Incorrect. {q.Explanation}";

            if (isCorrect) _score++;
            _currentIndex++;

            return feedback + "\n\n" + GetNextQuestion();
        }

        // Validates user answer
        private bool IsValidOption(string input, int numberOfOptions)
        {
            if (string.IsNullOrEmpty(input) || input.Length != 1)
                return false;

            char upper = input[0];
            return upper >= 'A' && upper < ('A' + numberOfOptions);
        }
    }


    public class QuizQuestion
    {
        public string Question { get; }
        public string[] Options { get; }
        public char CorrectAnswer { get; }
        public string Explanation { get; }

        public QuizQuestion(string question, string[] options, char correctAnswer, string explanation)
        {
            Question = question;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
        }
    }
}