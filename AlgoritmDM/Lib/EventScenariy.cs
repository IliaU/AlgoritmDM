using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventScenariy : EventArgs
    {
        /// <summary>
        /// Сценарий
        /// </summary>
        public UScenariy UScn { get; private set; }

        /// <summary>
        /// Обрабатывать или нет
        /// </summary>
        public Boolean Action = true;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="UScn">Сценарий</param>
        public EventScenariy(UScenariy UScn)
        {
            this.UScn = UScn;
        }
    }
}
