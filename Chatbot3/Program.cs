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
                TypeEffect("\nAsk me a question or manage tasks (e.g., add task, view tasks): "+ "\n(Enter <bye>,<exit>,<quit> to exit)");
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
                        TypeEffect("Here are your tasks:"+"\n you can delete task or complete task by the task number.");
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

                if (!foundMatch)
                {
                    TypeEffect("I'm not sure I understand. Can you rephrase or ask something about cybersecurity?");
                }
            }
        }
    }
}
