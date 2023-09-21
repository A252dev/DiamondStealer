using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Net;
/*using Telegram.Bot.Types.InputFiles;*/
using Microsoft.EntityFrameworkCore;
using DiamondStealer.Models;
using Microsoft.Data.SqlClient;

namespace DiamondStealer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var diamond = new TelegramBotClient("{botToken}");
            var admin = new TelegramBotClient("{adminBotToken}");

            diamond.StartReceiving(DiamondUpdate, DiamondError);
            admin.StartReceiving(AdminUpdate, AdminError);
            Console.WriteLine("Diamond is started:");
            Console.ReadLine();
        }

        public static async Task AdminUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancel)
        {
            var message = update.Message;

            await Handler.AdminCallbackQueryAsync(botClient, update);

            if (message != null)
            {
                if (message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Здрасьте.\n" +
                        "Список команд:\n" +
                        "/get - получить список людей из базы\n" +
                        "/code add {10 цифр, хотя похуй, я не делал валидатор} {2MONTH/1YEAR/LIFETIME} - добавить промокод на халяву в базу\n" +
                        "/code remove {промо} - удалить промокод из базы\n" +
                        "/code list - список промо");

                    return;
                }

                if (message.Text == "/get")
                {
                    using (DiamondDbContext db = new DiamondDbContext())
                    {
                        var user = db.Users.FirstOrDefault(x => x.userId == message.Chat.Id);
                        if (user != null)
                        {
                            if (user.adminAccess == true)
                            {
                                List<Users> model = new List<Users>();

                                foreach (var usr in db.Users)
                                {
                                    if (usr.requestOnAccess == true && usr.access == false && usr.photoURL != null)
                                    {
                                        InlineKeyboardButton submitButton = new InlineKeyboardButton("1")
                                        {
                                            Text = "✅ Подтверждаю",
                                            CallbackData = usr.userId.ToString() + " submit",
                                        };
                                        InlineKeyboardButton cancelButton = new InlineKeyboardButton("2")
                                        {
                                            Text = "❌ Отклоняю",
                                            CallbackData = usr.userId.ToString() + " cancel",
                                        };

                                        List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>()
                                        {
                                            submitButton, cancelButton
                                        };
                                        InlineKeyboardMarkup markup = new InlineKeyboardMarkup(buttons);

                                        await botClient.SendTextMessageAsync(
                                                    chatId: message.Chat.Id,
                                                    text: $"Пользователь {usr.userId} желает получить доступ к твоему " +
                                                    $"конструктору! Он заплатил за тип: {usr.productType}. Ссылка на транзакцию: {usr.photoURL} ." +
                                                    "Чего изволите, Агент?",
                                                    replyMarkup: markup);
                                        model.Add(usr);
                                    }
                                }
                                if (model.Count == 0)
                                {
                                    await botClient.SendTextMessageAsync(
                                            chatId: message.Chat.Id,
                                            text: "Пока что нет людей для верификации!");

                                    return;
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "У тебя нет доступа к получению людей из базы!");
                                return;
                            }
                        }
                    }
                }

                if (message.Text.StartsWith("/code"))
                {
                    using (DiamondDbContext db = new DiamondDbContext())
                    {
                        var user = db.Users.FirstOrDefault(x => x.userId == message.Chat.Id);
                        if (user != null)
                        {
                            if (user.adminAccess == true)
                            {
                                string[] args = message.Text.Split(" ");
                                if (args.Length > 4)
                                {
                                    await botClient.SendTextMessageAsync(
                                        chatId: message.Chat.Id,
                                        text: "Много параметров!");
                                    return;
                                }
                                else if (args[1] == "list")
                                {
                                    foreach (var line in db.Promo)
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chatId: message.Chat.Id,
                                            text: $"id:{line.Id}, promo:{line.promo}, status:{line.status}, actBy:{line.activatedByUserId}");
                                    }
                                    await botClient.SendTextMessageAsync(
                                        chatId: message.Chat.Id,
                                        text: "Это весь список! Если ничего нет, то в базе пусто.");
                                    return;
                                }
                                else if (args.Length < 4)
                                {
                                    await botClient.SendTextMessageAsync(
                                        chatId: message.Chat.Id,
                                        text: "Мало аргументов");
                                    return;
                                }
                                else
                                {
                                    string parameter = args[1];
                                    int promo = int.Parse(args[2]);
                                    string expireType = args[3];
                                    if (parameter == "add")
                                    {
                                        Promo promoModel = new Promo()
                                        {
                                            activatedByUserId = 0,
                                            promo = promo,
                                            status = false,
                                            expireTime = expireType,
                                        };
                                        db.Promo.Add(promoModel);
                                        db.SaveChanges();
                                        await botClient.SendTextMessageAsync(
                                            chatId: message.Chat.Id,
                                            text: $"Промокод {promo} с типом {expireType} добавлен в базу!");
                                        return;
                                    }
                                    if (parameter == "remove")
                                    {
                                        var havePromo = db.Promo.SingleOrDefault(x => x.promo == promo);
                                        if (havePromo != null)
                                        {
                                            db.Promo.Remove(havePromo);
                                            db.SaveChanges();
                                            await botClient.SendTextMessageAsync(
                                                chatId: message.Chat.Id,
                                                text: $"Промокод {promo} с типом {expireType} был удалён из базы!");
                                            return;
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chatId: message.Chat.Id,
                                                text: "Промо не найден в базе!");
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(
                                                    chatId: message.Chat.Id,
                                                    text: "У тебя нет доступа к промокодам!");
                                return;
                            }
                        }
                    }
                }
            }
            return;
        }

        public static async Task DiamondUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancel)
        {
            var message = update.Message;

            await Handler.CallbackQueryAsync(botClient, update);

            if (message?.Text != null)
            {
                if (message.Text == "/start")
                {

                    InlineKeyboardButton startBtw = new InlineKeyboardButton("start")
                    {
                        Text = "✅ Я согласен",
                        CallbackData = "startAgree",
                    };

                    InlineKeyboardMarkup btwMarkup = new InlineKeyboardMarkup(startBtw);

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "⚠️ Внимание! За использование наших продуктов в неправомерных целях " +
                        "несёте ответственность Вы сами. Данный бот был создан в исключительно ознакомительных " +
                        "целях. Мы не несём ответственность за потенциальный вред, который Вы можете причинить " +
                        "используя данного бота!",
                        replyMarkup: btwMarkup);

                    return;
                }

                if (message.Text == "\U0001f6d2 Купить лицензию")
                {

                    List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>()
                    {
                        new List<InlineKeyboardButton>()
                        {
                            new InlineKeyboardButton("1")
                            {
                                Text = "💸 2 месяца (5$)",
                                CallbackData = "buy1",
                            },
                        },

                        new List<InlineKeyboardButton>()
                        {
                            new InlineKeyboardButton("2")
                            {
                                Text = "💸 1 год (15$)",
                                CallbackData = "buy2",
                            },
                        },

                        new List<InlineKeyboardButton>()
                        {
                            new InlineKeyboardButton("3")
                            {
                                Text = "💸 Навсегда (30$)",
                                CallbackData = "buy3",
                            },
                        },
                    };

                    InlineKeyboardMarkup buyButtons = new InlineKeyboardMarkup(buttons);

                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "⚡️ Выберите тип товара для покупки.",
                        replyMarkup: buyButtons);
                }

                if (message.Text == "📋 Описание")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "🛑 Это конструктор в телеграмме, который может сделать вам стиллер." +
                        " Задайте необходимые параметры и получите готовый .exe файл для дальнейшего использования.");

                    return;
                }

                if (message.Text == "\U0001f9d1 Профиль")
                {
                    using (DiamondDbContext db = new DiamondDbContext())
                    {
                        var user = db.Users.FirstOrDefault(x => x.userId == message.Chat.Id);

                        if (user != null)
                        {
                            var userAccess = db.UserAccess.FirstOrDefault(x => x.userId == message.Chat.Id);

                            if (user != null)
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "🧑 Ваш Id: " + user.userId + "\n" +
                                    "🔒 Тип подписки: " + user.productType + "\n" +
                                    "🕒 Время подписки: " + userAccess.timeAccess);
                            }
                        }
                        return;
                    }
                }

                if (message.Text == "📕 Контакты")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Наши контакты:\n" +
                        "Форум Lolzteam: (ссылка типо)");
                    return;
                }

                if (message.Text.StartsWith("/alert"))
                {
                    using (DiamondDbContext db = new DiamondDbContext())
                    {
                        var user = db.Users.FirstOrDefault(x => x.userId == message.Chat.Id);

                        if (user != null)
                        {
                            if (user.adminAccess == false)
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "У вас нет доступа к данной команде!");
                                return;
                            }
                            else if (user.adminAccess == true)
                            {
                                string[] args = message.Text.Split(" ");
                                if (args.Length > 1)
                                {
                                    foreach (var usr in db.Users)
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chatId: usr.userId,
                                            text: string.Join(" ", args.Skip(1)));
                                    }
                                    await botClient.SendTextMessageAsync(
                                        chatId: message.Chat.Id,
                                        text: "Ваше сообщение было разослано всем!");
                                    return;
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(
                                        chatId: message.Chat.Id,
                                        text: "Недостаточно параметров! Пример: /alert [сообщение]");
                                    return;
                                }
                            }
                        }
                        return;
                    }
                }

                if (message.Text.StartsWith("/build"))
                {
                    using (DiamondDbContext db = new DiamondDbContext())
                    {
                        var user = db.Users.FirstOrDefault(x => x.userId == message.Chat.Id);
                        var userAccess = db.UserAccess.FirstOrDefault(x => x.userId == message.Chat.Id);
                        if (user != null && userAccess.userId != null)
                        {
                            if (user.access == false || userAccess.timeAccess <= DateTime.UtcNow)
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "🔒 У вас нет доступа к билдеру!\n" +
                                    "Для приобретения необходимо перейти в '\U0001f6d2 Купить лицензию', и следовать дальнейшим указаниям.");

                                return;
                            }
                            else if (user.access == true && userAccess.timeAccess > DateTime.UtcNow)
                            {
                                string[] args = message.Text.Split(" ");
                                if (args.Length == 2)
                                {
                                    await Builder.StartBuild(botClient, update, args);
                                    return;
                                }
                                else if (args.Length > 2)
                                {
                                    await botClient.SendTextMessageAsync(
                                        chatId: message.Chat.Id,
                                        text: "❌ Ошибка ввода!\n" +
                                        "Пример: /build [ссылка на хост]");

                                    return;
                                }
                                else if (args.Length < 2)
                                {
                                    await botClient.SendTextMessageAsync(
                                        chatId: message.Chat.Id,
                                        text: "⛔️ Не хватает параметров!\n" +
                                        "Пример: /build [ссылка на хост]");

                                    return;
                                }
                            }
                        }
                        return;
                    }
                }

                if (message.Text.StartsWith("/code"))
                {
                    string[] args = message.Text.Split(" ");
                    if (args.Length > 2)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Вы указали слишком много параметров!");
                        return;
                    }
                    else if (args.Length < 2)
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Вы указали слишком мало параметров!");
                        return;
                    }
                    else
                    {
                        int promocode = int.Parse(args[1]);
                        using (DiamondDbContext db = new DiamondDbContext())
                        {
                            var user = db.Users.FirstOrDefault(x => x.userId == message.Chat.Id);
                            var userAccess = db.UserAccess.FirstOrDefault(x => x.userId == message.Chat.Id);
                            if (user != null && userAccess != null)
                            {
                                var promo = db.Promo.FirstOrDefault(x => x.promo == promocode);
                                if (promo != null)
                                {
                                    if (promo.expireTime == "2MONTH" && promo.status == false)
                                    {
                                        userAccess.timeAccess = DateTime.UtcNow.AddMonths(2);
                                        user.requestOnAccess = false;
                                        user.access = true;
                                        user.productType = "2MONTH";

                                        promo.status = true;
                                        promo.activatedByUserId = (int)message.Chat.Id;
                                        db.SaveChanges();
                                        await botClient.SendTextMessageAsync(
                                            chatId: message.Chat.Id,
                                            text: $"Вы успешно активировали код для типа лицензии {promo.expireTime}");
                                        return;
                                    }
                                    else if (promo.status == true)
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chatId: message.Chat.Id,
                                            text: "Данный промокод уже активирован!");
                                        return;
                                    }
                                    if (promo.expireTime == "1YEAR")
                                    {
                                        userAccess.timeAccess = DateTime.UtcNow.AddYears(1);
                                        await botClient.SendTextMessageAsync(
                                            chatId: message.Chat.Id,
                                            text: $"Вы успешно активировали код для типа лицензии {promo.expireTime}");
                                        return;
                                    }
                                    if (promo.expireTime == "LIFETIME")
                                    {
                                        userAccess.timeAccess = DateTime.UtcNow.AddYears(10);
                                        await botClient.SendTextMessageAsync(
                                            chatId: message.Chat.Id,
                                            text: $"Вы успешно активировали код для типа лицензии {promo.expireTime}");
                                        return;
                                    }
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(
                                        chatId: message.Chat.Id,
                                        text: "Такого промокода не существует!");
                                    return;
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "Вас тут не существует!");
                                return;
                            }
                        }
                    }
                }

                if (message.Text.Contains("http"))
                {
                    using (DiamondDbContext db = new DiamondDbContext())
                    {
                        var user = db.Users.FirstOrDefault(x => x.userId == message.Chat.Id);
                        if (user != null)
                        {
                            if (user.access == false)
                            {
                                user.photoURL = message.Text;
                                user.requestOnAccess = true;
                                db.SaveChanges();

                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "✅ Транзакция отправлена на проверку!\n" +
                                    "Если вы хотите изменить ссылку, отправьте другую ;)");
                                return;
                            }
                            else if (user.access == true)
                            {
                                await botClient.SendTextMessageAsync(
                                    chatId: message.Chat.Id,
                                    text: "🔓 У вас уже есть доступ к билдеру!\n" +
                                    "Для его использования потребуется команда /build [ссылка на ваш хост].");

                                return;
                            }
                        }
                    }
                }
                return;
            }

            /*if (message?.Photo != null)
            {
                var photo = message.Photo;
                var fileId = photo.LastOrDefault()?.FileId;

                if (fileId != null)
                {       
                    using (DiamondDbContext db = new DiamondDbContext())
                    {
                        var user = db.Users.FirstOrDefault(x => x.userId == message.Chat.Id);

                        if (user.access == false)
                        {
                            Telegram.Bot.Types.File file = await botClient.GetFileAsync(fileId);
                            string filePath = $"https://api.telegram.org/file/bot{botToken}/" + file.FilePath;
                            user.photoURL = filePath;
                            user.requestOnAccess = true;
                            db.SaveChanges();

                            var webClient = new WebClient();
                            var photoBytes = webClient.DownloadData(user.photoURL.ToString());
                            var photoMemory = new MemoryStream(photoBytes);

                            await botClient.SendPhotoAsync(
                                chatId: message.Chat.Id,
                                photo: InputFile.FromStream(photoMemory),
                                caption: "✅ Транзакция отправлена на проверку!\n" +
                                "Если вы хотите изменить скриншот, отправьте другой ;)");

                            return;
                        } else if (user.access == true)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "🔓 У вас уже есть доступ к билдеру!\n" +
                                "Для его использования потребуется команда /build [имя файла на латинице].");

                            return;
                        }
                    }                    
                }
                return;
            }*/
            return;
        }

        private static Task DiamondError(ITelegramBotClient botClient, Exception arg2, CancellationToken cancel)
        {
            Console.WriteLine("Diamond error...");
            throw new NotImplementedException();
        }
        private static Task AdminError(ITelegramBotClient arg1, Exception arg2, CancellationToken cancel)
        {
            Console.WriteLine("Admin error...");
            throw new NotImplementedException();
        }
    }
}