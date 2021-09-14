using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Data.Lib;

namespace AlgoritmDM.Lib
{
    public class EventConfigurationListAsicRez : EventArgs
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        public ConfigurationList CfgL { get; private set; }

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
        /// <param name="CfgL">Конфигурация</param>
        /// <param name="ex">Исключение которое получили при выполнении</param>
        /// <param name="Cst">Фильтр по клинету по которому идёт работа</param>
        public EventConfigurationListAsicRez(ConfigurationList CfgL, ApplicationException ex, CustomerBase Cst)
        {
            this.CfgL = CfgL;
            this.ex = ex;
            this.Cst = Cst;
        }
    }
}
