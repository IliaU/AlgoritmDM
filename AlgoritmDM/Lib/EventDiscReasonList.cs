using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventDiscReasonList : EventArgs
    {
        /// <summary>
        /// Текущий список причин скидок
        /// </summary>
        public DiscReasonList DscReasL { get; private set; }

        /// <summary>
        /// Обрабатывать или нет
        /// </summary>
        public Boolean Action = true;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="DscReasL">Текущий список причин скидок</param>
        public EventDiscReasonList(DiscReasonList DscReasL)
        {
            this.DscReasL = DscReasL;
        }
    }
}
