using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventDiscReasonListAsicRez : EventArgs
    {
        /// <summary>
        /// Текущий список причин скидок
        /// </summary>
        public DiscReasonList DscReasL { get; private set; }

        /// <summary>
        /// Исключение которое получили при выполнении
        /// </summary>
        public ApplicationException ex { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CustL">Текущий список причин скидок</param>
        /// <param name="ex">Исключение которое получили при выполнении</param>
        public EventDiscReasonListAsicRez(DiscReasonList DscReasL, ApplicationException ex)
        {
            this.DscReasL = DscReasL;
            this.ex = ex;
        }
    }
}
