using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventConfigurationListProcessingProgerssBar : EventArgs
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        public ConfigurationList CfgL { get; private set; }

        /// <summary>
        /// Всего шагов
        /// </summary>
        public int AllStep { get; private set; }

        /// <summary>
        ///  Текущий шаг
        /// </summary>
        public int CurrentStep { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CfgL">Конфигурация</param>
        public EventConfigurationListProcessingProgerssBar(ConfigurationList CfgL, int AllStep, int CurrentStep)
        {
            this.CfgL = CfgL;
            this.AllStep = AllStep;
            this.CurrentStep = CurrentStep;
        }
    }
}
