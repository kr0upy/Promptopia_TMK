using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;

namespace TMK.Backend
{
    public class TmkBot
    {
        private readonly string _token = "8489507071:AAFcNHICJx5Glfk_fnF9Cqhm95LvnCHb5AI";
        private readonly string _webAppUrl = "https://unhonoured-unorbitally-kiley.ngrok-free.dev";
        private TelegramBotClient _bot;

        public TmkBot()
        {
            _bot = new TelegramBotClient(_token);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            var me = await _bot.GetMeAsync();
            Console.WriteLine($"Bot started as @{me.Username}");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                var message = update.Message;
                if (message.Text == "/start")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: new ChatId(message.Chat.Id),
                        text: "Добро пожаловать в ТМК! Откройте мини-приложение:",
                        replyMarkup: new ReplyKeyboardMarkup(
                            new[]
                            {
                                new[]
                                {
                                    new KeyboardButton("Открыть ТМК")
                                    {
                                        WebApp = new WebAppInfo { Url = _webAppUrl }
                                    }
                                }
                            }
                        )
                        {
                            ResizeKeyboard = true
                        },
                        cancellationToken: cancellationToken
                    );

                    await botClient.SendTextMessageAsync(
                        new ChatId(message.Chat.Id),
                        "Для запуска мини-приложения используйте /start.",
                        cancellationToken: cancellationToken
                    );
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        new ChatId(message.Chat.Id),
                        "Текст сообщения",
                        cancellationToken: cancellationToken
                    );
                }
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Bot error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}