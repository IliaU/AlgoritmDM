using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventSharedConfigurations : EventArgs
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        public ConfigurationList CfgL { get; private set; }

        /// <summary>
        /// Обрабатывать или нет
        /// </summary>
        public Boolean Action = true;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CfgL">Конфигурация</param>
        public EventSharedConfigurations(ConfigurationList CfgL)
        {
            this.CfgL = CfgL;
        }
    }
}
