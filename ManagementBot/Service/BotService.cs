using ManagementBot.Commons;
using ManagementBot.Data;
using ManagementBot.Enitis;
using ManagementBot.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrustyTalents.Service.Services.Emails;

namespace ManagementBot.Service
{
    public class BotService : IBotService
    {
        private readonly IGenericRepository<Users> userRepository;
        private readonly IGenericRepository<Requests> requestsRepository;
        private readonly ITelegramBotClient botService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BotService> _logger;
        private readonly IEmailInboxService emailInboxService;
        public BotService(IGenericRepository<Users> userRepository, ITelegramBotClient botService, IGenericRepository<Requests> requestsRepository, IServiceScopeFactory scopeFactory, ILogger<BotService> logger, IEmailInboxService emailInboxService)
        {
            this.userRepository = userRepository;
            this.botService = botService;
            this.requestsRepository = requestsRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
            this.emailInboxService = emailInboxService;
        }

        public async ValueTask HandleUpdateAsync(Update update)
        {
            if (update is null) return;

            else if(update.Message is not null) await HandleMessageAsync(update.Message, update?.CallbackQuery);
            else if(update.CallbackQuery is not null) await HandleMessageAsync(update?.CallbackQuery?.Message, update?.CallbackQuery);

        }

        public async ValueTask HandleMessageAsync(Message? message, CallbackQuery? callbackQuery)
        {
           if (message is null) return;
           long chatId = message.Chat.Id;

           var user = await userRepository.GetAll(x => x.ChatId == chatId).Include(x => x.Requests).FirstOrDefaultAsync();

            var sendRequest = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Send Request", CommandsKey.SendRequest),
                    }
                });


            if (user is null)
            {
                await userRepository.CreateAsync(new Users { State = State.send_request, ChatId = chatId });
                await userRepository.SaveChangeAsync();

                await botService.SendMessage(chatId, TelegramTexts.WelcomeMessage, parseMode: ParseMode.Markdown, replyMarkup: sendRequest);

                return;
            }

            if (TelegramBotExtensions.TelegramBotExtensions.IsStartOrSettingCommand(message?.Text))
            {
                if (message.Text == CommandsKey.Start)
                {
                    await botService.SendMessage(chatId, TelegramTexts.СontinueMessage, parseMode: ParseMode.Markdown, replyMarkup: sendRequest);
                    user.State = State.send_request;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                }
                else if(message.Text == CommandsKey.SendRequest)
                {
                    user.State = State.send_apply_button;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("✅ Yes", CommandsKey.ApplyRequest),
                            InlineKeyboardButton.WithCallbackData("❌ No", CommandsKey.CancelRequest)
                        }
                    });

                    await botService.SendMessage(chatId, TelegramTexts.RequestFillingMessage, parseMode: ParseMode.Markdown, replyMarkup: inlineKeyboard);
                }

                return;
            }

           await StartCommands(chatId, message, user, callbackQuery);
           return;
        }

        public async ValueTask StartCommands(long chatId, Message? messageText, Users? user, CallbackQuery? callbackQuery)
        {
            var nextButton = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton(CommandsKey.NextText)
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            var sendRequest = new InlineKeyboardMarkup(new[]
             {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Send Request", CommandsKey.SendRequest),
                }
            });

            switch (user?.State)
            {
                case State.start:
                    await botService.SendMessage(chatId, TelegramTexts.СontinueMessage, parseMode: ParseMode.Markdown, replyMarkup: sendRequest);
                    user.State = State.send_request;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_request:

                    user.State = State.send_apply_button;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("✅ Yes", CommandsKey.ApplyRequest),
                            InlineKeyboardButton.WithCallbackData("❌ No", CommandsKey.CancelRequest)
                        }
                    });

                    await botService.SendMessage(chatId, TelegramTexts.RequestFillingMessage, parseMode: ParseMode.Markdown, replyMarkup: inlineKeyboard);
                    return;
                case State.reject:
                    await botService.SendMessage(chatId, TelegramTexts.RequestFillingMessage, parseMode: ParseMode.Markdown, replyMarkup: sendRequest);
                    user.State = State.send_request;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_apply_button:

                    if(callbackQuery.Data == CommandsKey.CancelRequest)
                    {
                        user.State = State.reject;
                        user.UpdatedAt = DateTime.UtcNow;
                        userRepository.UpdateAsync(user);

                        await botService.SendMessage(chatId, TelegramTexts.CancelRequest, parseMode: ParseMode.Markdown, replyMarkup: sendRequest);
                    }
                    else
                    {
                        user.State = State.send_phone_number;
                        userRepository.UpdateAsync(user);

                        var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                        {
                            new KeyboardButton("📞 Share My Phone Number") { RequestContact = true }
                        })
                        {
                            ResizeKeyboard = true,
                            OneTimeKeyboard = true
                        };

                        await botService.SendMessage(chatId, TelegramTexts.SendPhoneNumber, parseMode: ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
                    }
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_phone_number:
                    if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;
                    await CreateRequest(true, user, user?.State, !string.IsNullOrEmpty(messageText?.Text) ? messageText?.Text : messageText?.Contact?.PhoneNumber);
                    await botService.SendMessage(chatId: chatId, text: TelegramTexts.SendEmail, parseMode: ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove());

                    user.State = State.send_email;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_email:
                    if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;

                    if (string.IsNullOrWhiteSpace(messageText?.Text) || !IsValidEmail(messageText.Text))
                    {
                        await botService.SendMessage(chatId, "❌ Invalid email address. Please enter the correct email address.");
                        return;
                    }

                    await CreateRequest(false, user, user?.State, messageText?.Text);
                    await botService.SendMessage(chatId, TelegramTexts.SendResponsiblePerson, parseMode: ParseMode.Markdown);

                    user.State = State.send_full_name;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_position:
                     if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;
                    await CreateRequest(false, user, user?.State, messageText?.Text);
                    await botService.SendMessage(chatId, TelegramTexts.SendInquiryContent, parseMode: ParseMode.Markdown);

                    user.State = State.send_inquiry_content;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_full_name:
                    if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;
                    await CreateRequest(false, user, user?.State, messageText?.Text);
                    await botService.SendMessage(chatId, TelegramTexts.SendCompanyName, parseMode: ParseMode.Markdown);

                    user.State = State.send_company_name;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_company_name:
                     if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;
                    await CreateRequest(false, user, user?.State, messageText?.Text);
                    await botService.SendMessage(chatId, TelegramTexts.SendPosition, parseMode: ParseMode.Markdown);

                    user.State = State.send_position;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;

                case State.send_inquiry_content:
                     if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;
                    await CreateRequest(false, user, user?.State, messageText?.Text);
                    await botService.SendMessage(chatId, TelegramTexts.SendProjectBudjet, parseMode: ParseMode.Markdown);

                    user.State = State.send_project_salary;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;       
                
                case State.send_project_salary:
                     if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;
                    await CreateRequest(false, user, user?.State, messageText?.Text);
                    await botService.SendMessage(chatId, TelegramTexts.SendInquirySource, parseMode: ParseMode.Markdown, replyMarkup: nextButton);

                    user.State = State.send_inquiry_source;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_inquiry_source:
                    if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;
                    if (messageText?.Text == CommandsKey.NextText) await CreateRequest(false, user, user?.State, "");

                    else await CreateRequest(false, user, user?.State, messageText?.Text);

                    var submitbutton = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                             new KeyboardButton("✅ Submit")
                        }
                    })
                    {
                        ResizeKeyboard = true,
                        OneTimeKeyboard = true
                    };

                    await botService.SendMessage(chatId, TelegramTexts.SendAdditionInformation, parseMode: ParseMode.Markdown, replyMarkup: submitbutton);

                    user.State = State.send_information;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                case State.send_information:
                    if (!string.IsNullOrEmpty(callbackQuery?.Data)) return;
                    if (messageText?.Text == "✅ Submit") await CreateRequest(false, user, user?.State, "");

                    else await CreateRequest(false, user, user?.State, messageText?.Text);

                    await botService.SendMessage(chatId, TelegramTexts.RequestSentMessage, parseMode: ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove());
                    await botService.SendMessage(chatId, TelegramTexts.SendNewRequest, parseMode: ParseMode.Markdown, replyMarkup: sendRequest);
                    user.State = State.submit_request;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();


                    var existuser = await userRepository.GetAll(x => x.ChatId == chatId).Include(x => x.Requests).FirstOrDefaultAsync();

                    var request = existuser?.Requests?.OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<CrmDBContext>();
                        var botRequest = new CrmRequest { ContactNumber = request.ContactNumber,
                            RequestStatusId = 4,
                            Date = DateTime.UtcNow.ToString("dd.MM.yyyy"),
                            Deadline = DateTime.UtcNow.AddDays(3),
                            ResponsiblePerson = request.ResponsiblePerson,
                            AdditionalInformation = request.AdditionalInformation,
                            ClientCompany = request.CompanyName,
                            Department = request.Department,
                            Email = request.Email,
                            InquirySource = request.InquirySource,
                            Notes = request.Notes, 
                            ProjectBudget = request.ProjectBudget,
                            Priority = ProjectManagement.Domain.Enum.Priority.Normal, 
                            Status = ProjectManagement.Domain.Enum.ProjectStatus.ToDO, 
                            ChatId = chatId};
                        dbContext.Requests.Add(botRequest);
                        await dbContext.SaveChangesAsync();
                    }

                    var verificationMessage = new EmailMessage();

                    verificationMessage = EmailMessage.SuccessSendRequest(request.Email, request.ResponsiblePerson);

                    emailInboxService.EnqueueEmail(verificationMessage);

                    return;
                case State.submit_request:
                    await botService.DeleteMessage(chatId, messageId: messageText.MessageId);

                    await botService.SendMessage(chatId, TelegramTexts.RequestFillingMessage, parseMode: ParseMode.Markdown, replyMarkup: sendRequest);
                    user.State = State.send_request;
                    user.UpdatedAt = DateTime.UtcNow;
                    userRepository.UpdateAsync(user);
                    await userRepository.SaveChangeAsync();
                    return;
                default:
                    break;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async ValueTask CreateRequest(bool isCreate, Users? user, string? state, string? message)
        {
            if (isCreate)
            {
                var request = new Requests
                {
                    ContactNumber = message,
                    UserId = user.Id
                };

                await requestsRepository.CreateAsync(request);
                await requestsRepository.SaveChangeAsync();
            }
            else
            {
                var lastRequest = user.Requests.OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                lastRequest.Email = state == State.send_email ? message : lastRequest.Email;
                lastRequest.CompanyName = state == State.send_company_name ? message : lastRequest.CompanyName;
                lastRequest.InquirySource = state == State.send_inquiry_source ? message : lastRequest.InquirySource;
                lastRequest.AdditionalInformation = state == State.send_information ? message : lastRequest.AdditionalInformation;
                lastRequest.ProjectBudget = state == State.send_project_salary ? message : lastRequest.ProjectBudget;
                lastRequest.ResponsiblePerson = state == State.send_full_name ? message : lastRequest.ResponsiblePerson;
                lastRequest.Department = state == State.send_position ? message : lastRequest.Department;
                lastRequest.Notes = state == State.send_inquiry_content ? message : lastRequest.Notes;

                requestsRepository.UpdateAsync(lastRequest);
                await requestsRepository.SaveChangeAsync();
            }
        }

        public async Task StartPollingAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Запуск Long Polling...");

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() 
            };

            botService.StartReceiving(
               updateHandler: async (botClient, update, ct) => await HandleUpdateAsync(update),
               errorHandler: async (botClient, exception, source, ct) => _logger.LogError(exception, "Ошибка в Telegram API"),
               receiverOptions: receiverOptions,
               cancellationToken: cancellationToken
           );

            _logger.LogInformation("Long Polling запущен.");
        }
    }
}
