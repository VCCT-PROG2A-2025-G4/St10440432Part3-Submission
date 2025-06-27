// Matteo Nusca ST10440432
// DECLARATION OF AI
// AI used for keyword generation & structure

using System;
using System.Collections.Generic;
using System.Linq;

namespace St10440432CyberBotGUI
{
    public class Bot
    {
        private readonly Speech _speech; // text-to-speech 
        private string _lastTopic = null; // last discussed topic
        private string _userInterest = null; // stored user interest
        private readonly Random _random = new Random(); // randomizer
        private readonly List<string> _activityLog = new(); // log list
        private int _logDisplayIndex = 0; // log page index

        public TaskManager TaskManager { get; set; } // task handler
        public QuizManager QuizManager { get; set; } // quiz handler

        // constructor
        public Bot(Speech speech)
        {
            _speech = speech;
            TaskManager = new TaskManager();
            QuizManager = new QuizManager();
        }

        // main logic
        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "❗ Please type something.";

            input = input.ToLower().Trim();

            // quiz handling
            if (QuizManager.IsInProgress)
            {
                if (input.Length == 1 && "abcd".Contains(input[0]))
                {
                    Log($"📝 Quiz answer submitted: {input.ToUpper()}");
                    string result = QuizManager.SubmitAnswer(char.ToUpper(input[0]));

                    if (!QuizManager.IsInProgress)
                        Log("✅ Quiz completed.");

                    return result;
                }
                else
                {
                    return "⚠️ Please answer the quiz with A, B, C, or D only.";
                }
            }

            // interest
            if (input.Contains("i'm interested in") || input.Contains("i am interested in"))
            {
                var interest = input.Replace("i'm interested in", "").Replace("i am interested in", "").Trim();
                _userInterest = interest;
                Log($"🧠 User interest remembered: '{interest}'");
                _lastTopic = null;
                return $"✨ Great! I'll remember that you're interested in {interest}. It's an important cybersecurity topic.";
            }

            // activity log request
            if (input.Contains("activity log") || input.Contains("what have you done") ||
                input.Contains("show log") || input.Contains("show activity log") ||
                input.Contains("what have you done for me"))
            {
                _logDisplayIndex = 0;
                _lastTopic = null;
                return GetActivityLog();
            }

            // log extension
            if (input.Contains("show more"))
            {
                _lastTopic = null;
                return GetActivityLog();
            }

            // quiz trigger
            if (input.Contains("quiz") || input.Contains("start quiz") || input.Contains("take quiz"))
            {
                Log("🎮 Quiz started by user.");
                _lastTopic = null;
                return QuizManager.StartQuiz();
            }

            // task input
            if (input.Contains("remind") || input.Contains("task") || input.Contains("add") || input.Contains("set reminder"))
            {
                string result = TaskManager.HandleTaskInput(input, out TaskItem newTask);

                if (newTask != null)
                {
                    string reminderInfo = newTask.ReminderDate.HasValue
                        ? $" (Reminder: {newTask.ReminderDate.Value:dd MMM yyyy})"
                        : "";
                    Log($"🗒️ Task created: {newTask.Title}{reminderInfo}");
                }
                else
                {
                    Log("⚠️ Task input was attempted but could not be parsed.");
                }

                _lastTopic = null;
                return result;
            }

            // follow-up topic
            if (_followUpTriggers.Any(trigger => input.Contains(trigger)) && _lastTopic != null)
            {
                foreach (var entry in _responses)
                {
                    if (entry.Key.Any(k => _lastTopic.Contains(k)))
                    {
                        string response = entry.Value[_random.Next(entry.Value.Count)];
                        return "🔁 Here's more on that:\n" + response;
                    }
                }
            }

            // keyword detection
            foreach (var entry in _responses)
            {
                foreach (var keyword in entry.Key)
                {
                    if (input.Contains(keyword))
                    {
                        string sentiment = GetSentimentPrefix(input);
                        string reply = entry.Value[_random.Next(entry.Value.Count)];
                        _lastTopic = keyword;
                        Log($"📌 NLP topic matched: '{keyword}'");

                        string extra = (!string.IsNullOrWhiteSpace(_userInterest) && !keyword.Contains(_userInterest))
                            ? $"\n🧠 As someone interested in {_userInterest}, you might also find this useful."
                            : "";

                        return sentiment + reply + extra;
                    }
                }
            }

            _lastTopic = null;
            return "🤔 I'm not sure how to respond to that. Try asking about a cybersecurity topic!";
        }

