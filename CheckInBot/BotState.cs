using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInBot
{
    public enum BotState
    {
        /// <summary>
        /// Базовое состояние
        /// </summary>
        None,
        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        Registration
    }
}
