using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Data.Lib;

namespace AlgoritmDM.Lib
{
    public class EventCustomerListProcessingProgerssBar : EventArgs
    {
        /// <summary>
        /// Список клиентов
        /// </summary>
        public CustomerList CstL { get; private set; }

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
        /// <param name="CstL">Список клиентов</param>
        public EventCustomerListProcessingProgerssBar(CustomerList CstL, int AllStep, int CurrentStep)
        {
            this.CstL = CstL;
            this.AllStep = AllStep;
            this.CurrentStep = CurrentStep;
        }
    }
}
