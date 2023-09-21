using DiamondStealer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DiamondStealer
{
    internal class Handler
    {
        public static string AdminCall(string result, ITelegramBotClient botClient, Update update)
        {
            var action = update.CallbackQuery.Data;

            CallbackQuery veryfyUser = new CallbackQuery()
            {
                Data = update.CallbackQuery.Data,
            };

            if (veryfyUser.Data == action)
            {
                result = action;
            }

            return result;
        }
        public async static Task AdminCallbackQueryAsync(ITelegramBotClient botClient, Update update)
        {
            if (update.CallbackQuery != null)
            {
                var action = update.CallbackQuery.Message;
                if (action != null)
                {
                    var detectUser = AdminCall(null, botClient, update);

                    if (detectUser != null)
                    {
                        if (detectUser.Contains(" submit"))
                        {
                            detectUser = detectUser.Replace(" submit", "");
                            long id = long.Parse(detectUser);
                            using (DiamondDbContext db = new DiamondDbContext())
                            {
                                var user = db.Users.FirstOrDefault(x => x.userId == id);
                                var userAccess = db.UserAccess.FirstOrDefault(x => x.userId == id);

                                if (user != null)
                                {
                                    if (user.productType == "2MONTH")
                                    {
                                        user.requestOnAccess = false;
                                        userAccess.userId = id;
                                        userAccess.timeAccess = DateTime.Now.AddDays(60);
                                        user.access = true;

                                        TelegramBotClient client = new TelegramBotClient("{botToken}");

                                        await client.SendTextMessageAsync(
                                            chatId: id,
                                            text: "✅ Поздравляем! Подписка на 2 месяца успешно оформлена. " +
                                            "Для того, чтобы начать работу Вам необходимо настроить сервер. " +
                                            "Обучение по настройке распологается по ссылке: (ссылка типо)");

                                    }
                                    else if (user.productType == "1YEAR")
                                    {
                                        user.requestOnAccess = false;
                                        userAccess.userId = id;
                                        userAccess.timeAccess = DateTime.Now.AddYears(1);
                                        user.access = true;

                                        TelegramBotClient client = new TelegramBotClient("botToken");

                                        await client.SendTextMessageAsync(
                                            chatId: id,
                                            text: "✅ Поздравляем! Подписка на 1 год успешно оформлена. " +
                                            "Для того, чтобы начать работу Вам необходимо настроить сервер. " +
                                            "Обучение по настройке распологается по ссылке: (ссылка типо)");
                                    }
                                    else if (user.productType == "LIFETIME")
                                    {
                                        user.requestOnAccess = false;
                                        userAccess.timeAccess = DateTime.Now.AddYears(10);
                                        user.access = true;

                                        TelegramBotClient client = new TelegramBotClient("botToken");

                                        await client.SendTextMessageAsync(
                                            chatId: id,
                                            text: "✅ Поздравляем! Подписка навсегда успешно оформлена. " +
                                            "Для того, чтобы начать работу Вам необходимо настроить сервер. " +
                                            "Обучение по настройке распологается по ссылке: (ссылка типо)");
                                    }

                                    db.SaveChanges();

                                    await botClient.SendTextMessageAsync(
                                        action.Chat.Id,
                                        text: "Пользователь " + id + " добавлен в базу! Тип товара был: " +
                                        "" + user.productType + ". Время подписки: " + userAccess.timeAccess + ".");

                                    return;
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(
                                        chatId: action.Chat.Id,
                                        text: "Аккаунт не найден!");

                                    return;
                                }
                            }
                        }
                        if (detectUser.Contains(" cancel"))
                        {
                            detectUser = detectUser.Replace(" cancel", "");
                            long id = long.Parse(detectUser);

                            TelegramBotClient client = new TelegramBotClient("{botToken}");
                            await client.SendTextMessageAsync(
                                chatId: id,
                                text: "❌ Вы не прошли проверку! Если это было ошибкой, то можете отослать верную ссылку " +
                                "на транзакцию. В ином случае можете связаться с нами на одном из форумов.");

                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "Пользователю " + id + " выслано уведомление о смене своего изображения!");

                            return;
                        }
                    }
                }
            }
            return;
        }
        public async static Task AddLister(long id, ITelegramBotClient botClient, Update update)
        {
            Update[] updates = await botClient.GetUpdatesAsync();

            foreach (Update up in updates)
            {
                if (up.Type == UpdateType.Message && up.Message.Type == MessageType.Photo && up.Message.From.IsBot)
                {
                    PhotoSize photo = update.Message.Photo.Last();
                    var filePath = await botClient.GetFileAsync(photo.FileId);
                    var URL = $"https://api.telegram.org/file/bot{botToken}}/{filePath}";

                    Console.WriteLine(URL);
                }
            }

            /*using (DiamondDbContext db = new DiamondDbContext())
            {
                var file = Telegram.Bot.
            }*/


            return;
        }
        public static string Call(string result, ITelegramBotClient botClient, Update update)
        {
            var action = update.CallbackQuery.Data;

            CallbackQuery startAgree = new CallbackQuery() { Data = "startAgree" };
            CallbackQuery payed = new CallbackQuery() { Data = "payed" };

            CallbackQuery buy1 = new CallbackQuery() { Data = "buy1" };
            CallbackQuery buy2 = new CallbackQuery() { Data = "buy2" };
            CallbackQuery buy3 = new CallbackQuery() { Data = "buy3" };

            CallbackQuery USDTTrc20 = new CallbackQuery() { Data = "TRC20" };
            CallbackQuery USDTErc20 = new CallbackQuery() { Data = "ERC20" };
            CallbackQuery TronTRX = new CallbackQuery() { Data = "TRX" };

            List<CallbackQuery> accountInfo = new List<CallbackQuery> { startAgree, payed };

            List<CallbackQuery> productList = new List<CallbackQuery>
            {
                buy1, buy2, buy3
            };

            List<CallbackQuery> currencyList = new List<CallbackQuery>()
            {
                USDTTrc20, USDTErc20, TronTRX
            };

            foreach (CallbackQuery info in accountInfo)
            {
                if (info.Data == action)
                {
                    result = action;
                }
            }

            foreach (CallbackQuery list in productList)
            {
                if (list.Data == action)
                {
                    result = action;
                }
            }

            foreach (CallbackQuery currency in currencyList)
            {
                if (currency.Data == action)
                {
                    result = action;
                }
            }

            return result;
        }
        public async static Task CallbackQueryAsync(ITelegramBotClient botClient, Update update)
        {
            if (update.CallbackQuery != null)
            {
                var action = update.CallbackQuery.Message;
                if (action != null)
                {
                    var selector = Call(null, botClient, update);

                    if (selector != null)
                    {
                        if (selector == "startAgree")
                        {
                            List<KeyboardButton> firstLine = new List<KeyboardButton>()
                            {
                                new KeyboardButton("buy")
                                {
                                    Text = "🛒 Купить лицензию",
                                },
                                new KeyboardButton("description")
                                {
                                    Text = "📋 Описание",
                                },
                            };

                            List<KeyboardButton> secondLine = new List<KeyboardButton>()
                            {
                                new KeyboardButton("profile")
                                {
                                    Text = "🧑 Профиль",
                                },
                                new KeyboardButton("contacts")
                                {
                                    Text = "📕 Контакты",
                                },
                            };

                            List<List<KeyboardButton>> keyboardButtons = new List<List<KeyboardButton>>()
                            {
                                firstLine, secondLine
                            };

                            ReplyKeyboardMarkup linesMarkup = new ReplyKeyboardMarkup(keyboardButtons)
                            {
                                ResizeKeyboard = true,
                            };

                            Users user = new Users()
                            {
                                userId = action.Chat.Id,
                                access = false,
                                adminAccess = false,
                                requestOnAccess = false,
                                productType = "NONE",
                                currencyType = "NONE",
                                photoURL = "NONE",
                            };

                            UserAccess userAccess = new UserAccess()
                            {
                                userId = action.Chat.Id,
                                timeAccess = DateTime.Now,
                            };

                            using (DiamondDbContext db = new DiamondDbContext())
                            {
                                bool haveUser = db.Users.Any(x => x.userId == action.Chat.Id);
                                bool haveAccess = db.UserAccess.Any(x => x.userId == action.Chat.Id);

                                if (haveUser == false && haveAccess == false)
                                {
                                    db.Users.Add(user);
                                    db.UserAccess.Add(userAccess);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    // nothing ?
                                }
                            }

                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "Выберите желаемый пункт меню.",
                                replyMarkup: linesMarkup);

                            return;
                        }

                        if (selector == "payed")
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "Отлично. Отправьте боту ссылку на транзакцию для проверки платежа! " +
                                "После этого вы получите доступ к конструктору.");
                            return;
                        }

                        if (selector == "buy1")
                        {
                            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>()
                            {
                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("1")
                                    {
                                        Text = "💲 USDT TRC20",
                                        CallbackData = "TRC20",
                                    },
                                },

                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("2")
                                    {
                                        Text = "💲 USDT ERC20",
                                        CallbackData = "ERC20",
                                    },
                                },

                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("3")
                                    {
                                        Text = "⚡ Tron TRX",
                                        CallbackData = "TRX",
                                    },
                                },
                            };

                            InlineKeyboardMarkup markup = new InlineKeyboardMarkup(buttons);

                            using (DiamondDbContext db = new DiamondDbContext())
                            {
                                var user = db.Users.FirstOrDefault(x => x.userId == action.Chat.Id);

                                if (user != null)
                                {
                                    user.productType = "2MONTH";
                                    db.SaveChanges();
                                }
                            }

                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "Стоимость приобретения доступа к билдеру на 2 месяца равна 100 " +
                                "американским долларам. В какой криптовалюте вы желаете оплатить товар?",
                                replyMarkup: markup);

                            return;
                        }

                        if (selector == "buy2")
                        {
                            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>()
                            {
                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("1")
                                    {
                                        Text = "💲 USDT TRC20",
                                        CallbackData = "TRC20",
                                    },
                                },

                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("2")
                                    {
                                        Text = "💲 USDT ERC20",
                                        CallbackData = "ERC20",
                                    },
                                },

                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("3")
                                    {
                                        Text = "⚡ Tron TRX",
                                        CallbackData = "TRX",
                                    },
                                },
                            };

                            InlineKeyboardMarkup markup = new InlineKeyboardMarkup(buttons);

                            using (DiamondDbContext db = new DiamondDbContext())
                            {
                                var user = db.Users.FirstOrDefault(x => x.userId == action.Chat.Id);

                                if (user != null)
                                {
                                    user.productType = "1YEAR";
                                    db.SaveChanges();
                                }
                            }

                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "Стоимость приобретения доступа к билдеру на 1 год равна 300 " +
                                "американским долларам. В какой криптовалюте вы желаете оплатить товар?",
                                replyMarkup: markup);

                            return;
                        }

                        if (selector == "buy3")
                        {
                            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>()
                            {
                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("1")
                                    {
                                        Text = "💲 USDT TRC20",
                                        CallbackData = "TRC20",
                                    },
                                },

                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("2")
                                    {
                                        Text = "💲 USDT ERC20",
                                        CallbackData = "ERC20",
                                    },
                                },

                                new List<InlineKeyboardButton>()
                                {
                                    new InlineKeyboardButton("3")
                                    {
                                        Text = "⚡ Tron TRX",
                                        CallbackData = "TRX",
                                    },
                                },
                            };

                            InlineKeyboardMarkup markup = new InlineKeyboardMarkup(buttons);

                            using (DiamondDbContext db = new DiamondDbContext())
                            {
                                var user = db.Users.FirstOrDefault(x => x.userId == action.Chat.Id);

                                if (user != null)
                                {
                                    user.productType = "LIFETIME";
                                    db.SaveChanges();
                                }
                            }

                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "Стоимость приобретения доступа к билдеру навсегда равна 500 " +
                                "американским долларам. В какой криптовалюте вы желаете оплатить товар?",
                                replyMarkup: markup);

                            return;
                        }

                        if (selector == "TRC20")
                        {
                            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>()
                            {
                                new InlineKeyboardButton("1")
                                {
                                    Text = "✅ Оплачено",
                                    CallbackData = "payed",
                                },
                            };

                            InlineKeyboardMarkup markup = new InlineKeyboardMarkup(buttons);

                            using (DiamondDbContext db = new DiamondDbContext())
                            {
                                var user = db.Users.FirstOrDefault(x => x.userId == action.Chat.Id);

                                if (user != null)
                                {
                                    user.currencyType = "TRC20";
                                    db.SaveChanges();
                                }
                            }

                            string walletTRC20 = "trc20wal";
                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "⚠️ Отлично. Для оплаты вам необходимо перевести ту сумму, которая была " +
                                "указана в названии товара! При неверном переводе платёж может не пройти! " +
                                "После отправки средств нажмите кнопку 'Оплачено'!\n" +
                                "Кошелёк для перевода валюты Tether TRC20: " + walletTRC20,
                                replyMarkup: markup);

                            return;
                        }

                        if (selector == "ERC20")
                        {
                            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>()
                            {
                                new InlineKeyboardButton("1")
                                {
                                    Text = "✅ Оплачено",
                                    CallbackData = "payed",
                                },
                            };

                            InlineKeyboardMarkup markup = new InlineKeyboardMarkup(buttons);

                            using (DiamondDbContext db = new DiamondDbContext())
                            {
                                var user = db.Users.FirstOrDefault(x => x.userId == action.Chat.Id);

                                if (user != null)
                                {
                                    user.currencyType = "ERC20";
                                    db.SaveChanges();
                                }
                            }

                            string walletERC20 = "erc20wal";
                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "⚠️ Отлично. Для оплаты вам необходимо перевести ту сумму, которая была " +
                                "указана в названии товара! При неверном переводе платёж может не пройти! " +
                                "После отправки средств нажмите кнопку 'Оплачено'!\n" +
                                "Кошелёк для перевода валюты Tether ERC20: " + walletERC20);

                            return;
                        }

                        if (selector == "TRX")
                        {
                            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>()
                            {
                                new InlineKeyboardButton("1")
                                {
                                    Text = "✅ Оплачено",
                                    CallbackData = "payed",
                                },
                            };

                            InlineKeyboardMarkup markup = new InlineKeyboardMarkup(buttons);

                            using (DiamondDbContext db = new DiamondDbContext())
                            {
                                var user = db.Users.FirstOrDefault(x => x.userId == action.Chat.Id);

                                if (user != null)
                                {
                                    user.currencyType = "TRX";
                                    db.SaveChanges();
                                }
                            }

                            string walletTRX = "trxwal";
                            await botClient.SendTextMessageAsync(
                                chatId: action.Chat.Id,
                                text: "⚠️ Отлично. Для оплаты вам необходимо перевести ту сумму, которая была " +
                                "указана в названии товара! При неверном переводе платёж может не пройти! " +
                                "После отправки средств нажмите кнопку 'Оплачено'!\n" +
                                "Кошелёк для перевода валюты Tron TRX: " + walletTRX);

                            return;
                        }
                        else return;
                    }
                }
            }
            else return;
        }
    }
}
