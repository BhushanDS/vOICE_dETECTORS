using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using System.Xml.Linq;
using System.Windows.Forms;

namespace UC
{
    public partial class Form1 : Form
    {
        static SpeechRecognitionEngine _recognizer = null;
        static ManualResetEvent manualResetEvent = null;
        public Form1()
        {
            InitializeComponent();
            InitializeButton();
        }

        private void InitializeButton()
        {
            button1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            UncleSam();
        }
        static void UncleSam()
        {

            // Create an in-process speech recognizer for the en-US locale.  
            _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));

            var gb = new GrammarBuilder(getChoiceLibrary());
            var g = new Grammar(gb);
            _recognizer.LoadGrammar(g);

                // Add a handler for the speech recognized event.  
                _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

                // Configure input to the speech recognizer.  
                _recognizer.SetInputToDefaultAudioDevice();

                // Start asynchronous, continuous speech recognition.  
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            
        }

        public static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "hello Computer")
            {
                SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
                string userName = Environment.UserName;
                speechSynthesizer.Speak("Hello Buddy, How May I Help You");
                speechSynthesizer.Dispose();
            }
            else if (e.Result.Text == "Computer exit")
            {
                manualResetEvent.Set();
                Environment.Exit(0);
            }
            else
                processSpeech(e.Result.Text);
        }

        public static void processSpeech(string speechText)
        {

            XDocument doc = XDocument.Load("config.xml");
            foreach (var commandElement in doc.Descendants("command"))
            {
                string speechText1 = commandElement.Attribute("text")?.Value;
                string path = commandElement.Attribute("path")?.Value;
                string arguments = commandElement.Attribute("arguments")?.Value;
                Console.WriteLine(speechText);
                if (!string.IsNullOrEmpty(speechText1) && !string.IsNullOrEmpty(path) && speechText == speechText1)
                {
                    Console.WriteLine(speechText1);
                    ProcessCommand(speechText1, path, arguments);
                    break;
                }
            }
        }

        static void ProcessCommand(string speechText, string path, string arguments)
        {
            switch (speechText)
            {
                case "Computer exit":
                    // Special case for exit command
                    Environment.Exit(0);
                    break;
                default:
                    // Start the process with the specified path and arguments
                    if (!string.IsNullOrEmpty(arguments) && arguments != "Close")
                    {
                        Process.Start(path, arguments);
                    }
                    else
                        Process.Start(path);
                    break;
            }
        }

        public static Choices getChoiceLibrary()
        {
            Choices myChoices = new Choices();

            try
            {
                XDocument doc = XDocument.Load("config.xml");

                foreach (var choiceElement in doc.Descendants("choice"))
                {
                    string choiceText = choiceElement.Value;
                    myChoices.Add(choiceText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading choices from XML: {ex.Message}");
            }

            return myChoices;
        }
        static void _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "hello computer")
            {
                SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.Speak("hello user");
                speechSynthesizer.Dispose();
            }
            //manualResetEvent.Set();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("This application is created by BRANE \nAgent Jack 1.0.0.0 .", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
