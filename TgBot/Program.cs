using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace TelegramBotExperiments
{
    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient(
            "6112172788:AAGL9CO3FCM3HTomySA8-AvLaY3C5aMtzf0"
        );
        public static bool DelAcc = false;
        public static bool StartedReg = false;
        public static int count = 1;
        public static string changing = "None";
        public static Userx newuser = new Userx();
        public static async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken
        )
        {
            //return; //fix
            bool isLoggedIn = false;
            ApplicationContext db = new ApplicationContext();
            Userx tempuser = new Userx();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == UpdateType.Message)
            {
                Console.WriteLine(update.Message.Chat.Username);
                if (update.Message.Chat.Username != null && checkreg(update.Message.Chat.Username))
                {
                    isLoggedIn = true;
                    var users = db.Users.ToList();
                    foreach (Userx u in users)
                    {
                        if (u.Id == update.Message.Chat.Username)
                        {
                            tempuser = u;
                            tempuser.InContact = "None";
                            tempuser.InQueue = false;
                            tempuser.ChatId = update.Message.Chat.Id;
                            db.SaveChanges();
                            break;
                        }
                    }
                }
                else
                newuser.Id = update.Message.Chat.Username;
                var message = update.Message;
                string msg = message.Text.ToLower();
                if (msg == "/start")
                {
                    StartedReg = false;
                    count = 1;
                }
                if (StartedReg == false && changing == "None")
                {
                    switch (msg)
                    {
                        case "/start":
                            if (isLoggedIn == false)
                            {
                                await botClient.SendPhotoAsync(
                                chatId: message.Chat.Id,
                                photo: InputFile.FromUri("https://raw.githubusercontent.com/seku345/RandomCoffeeTelegramBot/main/TgBot/im/12.jpg"),
                                caption: "Давай пройдем регистрацию!",
                                parseMode: ParseMode.Html,
                                replyMarkup: new InlineKeyboardMarkup(
                                        new[]
                                        {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Давай!",
                                        callbackData: "Register"
                                    ),
                                        }
                                    ),
                                cancellationToken: cancellationToken);
                            }
                            else
                            {
                                Console.WriteLine(isLoggedIn);
                                await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Основное меню",
                                replyMarkup: new ReplyKeyboardMarkup(
                                    new[]
                                    {
                                    new KeyboardButton[] {"Найти компанию по кофейку", "Календарь встреч" },
                                    new KeyboardButton[] {"Профиль","Настройки и помощь" },
                                    }
                                )
                                {
                                    ResizeKeyboard = true
                                }
                            );
                            }
                            break;
                        case "да":
                            if (DelAcc == true && message.Chat.Username!=null)
                            {
                                await DeleteUser(tempuser);
                                DelAcc = false;
                                await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Удаление данных о пользователе из базы",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                            }
                            break;
                        case "нет":
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Основное меню",
                                replyMarkup: new ReplyKeyboardMarkup(
                                    new[]
                                    {
                                    new KeyboardButton[] {"Найти компанию по кофейку", "Календарь встреч" },
                                    new KeyboardButton[] {"Профиль","Настройки и помощь" },
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
                            waitforbuddy(tempuser, botClient, db, message);
                            break;
                        case "календарь встреч":
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "В разработке..!",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                            //calendar
                            break;
                        case "профиль":
                            /*await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                $"Профиль\nИмя: {tempuser.Name}\nФамилия: {tempuser.SurName}\nГруппа: {tempuser.Group}\nПол: {tempuser.Sex}\nВозраст: {tempuser.Age}",
                                replyMarkup: new InlineKeyboardMarkup(
                                    new[]
                                    {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Покинуть поиск",
                                        callbackData: "LeaveQueue"
                                    ),
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Изменить данные",
                                        callbackData: "ChangeData"
                                    ),
                                    }
                                )
                            );*/
                            await botClient.SendPhotoAsync(
                                chatId: message.Chat.Id,
                                photo: InputFile.FromUri("https://raw.githubusercontent.com/seku345/RandomCoffeeTelegramBot/main/TgBot/im/profil.png"),
                                caption: $"Имя: { tempuser.Name}\nФамилия: { tempuser.SurName}\nГруппа: { tempuser.Group}\nПол: { tempuser.Sex}\nВозраст: { tempuser.Age}",
                                parseMode: ParseMode.Html,
                                replyMarkup: new InlineKeyboardMarkup(
                                    new[]
                                    {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Изменить данные",
                                        callbackData: "ChangeData"
                                    ),
                                    }
                                ),
                                cancellationToken: cancellationToken);
                            //...
                            break;
                        case "сообщить об ошибке":
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Напишите @prunovxd насчёт ошибки.",
                                replyMarkup: new ReplyKeyboardMarkup(
                                    new[]
                                    {
                                    new KeyboardButton[] {"Найти компанию по кофейку", "Календарь встреч" },
                                    new KeyboardButton[] {"Профиль","Настройки и помощь" },
                                    }
                                )
                                {
                                    ResizeKeyboard = true
                                }
                            );
                            break;
                        case "основное меню":
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Открываем...",
                                replyMarkup: new ReplyKeyboardMarkup(
                                    new[]
                                    {
                                    new KeyboardButton[] {"Найти компанию по кофейку", "Календарь встреч" },
                                    new KeyboardButton[] {"Профиль","Настройки и помощь" },
                                    }
                                )
                                {
                                    ResizeKeyboard = true
                                }
                            );
                            break;
                        case "о боте":
                            /*await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "бла бла бла...",
                                replyMarkup: new ReplyKeyboardMarkup(
                                    new[]
                                    {
                                    new KeyboardButton[] {"Найти компанию по кофейку", "Календарь встреч" },
                                    new KeyboardButton[] {"Профиль","Настройки и помощь" },
                                    }
                                )
                                {
                                    ResizeKeyboard = true
                                }
                            );*/
                            await botClient.SendPhotoAsync(
                                chatId: message.Chat.Id,
                                photo: InputFile.FromUri("https://raw.githubusercontent.com/seku345/RandomCoffeeTelegramBot/main/TgBot/im/o-bot.png"),
                                caption: "\"MISiS: Cappuccino Conversations\" – ваш университетский спутник в мире образования и общения. Мы помогаем студентам находить учебных партнеров, расширять социальные связи и достигать учебных целей.\r\n\r\nЧем мы занимаемся?\r\n\r\nПодбираем партнеров для совместного обучения.\r\nСоздаем возможности для неформальных встреч и общения.\r\nПоддерживаем вас в учебных заданиях и проектах.\r\nКак это работает?\r\nЗарегистрируйтесь, заполните профиль и начните общение с единомышленниками. Мы предоставим вам персонализированные рекомендации и поможем найти учебного партнера.\r\n\r\nНаши цели:\r\n\r\nПоддержка в достижении успехов в учебе.\r\nСоздание дружного студенческого сообщества.\r\nУлучшение образовательной среды.\r\nДобро пожаловать в \"MISiS: Cappuccino Conversations\" – ваш надежный партнер в учебе и общении!",
                                parseMode: ParseMode.Html,
                                cancellationToken: cancellationToken);
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
                            /*await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Чем помочь?",
                                replyMarkup: new ReplyKeyboardMarkup(
                                    new[]
                                    {
                                    new KeyboardButton[] {"Переключить язык", "Сообщить об ошибке" },
                                    new KeyboardButton[] {"О боте","Удалить аккаунт"},
                                    }
                                )
                                {
                                    ResizeKeyboard = true
                                }
                            );*/
                            await botClient.SendPhotoAsync(
                                chatId: message.Chat.Id,
                                photo: InputFile.FromUri("https://raw.githubusercontent.com/seku345/RandomCoffeeTelegramBot/main/TgBot/im/nastroiki.png"),
                                caption: "",
                                parseMode: ParseMode.Html,
                                replyMarkup: new ReplyKeyboardMarkup(
                                    new[]
                                    {
                                    new KeyboardButton[] { "Основное меню", "Сообщить об ошибке"},
                                    new KeyboardButton[] {"О боте","Удалить аккаунт"},
                                    }
                                )
                                {
                                    ResizeKeyboard = true
                                },
                                cancellationToken: cancellationToken);
                            //nayti druga
                            break;
                        default:
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                "Прости... по запросу ничего не найдено. :(",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                            break;
                    }
                }
                else if(StartedReg == true)
                {
                    switch (count)
                    {
                        case 1:
                            newuser.Name = update.Message.Text;
                            await botClient.SendTextMessageAsync(
                                update.Message.Chat.Id,
                                "Фамилию:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                            count++;
                            break;
                        case 2:
                            newuser.SurName = update.Message.Text;
                            await botClient.SendTextMessageAsync(
                                update.Message.Chat.Id,
                                "Пол",
                                replyMarkup: new InlineKeyboardMarkup(
                                        new[]
                                        {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Мужской",
                                        callbackData: "Мужской"
                                    ),
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Женский",
                                        callbackData: "Женский"
                                    ),
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Боевой вертолёт",
                                        callbackData: "Боевой вертолёт"
                                    ),
                                        }
                                    ),
                                cancellationToken: cancellationToken);
                            count++;
                            break;
                        case 3:
                            break;
                        case 4:
                            newuser.Group = update.Message.Text;
                            await botClient.SendTextMessageAsync(
                                update.Message.Chat.Id,
                                "Возраст",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                            count++;
                            break;
                        case 5:
                            newuser.Age = (int)new Int32Converter().ConvertFromString(update.Message.Text);
                            await botClient.SendTextMessageAsync(
                                update.Message.Chat.Id,
                                "Вы завершили регистрацию! Переход в основное меню.",
                                replyMarkup: new ReplyKeyboardMarkup(
                                    new[]
                                    {
                                    new KeyboardButton[] {"Найти компанию по кофейку", "Календарь встреч" },
                                    new KeyboardButton[] {"Профиль","Настройки и помощь" },
                                    }
                                )
                                {
                                    ResizeKeyboard = true
                                }
                            );
                            Console.WriteLine(newuser.Name);
                            Register(newuser);
                            StartedReg = false;
                            count++;
                            break;

                    }
                }
                else if(changing != "None")
                {
                    switch (changing)
                    {
                        case "Name":
                            tempuser.Name = update.Message.Text;

                            db.SaveChanges();
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                $"Профиль\nИмя: {tempuser.Name}\nФамилия: {tempuser.SurName}\nГруппа: {tempuser.Group}\nПол: {tempuser.Sex}\nВозраст: {tempuser.Age}",
                                replyMarkup: new InlineKeyboardMarkup(
                                    new[]
                                    {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Изменить данные",
                                        callbackData: "ChangeData"
                                    ),
                                    }
                                )
                            );
                            changing = "None";
                            break;
                        case "SurName":
                            tempuser.SurName = update.Message.Text;
                            db.SaveChanges();
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                $"Профиль\nИмя: {tempuser.Name}\nФамилия: {tempuser.SurName}\nГруппа: {tempuser.Group}\nПол: {tempuser.Sex}\nВозраст: {tempuser.Age}",
                                replyMarkup: new InlineKeyboardMarkup(
                                    new[]
                                    {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Изменить данные",
                                        callbackData: "ChangeData"
                                    ),
                                    }
                                )
                            );
                            changing = "None";
                            break;
                        case "Group":
                            tempuser.Group = update.Message.Text;
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                $"Профиль\nИмя: {tempuser.Name}\nФамилия: {tempuser.SurName}\nГруппа: {tempuser.Group}\nПол: {tempuser.Sex}\nВозраст: {tempuser.Age}",
                                replyMarkup: new InlineKeyboardMarkup(
                                    new[]
                                    {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Изменить данные",
                                        callbackData: "ChangeData"
                                    ),
                                    }
                                )
                            );
                            changing = "None";
                            db.SaveChanges();
                            break;
                        case "Sex":
                            tempuser.Sex = update.Message.Text;
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                $"Профиль\nИмя: {tempuser.Name}\nФамилия: {tempuser.SurName}\nГруппа: {tempuser.Group}\nПол: {tempuser.Sex}\nВозраст: {tempuser.Age}",
                                replyMarkup: new InlineKeyboardMarkup(
                                    new[]
                                    {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Изменить данные",
                                        callbackData: "ChangeData"
                                    ),
                                    }
                                )
                            );
                            changing = "None";
                            db.SaveChanges();
                            break;
                        case "Age":
                            tempuser.Age = (int)new Int32Converter().ConvertFromString(update.Message.Text);
                            await botClient.SendTextMessageAsync(
                                message.Chat.Id,
                                $"Профиль\nИмя: {tempuser.Name}\nФамилия: {tempuser.SurName}\nГруппа: {tempuser.Group}\nПол: {tempuser.Sex}\nВозраст: {tempuser.Age}",
                                replyMarkup: new InlineKeyboardMarkup(
                                    new[]
                                    {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Изменить данные",
                                        callbackData: "ChangeData"
                                    ),
                                    }
                                )
                            );
                            changing = "None";
                            db.SaveChanges();
                            break;

                    }
                }
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null && update.CallbackQuery.Message != null)
            {
                switch (update.CallbackQuery.Data)
                {
                    case "LeaveQueue":
                        break;
                    case "ChangeData":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                $"Какие данные ты хочешь изменить?",
                                replyMarkup: new InlineKeyboardMarkup(new[] {
                                    new[]
                                    {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Имя",
                                        callbackData: "Name"
                                    ),
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Фамилию",
                                        callbackData: "SurName"
                                    ) },
                                    new[] {
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Группу",
                                        callbackData: "Group"
                                    ),
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Пол",
                                        callbackData: "Sex"
                                    ),
                                    InlineKeyboardButton.WithCallbackData(
                                        text: "Возраст",
                                        callbackData: "Age"
                                    ),
                                    }
                                    }
                                )
                            );
                        break;
                    case "Name":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                "Введи новое имя:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                        changing = "Name";
                        break;
                    case "SurName":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                "Введи новоую фамилию:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                        changing = "SurName";
                        break;
                    case "Group":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                "Введи новоую группу:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                        changing = "Group";
                        break;
                    case "Sex":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                "Введи новоый пол:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                        changing = "Sex";
                        break;
                    case "Age":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                "Введи новый возраст:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                        changing = "Age";
                        break;
                    case "Мужской":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        newuser.Sex = update.CallbackQuery.Data;
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                "Группу в которой ты обучаешься:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                        count++;
                        break;
                    case "Женский":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        newuser.Sex = update.CallbackQuery.Data;
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                "Группу в которой ты обучаешься:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                        count++;
                        break;
                    case "Боевой вертолёт":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        newuser.Sex = update.CallbackQuery.Data;
                        Console.WriteLine(update.CallbackQuery.Data);
                        await botClient.SendTextMessageAsync(
                                update.CallbackQuery.Message.Chat.Id,
                                "Группу в которой ты обучаешься:",
                                replyMarkup: new ReplyKeyboardRemove()
                            );
                        count++;
                        break;
                    case "Register":
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id,"Введи своё имя:",replyMarkup: new ReplyKeyboardRemove());
                        StartedReg = true;
                        tempuser.InQueue = true;
                        break;
                    default:
                        break;
                }
            }
            }
        public static bool checkreg(string user)
        {
            ApplicationContext db = new ApplicationContext();
            var users = db.Users.ToList();
            foreach (Userx u in users)
            {
                if (u.Id == user) { return true; };
            }
            return false;
        }
        public class Userx
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public string? SurName { get; set; }
            public string? Sex { get; set; }
            public string? Group { get; set; }
            public int? Age { get; set; }
            public bool? InQueue { get; set; }
            public string? InContact { get; set; }
            public long? ChatId { get; set; }
        }
        public static async Task waitforbuddy(Userx tempuser, ITelegramBotClient botClient, ApplicationContext db, Message message)
        {
            tempuser.InQueue = true;
            tempuser.InContact = "None";
            db.SaveChanges();
            var users = db.Users.ToList();
            foreach (Userx u in users)
            {
                Console.WriteLine($"{u.Id} {u.InQueue} {u.InContact}");
            }
            users.Remove(tempuser);
            Userx user = new Userx();
            Console.WriteLine(users.Count);
            while (true)
            {
                await Task.Delay(50);
                users = db.Users.ToList();
                users.Remove(tempuser);
                user = users[new Random().Next(users.Count)];
                if (user.InQueue==true || user.InContact == tempuser.Id)
                {
                    break;
                }
            }
            user.InContact = tempuser.Id;
            Console.WriteLine(tempuser.InContact);
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                $"Вам был подобран {user.Name} {user.SurName} (@{user.Id})\nГруппа: {user.Group}\nПол: {user.Sex}\nВозраст: {user.Age}",
                replyMarkup: new ReplyKeyboardRemove()
            );
            await botClient.SendTextMessageAsync(
                user.ChatId,
                $"Вам был подобран {tempuser.Name} {tempuser.SurName} (@{tempuser.Id})\nГруппа: {tempuser.Group}\nПол: {tempuser.Sex}\nВозраст: {tempuser.Age}",
                replyMarkup: new ReplyKeyboardRemove()
            );
            tempuser.InQueue = false;
            db.SaveChanges();
            return;
        }
        public static void Register(Userx newuser)
        {
            ApplicationContext db = new ApplicationContext();
            db.Users.Add(newuser);
            db.SaveChanges();
        }
        public static async Task DeleteUser(Userx olduser)
        {
            ApplicationContext db = new ApplicationContext();
            Console.WriteLine($"Deleted user {olduser.Id}");
            db.Users.Remove(olduser);
            db.SaveChanges();
        }
        public static async Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            Console.WriteLine(exception);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
            return;
        }
        public class ApplicationContext : DbContext
        {
            public DbSet<Userx> Users => Set<Userx>();
            public ApplicationContext() => Database.EnsureCreated();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlite("Data Source=CapCo2.db");
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
            /*ApplicationContext db = new ApplicationContext();
            var users = db.Users.ToList();
            foreach (Userx u in users)
            {
                DeleteUser(u);
            }*/
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
