using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Web;
using TgBot;
using Telegram.Bot.Types.Enums;
using System.Collections.Specialized;
using System.Text.Json.Nodes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;
using Telegram.Bot.Requests;
using Telegram.Bot.Extensions.Markup;
using Telegram.Bot.Types.InlineQueryResults;
using System.Diagnostics.Metrics;

namespace TelegramBotExperiments
{
    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient(
            "6112172788:AAGL9CO3FCM3HTomySA8-AvLaY3C5aMtzf0"
        );
        static bool isLoggedIn = false;
        static bool DelAcc = false;
        public static async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken
        )
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                string msg = message.Text.ToLower();
                switch (msg)
                {
                    case "/start":
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "Вы новый пользователь?",
                            replyMarkup: new ReplyKeyboardMarkup(
                                new[] { new KeyboardButton("Да"), new KeyboardButton("Нет"), }
                            )
                            {
                                ResizeKeyboard = true
                            }
                        );
                        break;
                    case "да":
                        if (DelAcc == false) { 
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Давай зарегистрируемся!",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                } else
                        {
                            DelAcc = false;
                            // delete acc
                            await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "Удаление пользователя из базы",
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                        }
                        //long your_bro = FindBro.Find_SB();
                        break;
                    case "нет":
                        isLoggedIn = true;
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "Основное меню",
                            replyMarkup: new ReplyKeyboardMarkup(
                                new[]
                                {
                                    new KeyboardButton("Найти компанию по кофейку"),
                                    new KeyboardButton("Календарь встреч"),
                                    new KeyboardButton("Профиль"),
                                    new KeyboardButton("Настройки и помощь"),
                                }
                            )
                            {
                                ResizeKeyboard = true
                            }
                        );
                        break;
                    case "найти компанию по кофейку":
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "В поиске временного друга :3",
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                        //long your_bro = FindBro.Find_SB();
                        break;
                    case "календарь встреч":
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "загружаемся..!",
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                        //long your_bro = FindBro.Calendar();
                        break;
                    case "профиль":
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "Профиль",
                            replyMarkup: new InlineKeyboardMarkup(
                                new[]
                                {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Сменить пароль",
                                        callbackData: "ChangePass"
                                    ),
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Изменить данные",
                                        callbackData: "ChangeData"
                                    ),
                                }
                            )
                        );
                        //long your_bro = FindBro.Find_SB();
                        break;
                    case "сообщить об ошибке":
                        //Report handler
                        break;
                    case "о боте":
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "бла бла бла...",
                            replyMarkup: new ReplyKeyboardMarkup(
                                new[]
                                {
                                    new KeyboardButton("Найти компанию по кофейку"),
                                    new KeyboardButton("Календарь встреч"),
                                    new KeyboardButton("Профиль"),
                                    new KeyboardButton("Настройки и помощь"),
                                }
                            )
                            {
                                ResizeKeyboard = true
                            }
                        );
                        break;
                    case "удалить аккаунт":
                        DelAcc = true;
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "Ваш аккаунт будет удален безвозвратно. Точно хотите продолжить?",
                            replyMarkup: new ReplyKeyboardMarkup(
                                new[]
                                {
                                    new KeyboardButton("Да"),
                                    new KeyboardButton("Нет"),
                                }
                            )
                            {
                                ResizeKeyboard = true
                            }
                        );
                        break;
                    case "настройки и помощь":
                        await botClient.SendTextMessageAsync(
                            message.Chat.Id,
                            "Чем помочь?",
                            replyMarkup: new ReplyKeyboardMarkup(
                                new[]
                                {
                                    new KeyboardButton("Переключить язык"),
                                    new KeyboardButton("Сообщить об ошибке"),
                                    new KeyboardButton("Переключить уведомления"),
                                    new KeyboardButton("О боте"),
                                    new KeyboardButton("Удалить аккаунт"),
                                }
                            )
                            {
                                ResizeKeyboard = true
                            }
                        );
                        //long your_bro = FindBro.Find_SB();
                        break;
                    default:
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            "Прости... по запросу ничего не найдено. :(",
                            replyMarkup: new ReplyKeyboardRemove()
                        );
                        break;
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                var results = new List<InlineQueryResult>();
                switch (update.CallbackQuery.Data)
                {
                    case "ChangePass":
                        //results.Add(new InlineQueryResultArticle("1", "Test2", new InputTextMessageContent("Test12"))
                        //);
                        //await bot.AnswerInlineQueryAsync(update.CallbackQuery.Id, results);
                        await botClient.EditMessageTextAsync(
                            update.Message.Chat.Id,
                            update.Message.MessageId,
                            "Смена пароля на: "
                        );
                        break;
                    case "ChangeData":
                        //results.Add(new InlineQueryResultArticle("0", "Test1", new InputTextMessageContent("Test"))
                        //);
                        await bot.AnswerInlineQueryAsync(update.InlineQuery.Id, results);
                        break;
                    default:
                        break;
                }
            }
        }
        public static async Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions { AllowedUpdates = { }, };
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
