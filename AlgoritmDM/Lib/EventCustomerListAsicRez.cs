using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Data.Lib;

namespace AlgoritmDM.Lib
{
    public class EventCustomerListAsicRez : EventArgs
    {
        /// <summary>
        /// Текущий список клиентов
        /// </summary>
        public CustomerList CustL { get; private set; }

        /// <summary>
        /// Исключение которое получили при выполнении
        /// </summary>
        public ApplicationException ex { get; private set; }

        /// <summary>
        /// Фильтр по клинету
        /// </summary>
        public CustomerBase Cst { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="CustL">Текущий список клиентов</param>
        /// <param name="Cst">Фильтр по клинету по которому идёт работа</param>
        /// <param name="ex">Исключение которое получили при выполнении</param>
        public EventCustomerListAsicRez(CustomerList CustL, CustomerBase Cst, ApplicationException ex)
        {
            this.CustL = CustL;
            this.Cst = Cst;
            this.ex = ex;
        }
    }
}
