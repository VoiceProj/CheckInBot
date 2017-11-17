using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CheckInBot
{
    class Bot
    {
        /// <summary>
        /// Клиент бота Telegram
        /// </summary>
        private TelegramBotClient client;
        Storage.CheckInData DB;
        Dictionary<int, BotState> states;
        List<Storage.CheckIn> checkIns;
        List<Storage.Registration> students;
        /// <summary>
        /// База данных
        /// </summary>

        /// <summary>
        /// Конструктор
        /// </summary>
        public Bot()
        {
            string token = Properties.Settings.Default.Token;
            client = new TelegramBotClient(token);
            client.OnMessage += MessageProcessor;
            DB = new Storage.CheckInData();
            states = new Dictionary<int, BotState>();
            checkIns = DB.CheckInDB.ToList();
            students = DB.RegistrationDB.ToList();
        }

        /// <summary>
        /// Запуск бота
        /// </summary>
        public void Start()
        {
            client.StartReceiving();
        }

        /// <summary>
        /// Останов бота
        /// </summary>
        public void Stop()
        {
            client.StopReceiving();
        }

        /// <summary>
        /// Обработка получаемых сообщений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageProcessor(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            // Проверка типа сообщения
            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.TextMessage:
                    TextProcessor(e.Message);
                    break;

                case Telegram.Bot.Types.Enums.MessageType.LocationMessage:
                    LocationProcessor(e.Message);
                    break;

                default:
                    client.SendTextMessageAsync(e.Message.Chat.Id, "Непонятный тип сообщения: " + e.Message.Type);
                    break;
            }
        }

        /// <summary>
        /// Обработка полученных геоданных
        /// </summary>
        /// <param name="message"></param>
        private void LocationProcessor(Message message)
        {
            Location loc = message.Location;

            // Проверка попадания в определенный квадрат, примерно +-1км
            double longMin, longMax, latMin, latMax, targetLong= 37.683528, targetLat = 55.752890;
            latMin = targetLat - 0.007;
            latMax = targetLat + 0.007;
            longMin = targetLong - 0.01;
            longMax = targetLong + 0.01;

            // "Yes" если человек во время чекина внутри квадрата, "No" если нет
            string isHere = (loc.Latitude > latMin && loc.Latitude < latMax && loc.Longitude > longMin && loc.Longitude < longMax) ? "Yes" : "No";

            // ID нового чекина
            var newID = (checkIns.Count() > 0) ? checkIns.Last().ID : 0;

            // Запись нового чекина
            Storage.CheckIn new_checkin = new Storage.CheckIn
            {
                ID = ++newID,
                UserName = message.From.Username,
                Name = message.From.FirstName,
                LastName = message.From.LastName,
                UserID = message.From.Id,
                TimeStamp = message.Date,
                Longitude = loc.Longitude,
                Latitude = loc.Latitude,
                IsHere = isHere
            };
            DB.CheckInDB.Add(new_checkin);
            DB.SaveChanges();

            // Обновление списка чекинов из БД
            checkIns = DB.CheckInDB.ToList();

        }

        /// <summary>
        /// Обработка текстовых сообщений
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private void TextProcessor(Message msg)
        {
            if (msg.Text.Substring(0, 1) == "/")
            {
                CommandProcessor(msg, msg.Text.Substring(1));
            }
            else
            {
                BotState s = states.ContainsKey(msg.From.Id) ? states[msg.From.Id] : BotState.None;
                switch (s)
                {
                    case BotState.None:
                        string message = string.Format("Привет, {0}!", msg.From.FirstName);
                        client.SendTextMessageAsync(msg.Chat.Id, message);
                        break;
                    case BotState.Registration:
                        bool emailFound = false;
                        foreach (var entity in msg.Entities)
                        {
                            if (entity.Type == Telegram.Bot.Types.Enums.MessageEntityType.Email)
                            {
                                var newID = (students.Count() > 0) ? students.Last().ID : 0;
                                //Регистрация пользователя
                                Storage.Registration newUser = new Storage.Registration()
                                {
                                    Email = msg.Text.Substring(entity.Offset, entity.Length),
                                    UserID = msg.From.Id,
                                    ID = ++newID
                                };
                                DB.RegistrationDB.Add(newUser);
                                DB.SaveChanges();
                                // Обновление списка регистраций из БД
                                students = DB.RegistrationDB.ToList();
                                message = string.Format("Спасибо, {0}, регистрация выполнена!", msg.From.FirstName);
                                client.SendTextMessageAsync(msg.Chat.Id, message);
                                emailFound = true;
                                states[msg.From.Id] = BotState.None;
                                break;
                            }
                        }
                        if (!emailFound)
                        {
                            message = string.Format("{0}, для завершения регистрации отправьте, пожалуйста, E-mail", msg.From.FirstName);
                            client.SendTextMessageAsync(msg.Chat.Id, message);
                        }
                        break;

                }
                
            }
        }

        /// <summary>
        /// Обработка текстовой команды
        /// </summary>
        /// <param name="msg">Сообщение</param>
        /// <param name="cmd">Команда</param>
        private void CommandProcessor(Message msg, string cmd)
        {
            bool isListed = false;
            foreach (var s in students)
            {
                if (s.UserID == msg.From.Id)
                    isListed = true;
            }
            // Селектор команд без учета регистра букв
            switch (cmd.ToLower())
            {
                case "checkin":                    
                    if (isListed)
                    {
                        // Кнопка для регистрации
                        KeyboardButton locationBtn = new KeyboardButton("Отметиться на карте")
                        {
                            RequestLocation = true
                        };
                        // Панель кнопок
                        KeyboardButton[] keys = new KeyboardButton[1] { locationBtn };
                        // Разметка ответа
                        var markup = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(keys, true, true);
                        // Ответ
                        client.SendTextMessageAsync(msg.Chat.Id, "Предоставьте данные для отметки", replyMarkup: markup);
                    }
                    else
                        client.SendTextMessageAsync(msg.Chat.Id, "Вы еще не зарегистрированы, введите /register и пройдите процедуру регистрации");
                    break;

                case "register":
                    if (!isListed)
                    {
                        client.SendTextMessageAsync(msg.Chat.Id, "Для регистрации отправьте мне Email");
                        states[msg.From.Id] = BotState.Registration;
                    }
                    else
                        client.SendTextMessageAsync(msg.Chat.Id, "Вы уже зарегистрированы");
                    break;

                case "start":
                    client.SendTextMessageAsync(msg.Chat.Id, "Приветствую! Если вы еще не зарегистрированы - напишите /register");
                    break;
                default:
                    client.SendTextMessageAsync(msg.Chat.Id, "Неизвестная команда: " + cmd);
                    break;
            }
        }
    }
}
