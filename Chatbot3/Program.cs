using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;

namespace Chatbot3
{
    internal class Program
    {
        static string userName = "";
        static string userInterest = "";

        class TaskItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? ReminderDate { get; set; }
            public bool Completed { get; set; } = false;
        }

        static List<TaskItem> tasks = new List<TaskItem>();

        static Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>
        {
            { "password", new List<string> { "Use strong, unique passwords and enable two-factor authentication.",
                                             "Avoid using personal information in your passwords.",
                                             "Consider using a password manager to keep track of your credentials." } },
            { "scam", new List<string> { "Be cautious of unsolicited emails requesting personal info.",
                                             "Verify the source before clicking on suspicious links." } },
            { "phishing", new List<string> { "Be cautious of emails asking for personal information.",
                                             "Scammers may pose as trusted organizations—always verify links.",
                                             "Do not click on suspicious links or attachments." } },
            { "privacy", new List<string> { "Adjust your social media privacy settings for better protection.",
                                           "Limit the amount of personal information you share online.",
                                           "Use secure and encrypted messaging apps." } },
        };

        static Dictionary<string, string> sentimentResponses = new Dictionary<string, string>
        {
            { "worried", "It's completely understandable to feel that way, {0}. Let me share some tips to help you stay safe." },
            { "curious", "Curiosity is great, {0}! Let's dive into the topic together." },
            { "frustrated", "Don't worry, {0}—these concepts can be tricky. I'm here to help!" }
        };