        // add to activity log
        private void Log(string description)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            _activityLog.Add($"[{timestamp}] {description}");
        }

        // display log with pagination
        private string GetActivityLog()
        {
            const int entriesPerPage = 5;

            if (_activityLog.Count == 0)
                return "📭 No activity recorded yet.";

            var entries = _activityLog.Skip(_logDisplayIndex).Take(entriesPerPage).ToList();

            if (entries.Count == 0)
                return "🔚 No more entries to show.";

            _logDisplayIndex += entries.Count;

            string result = "🧾 Activity Log:\n" + string.Join("\n", entries);

            if (_logDisplayIndex < _activityLog.Count)
            {
                result += "\n\n📄 Type 'show more' to view more activity...";
            }

            return result;
        }

        // sentiment detection
        private string GetSentimentPrefix(string input)
        {
            input = input.ToLower();
            var positiveIntros = new List<string>
            {
                "😊 I'm happy to help! ",
                "👍 Great question! ",
                "🌟 Awesome! ",
                "😄 Glad you're curious! ",
                "💡 Let's dive in! "
            };
            var negativeIntros = new List<string>
            {
                "😟 I understand your concern. ",
                "🙏 Don't worry, I'll explain: ",
                "😥 It can be tricky, but here's help: ",
                "🛡️ Let me make this simple: ",
                "🔍 Here's a clearer explanation: "
            };

            foreach (string word in _positiveWords)
                if (input.Contains(word)) return positiveIntros[_random.Next(positiveIntros.Count)];

            foreach (string word in _negativeWords)
                if (input.Contains(word)) return negativeIntros[_random.Next(negativeIntros.Count)];

            return "";
        }

        // sentiment keyword lists
        private readonly string[] _positiveWords =
        {
            "secure", "protected", "safe", "confident", "reassured", "understand", "clear", "helpful",
            "great", "good", "trusted", "reliable", "encrypted", "successful", "aware", "educated",
            "prepared", "empowered", "satisfied", "peaceful", "calm", "comfortable", "assured",
            "happy", "relieved", "alert", "mindful", "smooth", "organized"
        };

        private readonly string[] _negativeWords =
        {
            "vulnerable", "worried", "confused", "scared", "anxious", "concerned", "fearful", "doubtful",
            "unsure", "lost", "overwhelmed", "stressed", "frustrated", "suspicious", "unprotected",
            "exposed", "threatened", "helpless", "alarmed", "nervous", "panicked", "insecure",
            "risk", "attack", "breach", "compromised", "uncertain", "unsafe"
        };

        // follow-up trigger phrases
        private readonly List<string> _followUpTriggers = new()
        {
            "tell me more", "what else", "continue", "go on", "more info", "elaborate",
            "explain further", "say more", "expand", "details", "additional info", "keep going",
            "clarify", "more details", "please explain", "dig deeper", "examples", "how does it work",
            "steps", "procedure", "recommendations", "advice"
        };

        private readonly Dictionary<List<string>, List<string>> _responses = new()
        {
            {
                new List<string> { "how are you", "how’s it going", "you okay", "status", "how do you feel", "bot status" },
                new List<string>
                {
                    "🤖 I'm running at full capacity, ready to protect and inform!",
                    "⚙️ All systems are green. Let’s keep your data safe!",
                    "😊 Operational and happy to help with your cybersecurity questions.",
                    "🧠 Running smoothly with zero bugs today."
                }
            },
            {
                new List<string> { "goodbye", "bye", "exit", "quit", "leave", "close", "shutdown" },
                new List<string>
                {
                    "👋 Goodbye! Stay cyber-safe out there.",
                    "🔒 Logging off... remember to update those passwords!",
                    "🛡️ See you next time. Keep your software patched!",
                    "🌐 Until next time — keep learning and stay aware!"
                }
            },
            {
                new List<string> { "purpose", "what do you do", "bot mission", "what’s your goal", "why were you made" },
                new List<string>
                {
                    "🎯 I'm here to help you understand and defend against cybersecurity threats.",
                    "🛡️ My mission is to educate you on safe digital habits.",
                    "📘 Think of me as your personal cybersecurity assistant.",
                    "🔐 I simulate threats, explain dangers, and provide tips to stay secure."
                }
            },
            {
                new List<string> { "phishing", "email scam", "fake email", "spoofing", "fraudulent email" },
                new List<string>
                {
                    "🎣 Phishing tricks you into sharing personal info using fake emails or sites.",
                    "📧 Watch out for urgent or suspicious emails from unknown sources.",
                    "🔗 Hover over links to see the real URL before clicking!",
                    "🚨 Never share passwords or info via email — companies never ask like that."
                }
            },
            {
                new List<string> { "ransomware", "encrypted files", "ransom malware", "file lock virus", "ransom attack" },
                new List<string>
                {
                    "💣 Ransomware locks your files and demands money to unlock them.",
                    "🧠 Avoid ransomware by not opening unknown attachments and keeping backups.",
                    "🛡️ Never pay the ransom. It encourages more attacks and doesn't guarantee recovery.",
                    "💾 Backup your data regularly so you’re not held hostage!"
                }
            },
            {
                new List<string> { "password", "strong password", "password safety", "password tip", "login credentials" },
                new List<string>
                {
                    "🔑 Use unique passwords with symbols, numbers, and upper/lowercase letters.",
                    "🧠 Don’t reuse passwords across accounts — use a manager like Bitwarden or LastPass.",
                    "🛡️ Enable 2FA on all accounts to prevent password-only access.",
                    "🔁 Change your passwords regularly and after any breach."
                }
            },
            {
                new List<string> { "vpn", "virtual private network", "vpn safety", "vpn secure", "online privacy vpn" },
                new List<string>
                {
                    "🔐 VPNs hide your IP and encrypt traffic — useful on public Wi-Fi!",
                    "🌍 Great for bypassing censorship and adding a privacy layer.",
                    "📱 Choose a trusted VPN with a no-logs policy.",
                    "🛡️ Use a VPN when banking, traveling, or browsing on public hotspots."
                }
            },
            {
                new List<string> { "firewall", "internet firewall", "network firewall", "firewall settings", "firewall security" },
                new List<string>
                {
                    "🧱 Firewalls act as digital walls, blocking unauthorized access.",
                    "⚙️ Keep your firewall enabled on Windows/Mac to protect your network.",
                    "🔐 Advanced firewalls monitor both incoming and outgoing traffic.",
                    "🛡️ Routers and business systems often include hardware firewalls too."
                }
            },
            {
                new List<string> { "wifi", "wifi safety", "secure wifi", "public wifi", "wifi protection", "wifi password" },
                new List<string>
                {
                    "📶 Avoid using sensitive accounts on public Wi-Fi — it's easy to intercept.",
                    "🔒 Use a VPN when connected to coffee shop or airport Wi-Fi.",
                    "📡 At home, change your router password and use WPA3 encryption.",
                    "🔐 Don’t share your Wi-Fi password with guests — use a guest network!"
                }
            },
            {
                new List<string> { "antivirus", "virus", "trojan", "malware", "spyware", "computer virus", "adware" },
                new List<string>
                {
                    "🦠 Antivirus software scans for and removes malicious programs.",
                    "📈 Keep antivirus definitions updated to catch the newest threats.",
                    "🛡️ Use real-time protection to catch viruses before they act.",
                    "🚨 Beware of fake antivirus popups — those are often malware themselves!"
                }
            },
            {
                new List<string> { "updates", "patch", "software update", "security patch", "update system" },
                new List<string>
                {
                    "🛠️ Updates fix bugs and close security holes — don’t delay them!",
                    "🔄 Enable auto-updates so you’re protected without thinking about it.",
                    "📅 Patch Tuesday is when Microsoft releases major updates. Stay on top!",
                    "⚠️ Hackers exploit outdated software — always update your apps."
                }
            },
            {
                new List<string> { "social media", "facebook", "instagram", "tiktok", "social media privacy", "safe social" },
                new List<string>
                {
                    "📱 Keep your profiles private — oversharing helps attackers.",
                    "👻 Think before you post — even deleted content can be archived.",
                    "🧠 Avoid clicking strange DMs or friend requests — phishing is common.",
                    "🔒 Use strong passwords and 2FA on all social accounts."
                }
            },
            {
                new List<string> { "cloud", "google drive", "dropbox", "cloud safety", "cloud storage" },
                new List<string>
                {
                    "☁️ Encrypt files before uploading to cloud storage.",
                    "🔐 Use strong passwords and enable 2FA on cloud services.",
                    "📁 Review shared files — don’t leave private docs open to the public.",
                    "💾 Back up important cloud files offline in case of outage or loss."
                }
            },
            {
                new List<string> { "cybersecurity", "online safety", "internet security", "digital safety", "cyber threats" },
                new List<string>
                {
                    "🛡️ Cybersecurity is about protecting systems from digital threats.",
                    "💻 Use antivirus, firewalls, and good habits to reduce your risk.",
                    "📚 Stay informed — social engineering and phishing evolve constantly.",
                    "🔐 Even strong passwords aren’t enough — layer your defenses."
                }
            },
            {
                new List<string> { "career", "cyber job", "security career", "how to learn cybersecurity", "learn cyber" },
                new List<string>
                {
                    "🚀 Cybersecurity is booming — learn networking, Linux, and hacking basics.",
                    "📘 Start with free courses like Cybrary, TryHackMe, or Google’s certs.",
                    "🧠 Certifications like Security+, CEH, and CISSP help land jobs.",
                    "💼 Real-world practice in labs and bug bounties builds your skill."
                }
            },
        };
    }
}


