using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TwitterTracker.Core;

namespace TelegramBot
{
    class Program
    {
        private static KeyValuePair<string, FrequencyItem> topItem = new KeyValuePair<string, FrequencyItem>("", null);
        private static ConcurrentDictionary<long, ChatId> chats = new ConcurrentDictionary<long, ChatId>();
        private static TelegramBotClient botClient = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramApiKey"]);

        private static Dictionary<string, FrequencyItem> items = null;

        static void Main(string[] args)
        {
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"ID={me.Id} User={me.FirstName}");

            botClient.OnMessage += HandleMessage;
            botClient.StartReceiving();

            foreach (var line in Run(args, ConsoleIn()))
            {
                Console.WriteLine(line);
            }
        }

        private static void HandleMessage(object sender, MessageEventArgs e)
        {
            if (chats.TryAdd(e.Message.Chat.Id, e.Message.Chat))
            {
                Console.WriteLine($"Received: {e.Message.Text}");
                botClient.SendTextMessageAsync(e.Message.Chat, "You will now be forwarded updates. (send: '/stop' to exit the message forwarding queue)");
            }
            else if (e.Message.Text.Contains("/stop"))
            {
                if (chats.TryRemove(e.Message.Chat.Id, out var _))
                    botClient.SendTextMessageAsync(e.Message.Chat, "You will no longer be forwarded updates.");
            }
        }

        private static IEnumerable<string> ConsoleIn()
        {
            while (true)
            {
                yield return Console.ReadLine();
            }
        }

        private static IEnumerable<string> Run(string[] args, IEnumerable<string> inputs)
        {
            if (items == null)
                items = new Dictionary<string, FrequencyItem>(1000);

            foreach (var input in inputs)
            {
                var msg = ProcessInput(input);
                if (msg != null)
                {
                    foreach (var chat in chats)
                    {
                        botClient.SendTextMessageAsync(chat.Value, msg.Value);
                    }
                    yield return $"Sent {msg.Value} to {chats.Count} clients.";
                }
            }
        }

        private static FrequencyItem ProcessInput(string input)
        {
            int min = 10;
            int max = 1000;

            var lastOutput = DateTime.MinValue;

            var split = input.Contains("#retweets=") ?
                input.Split(new[] { "#retweets=" }, StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray() :
                input.Split(',');
            var count = 0;
            var item = split.Length == 2 ? split[1] : "";
            var isItem = int.TryParse(split[0], out count) && item.StartsWith("http");
            if (isItem)
            {
                if (items.ContainsKey(item))
                    items[item].Seen(count);
                else
                {
                    if (items.Count >= max)
                        items.Remove(items.OrderByDescending(x => x.Value.Rank()).Last().Key);

                    items.Add(item, new FrequencyItem() { Value = item, Count = Math.Max(count, 1), LastSeen = DateTime.Now });
                }



                if (items.Count >= min)
                {
                    var newTop = items.OrderByDescending(x => x.Value.Rank()).FirstOrDefault();

                    if (newTop.Key != topItem.Key)
                    {
                        topItem = newTop;
                        return topItem.Value;
                    }
                    else
                        return null;
                }
                else
                    Console.WriteLine($"Threshold: {items.Count} < {min}");
            }

            return null;
        }
    }
}
