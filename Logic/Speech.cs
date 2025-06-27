// Matteo Nusca ST10440432
using System;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;

namespace St10440432CyberBotGUI
{
    public class Speech
    {
        private readonly SpeechSynthesizer speechSynthesizer;

        public Speech()
        {
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice(); // selects default audio device
            speechSynthesizer.Rate = 2; // speeds up the text-to-voice
        }

        // plays WAV greeting
        public void PlayVoiceGreeting()
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer("Resources/Greeting.wav")) // WAV path
                {
                    player.PlaySync(); // plays and waits
                }
            }
            catch (Exception ex) //if missing
            {
                Speak($"🔊 Voice greeting unavailable: {ex.Message}");
            }
        }

        // speaks message
        public void Speak(string message, ConsoleColor color = ConsoleColor.Green)
        {
            string cleanMessage = new string(message.Where(c => c <= 127).ToArray()); // removes emojis
            try
            {
                speechSynthesizer.SpeakAsyncCancelAll(); // interrupt current speech
                speechSynthesizer.SpeakAsync(cleanMessage); // speak new message
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Speech error: " + ex.Message);
            }

            Console.ForegroundColor = color;
            Console.WriteLine(message); // logs full version with emojis
            Console.ResetColor();
        }
    }
}