        static List<(string Question, string[] Options, string Answer, string Explanation)> quizQuestions = new List<(string, string[], string, string)>
{
    ("What should you do if you receive an email asking for your password?",
        new[] { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" }, "C",
        "Correct! Reporting phishing emails helps prevent scams."),

    ("True or False: It's safe to use the same password for multiple accounts.",
        new[] { "A) True", "B) False" }, "B",
        "Correct! Reusing passwords increases your risk if one account is compromised."),

    ("Which of the following is a sign of a phishing attempt?",
        new[] { "A) Email from a friend", "B) Unexpected attachment", "C) Website with HTTPS", "D) Newsletter subscription" }, "B",
        "Correct! Unexpected attachments can carry malware."),

    ("What's the best way to create a strong password?",
        new[] { "A) Use your birthday", "B) Use 'password123'", "C) Use a mix of letters, numbers, and symbols", "D) Use your pet’s name" }, "C",
        "Correct! Strong passwords use a mix of characters and are hard to guess."),

    ("True or False: Public Wi-Fi is always safe to use for online banking.",
        new[] { "A) True", "B) False" }, "B",
        "Correct! Public Wi-Fi is not secure for sensitive activities."),

    ("Which of the following protects your device from viruses?",
        new[] { "A) Using a VPN", "B) Antivirus software", "C) Closing unused apps", "D) Bright screen" }, "B",
        "Correct! Antivirus software helps detect and remove threats."),

    ("What is two-factor authentication (2FA)?",
        new[] { "A) A second email", "B) A password reset", "C) A verification step after your password", "D) Using two passwords" }, "C",
        "Correct! 2FA adds an extra step for more secure logins."),

    ("True or False: It's okay to click on any link sent by your friend.",
        new[] { "A) True", "B) False" }, "B",
        "Correct! Their account could be compromised and used to send malicious links."),

    ("What does HTTPS mean?",
        new[] { "A) It's a government website", "B) It’s a secure connection", "C) It’s a social media site", "D) It’s a spam site" }, "B",
        "Correct! HTTPS encrypts the data between you and the website."),

    ("Why should you keep your software updated?",
        new[] { "A) To make it look nice", "B) To remove files", "C) To fix security bugs", "D) To increase file size" }, "C",
        "Correct! Updates often include patches for security vulnerabilities.")
};
        
        static Dictionary<string, List<string>> nlpKeywords = new Dictionary<string, List<string>>
{
    { "add_task", new List<string> { "add task", "create task", "new task", "add a task", "set a task" } },
    { "reminder", new List<string> { "remind me", "set reminder", "add reminder" } },
    { "quiz", new List<string> { "start quiz", "cybersecurity quiz", "play quiz", "quiz" } },
    { "phishing", new List<string> { "phishing", "phishing email", "phishing scam" } },
    { "password", new List<string> { "password", "update password", "change password" } },
};

        static void Main(string[] args)
        {
            SoundPlayer player = new SoundPlayer(@"C:\Users\lab_services_student\Desktop\PROG6221_POE_ST10451904\Chatbot3\08pgn-1x0pn.wav");
            player.PlaySync();

            TypeEffect("Starting Cybersecurity Awareness Bot...");
            DisplayAsciiArt();
            TextGreeting();
            ResponseSystem();
        }

        static void TypeEffect(string text, int delay = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
        static bool MatchesKeyword(string input, List<string> keywordVariants)
        {
            foreach (var keyword in keywordVariants)
            {
                if (input.Contains(keyword))
                {
                    return true;
                }
            }
            return false;
        }

        static void DisplayAsciiArt()
        {
            Console.WriteLine(@" 
  ____      _                                        _ _            
 / ___|   _| |__   ___ _ __ ___  ___  ___ _   _ _ __(_) |_ _   _    
| |  | | | | '_ \ / _ \ '__/ __|/ _ \/ __| | | | '__| | __| | | |   
| |__| |_| | |_) |  __/ |  \__ \  __/ (__| |_| | |  | | |_| |_| |   
 \____\__, |_.__/ \___|_|  |___/\___|\___|\__,_|_|  |_|\__|\__, |   
   / \|___/   ____ _ _ __ ___ _ __   ___  ___ ___  | __ )  |___/ |_ 
  / _ \ \ /\ / / _` | '__/ _ \ '_ \ / _ \/ __/ __| |  _ \ / _ \| __|
 / ___ \ V  V / (_| | | |  __/ | | |  __/\__ \__ \ | |_) | (_) | |_ 
/_/   \_\_/\_/ \__,_|_|  \___|_| |_|\___||___/___/ |____/ \___/ \__|
");
        }

        static void TextGreeting()
        {
            TypeEffect("\nWhat's your name? ");
            userName = Console.ReadLine();
            TypeEffect($"\nWelcome, {userName}! Let's explore how to stay safe online together.");
        }

        static void ResponseSystem()
        {
            Random rnd = new Random();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                TypeEffect("\nAsk me a question or do the followings..." + "\n<Add task><View tasks><Delete task><Complete task> " + "\n<quiz><show actions><summary>" + "\nEnter <bye>,<exit>,<quit> to exit");
                string input = Console.ReadLine()?.ToLower();
                Console.ResetColor();

                if (string.IsNullOrWhiteSpace(input))
                {
                    TypeEffect("I didn't quite understand that. Could you rephrase?" + "\n(Enter <bye>,<exit>,<quit> to exit)");
                    continue;
                }

                if (input.Contains("exit") || input.Contains("quit") || input.Contains("bye"))
                {
                    TypeEffect($"Thank you for chatting, {userName}! Stay safe online.");
                    break;
                }

                bool foundMatch = false;

                // Keyword responses
                foreach (var keyword in keywordResponses.Keys)
                {
                    if (input.Contains(keyword))
                    {
                        string response = keywordResponses[keyword][rnd.Next(keywordResponses[keyword].Count)];
                        TypeEffect(response);
                        userInterest = keyword;
                        foundMatch = true;
                    }
                }

                // Sentiment detection
                foreach (var sentiment in sentimentResponses.Keys)
                {
                    if (input.Contains(sentiment))
                    {
                        TypeEffect(string.Format(sentimentResponses[sentiment], userName));
                        foundMatch = true;
                    }
                }

                // Name recall
                if (input.Contains("my name"))
                {
                    TypeEffect($"Your name is {userName}, of course!");
                    foundMatch = true;
                }

                // Memory-based follow-up
                if (input.Contains("tell me more") && !string.IsNullOrWhiteSpace(userInterest))
                {
                    TypeEffect($"{userName}, since you're interested in {userInterest}, here's a tip: Always update your apps to patch security vulnerabilities.");
                    foundMatch = true;
                }

                // Task management
                if (input.StartsWith("add task"))
                {
                    TypeEffect("Please enter the task title:");
                    string title = Console.ReadLine();

                    TypeEffect("Enter a description for the task:");
                    string desc = Console.ReadLine();

                    TypeEffect("Would you like to set a reminder? (yes/no)");
                    string remindAns = Console.ReadLine()?.ToLower();

                    DateTime? reminderDate = null;
                    if (remindAns == "yes")
                    {
                        TypeEffect("Enter number of days from now for the reminder:");
                        if (int.TryParse(Console.ReadLine(), out int days))
                        {
                            reminderDate = DateTime.Now.AddDays(days);
                        }
                    }

                    tasks.Add(new TaskItem { Title = title, Description = desc, ReminderDate = reminderDate });
                    TypeEffect($"Task added: \"{title}\". Reminder: {(reminderDate.HasValue ? reminderDate.Value.ToShortDateString() : "None")}");
                    foundMatch = true;
                }
                else if (input.Contains("view tasks"))
                {
                    if (tasks.Count == 0)
                    {
                        TypeEffect("You have no tasks yet.");
                    }
                    else
                    {
                        TypeEffect("Here are your tasks:" + "\n you can delete task or complete task by the task number.");
                        int index = 1;
                        foreach (var task in tasks)
                        {
                            string status = task.Completed ? "[Completed]" : "[Pending]";
                            string reminder = task.ReminderDate.HasValue ? $" (Reminder on {task.ReminderDate.Value.ToShortDateString()})" : "";
                            TypeEffect($"{index++}. {task.Title}: {task.Description} {status}{reminder}");
                        }
                    }
                    foundMatch = true;
                }
                else if (input.Contains("delete task"))
                {
                    TypeEffect("Enter the number of the task to delete:");
                    if (int.TryParse(Console.ReadLine(), out int delIndex) && delIndex <= tasks.Count && delIndex > 0)
                    {
                        TypeEffect($"Task \"{tasks[delIndex - 1].Title}\" deleted.");
                        tasks.RemoveAt(delIndex - 1);
                    }
                    else
                    {
                        TypeEffect("Invalid task number.");
                    }
                    foundMatch = true;
                }
                else if (input.Contains("complete task"))
                {
                    TypeEffect("Enter the number of the task to mark as complete:");
                    if (int.TryParse(Console.ReadLine(), out int compIndex) && compIndex <= tasks.Count && compIndex > 0)
                    {
                        tasks[compIndex - 1].Completed = true;
                        TypeEffect($"Task \"{tasks[compIndex - 1].Title}\" marked as complete.");
                    }
                    else
                    {
                        TypeEffect("Invalid task number.");
                    }
                    foundMatch = true;
                }

                //cybersecurity quiz
                if (input.Contains("quiz") || input.Contains("cybersecurity quiz"))
                {
                    StartCybersecurityQuiz();
                    foundMatch = true;
                }

                // NLP Simulated Detection for Adding Task
                if (MatchesKeyword(input, nlpKeywords["add_task"]) || MatchesKeyword(input, nlpKeywords["reminder"]))
                {
                    string extractedTask = "";

                    // Simple extraction: if user says "remind me to ..." or "add a task to ..."
                    if (input.StartsWith("remind me to "))
                    {
                        extractedTask = input.Substring("remind me to ".Length);
                    }
                    else if (input.StartsWith("add a task to "))
                    {
                        extractedTask = input.Substring("add a task to ".Length);
                        TypeEffect("Task added.");
                    }
                    else if (input.StartsWith("set reminder to "))
                    {
                        extractedTask = input.Substring("set reminder to ".Length);
                    }

                    // If no extraction, ask user
                    if (string.IsNullOrWhiteSpace(extractedTask))
                    {
                        TypeEffect("Please enter the task title:");
                        extractedTask = Console.ReadLine();
                    }

                    TypeEffect("Would you like to set a reminder date? (yes/no)");
                    string remindAns = Console.ReadLine()?.ToLower();

                    DateTime? reminderDate = null;
                    if (remindAns == "yes")
                    {
                        TypeEffect("Enter number of days from now for the reminder:");
                        if (int.TryParse(Console.ReadLine(), out int days))
                        {
                            reminderDate = DateTime.Now.AddDays(days);
                        }
                    }

                    tasks.Add(new TaskItem { Title = extractedTask, Description = "Added via NLP command", ReminderDate = reminderDate });
                    TypeEffect($"Task added: \"{extractedTask}\". Reminder: {(reminderDate.HasValue ? reminderDate.Value.ToShortDateString() : "None")}");
                    foundMatch = true;
                }

                // NLP for Quiz
                else if (MatchesKeyword(input, nlpKeywords["quiz"]))
                {
                    StartCybersecurityQuiz();
                    foundMatch = true;
                }

                // NLP Example for Phishing or Password keyword advice
                else if (MatchesKeyword(input, nlpKeywords["phishing"]) || MatchesKeyword(input, nlpKeywords["password"]))
                {
                    Random rnd1 = new Random();
                    string key = MatchesKeyword(input, nlpKeywords["phishing"]) ? "phishing" : "password";
                    string response = keywordResponses[key][rnd1.Next(keywordResponses[key].Count)];
                    TypeEffect(response);
                    userInterest = key;
                    foundMatch = true;
                }

                else if (input.Contains("what have you done") || input.Contains("show actions") || input.Contains("summary"))
                {
                    if (tasks.Count == 0)
                    {
                        TypeEffect("You have no recent actions.");
                    }
                    else
                    {
                        TypeEffect("Here's a summary of recent actions:");
                        int index = 1;
                        foreach (var task in tasks)
                        {
                            string reminder = task.ReminderDate.HasValue ? $" on {task.ReminderDate.Value.ToShortDateString()}" : "(no reminder set)";
                            TypeEffect($"{index++}. Task: \"{task.Title}\" {reminder}");
                        }
                    }
                    foundMatch = true;
                }
                if (!foundMatch)
                {
                    TypeEffect("I'm not sure I understand. Can you rephrase or ask something about cybersecurity?");


                }
            }

            static void StartCybersecurityQuiz()
            {
                int score = 0;

                TypeEffect("\n~~~Let's begin the Cybersecurity Quiz!~~~");
                Thread.Sleep(500);

                for (int i = 0; i < quizQuestions.Count; i++)
                {
                    var (question, options, correctAnswer, explanation) = quizQuestions[i];

                    TypeEffect($"\nQuestion {i + 1}: {question}");
                    foreach (var option in options)
                    {
                        Console.WriteLine(option);
                    }

                    Console.Write("Your answer (A/B/C/D): ");
                    string userAnswer = Console.ReadLine()?.Trim().ToUpper();

                    if (userAnswer == correctAnswer)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        TypeEffect("Correct! " + explanation);
                        score++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        TypeEffect($"Incorrect. The correct answer is {correctAnswer}. {explanation}");
                    }
                    Console.ResetColor();
                    Thread.Sleep(500);
                }

                TypeEffect($"\nYou scored {score} out of {quizQuestions.Count}!");

                if (score >= 8)
                    TypeEffect("Excellent! You're a cybersecurity pro!");
                else if (score >= 5)
                    TypeEffect("Good effort! Keep learning to stay even safer online.");
                else
                    TypeEffect("Keep practicing. The more you know, the safer you'll be!");
            }

        }
    }
}
//Answer for CybersecurityQuiz (use for quickly test the code) is CBBCBBCBBC.