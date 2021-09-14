using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class EventCustDetailCheck : EventArgs
    {
        /// <summary>
        /// Объект представляющий результат
        /// </summary>
        public UProvider.CustDetailCheck CstDetailChk { get; private set; }

        /// <summary>
        /// Обрабатывать или нет
        /// </summary>
        public Boolean Action = true;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CstDetailChk">Объект представляющий результат</param>
        public EventCustDetailCheck(UProvider.CustDetailCheck CstDetailChk)
        {
            this.CstDetailChk = CstDetailChk;

        }
    }
}
