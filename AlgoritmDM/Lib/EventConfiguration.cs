using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventConfiguration : EventArgs
    {
        /// <summary>
        /// Элемент конфигурации
        /// </summary>
        public Configuration Cfg { get; private set; }

        /// <summary>
        /// Обрабатывать или нет
        /// </summary>
        public Boolean Action = true;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Cfg">Элемент конфигурации</param>
        public EventConfiguration(Configuration Cfg)
        {
            this.Cfg = Cfg;
        }
    }
}
