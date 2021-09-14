using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventCustDetailCheckAsicRez : EventArgs
    {
        /// <summary>
        /// Объект представляющий результат
        /// </summary>
        public UProvider.CustDetailCheck CstDetailChk { get; private set; }

        /// <summary>
        /// Исключение которое получили при выполнении
        /// </summary>
        public ApplicationException ex { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CstDetailChk">Объект представляющий результат</param>
        /// <param name="ex">Исключение которое получили при выполнении</param>
        public EventCustDetailCheckAsicRez(UProvider.CustDetailCheck CstDetailChk, ApplicationException ex)
        {
            this.CstDetailChk = CstDetailChk;
            this.ex = ex;
        }
    }
}
