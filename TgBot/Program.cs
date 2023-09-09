using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using TgBot;
using Telegram.Bot.Types.Enums;
using System.Collections.Specialized;
/*
 await botClient.SendTextMessageAsync(message.Chat.Id, "Начнем регистрацию!", replyMarkup: new ReplyKeyboardMarkup(new[]
                            {
                        new KeyboardButton("Я уже смешарик!!!"),
                        new KeyboardButton("Я новенький :)"),
                            })
                    { ResizeKeyboard = true });*/
namespace TelegramBotExperiments
{
    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("6112172788:AAGL9CO3FCM3HTomySA8-AvLaY3C5aMtzf0");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            KeyboardButton a = new KeyboardButton("Я уже смешарик!!!");
            KeyboardButton b = new KeyboardButton("Я новенький :)");
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                string msg = message.Text.ToLower();
                switch (msg) { 
                case "/start":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Привет!", replyMarkup: new ReplyKeyboardMarkup(new[]
                            {
                        new KeyboardButton("Я уже смешарик!!!"),
                        new KeyboardButton("Я новенький :)"),
                            })
                    { ResizeKeyboard = true });
                    break;
                case "я новенький :)":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Начнем регистрацию! Сколько тебе лет?", replyMarkup: new ReplyKeyboardRemove());
                    
                    break;
                case "я уже смешарик!!!":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Молодец! Найти тебе друга по учебе?", replyMarkup: new ReplyKeyboardMarkup(new[]
                            {
                        new KeyboardButton("Да!"),
                        new KeyboardButton("Нет, я соло воин 😎"),
                            })
                    { ResizeKeyboard = true });
                        Console.WriteLine(update.Message.Text.ToLower());
                        switch (update.Message.Text.ToLower()) {
                            case "да!":
                                long your_bro = FindBro.Find_SB();
                                break;
                            case "нет, я соло воин ??":
                                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ладно тогда, удачи тебе!");
                                break;
                            default:
                                break;
                        }
                        break;
                default:
                    await botClient.SendTextMessageAsync(message.Chat, "Прости... по запросу ничего не найдено. :(");
                    break;
                }
            }
        }
     

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}