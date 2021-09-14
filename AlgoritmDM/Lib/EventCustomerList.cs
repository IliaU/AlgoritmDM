using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Data.Lib;

namespace AlgoritmDM.Lib
{
    public class EventCustomerList : EventArgs
    {
        /// <summary>
        /// Текущий список клиентов
        /// </summary>
        public CustomerList CustL { get; private set; }

        /// <summary>
        /// Обрабатывать или нет
        /// </summary>
        public Boolean Action = true;

        /// <summary>
        /// Фильтр по клинету
        /// </summary>
        public CustomerBase Cst { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CustL">Текущий список клиентов</param>
        /// <param name="Cst">Фильтр по клинету по которому идёт работа</param>
        public EventCustomerList(CustomerList CustL, CustomerBase Cst)
        {
            this.CustL = CustL;
            this.Cst = Cst;
        }
    }
}
