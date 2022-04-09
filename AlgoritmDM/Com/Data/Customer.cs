using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Lib;

namespace AlgoritmDM.Com.Data
{
    /// <summary>
    /// Представляет из себя клинета
    /// </summary>
    public sealed  class Customer : Lib.CustomerBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="SourceType">Тип источника откуда пришли данные</param>
        /// <param name="CustSid">Id покупателя</param>
        /// <param name="CustSidPrizm">Id покупателя из второй базы типа Prizm</param>
        /// <param name="FirstName">Имя Отчество</param>
        /// <param name="LastName">Фамилия</param>
        /// <param name="CustId">Номер карты</param>
        /// <param name="MaxDiscPerc">Максимальный процент скидки</param>
        /// <param name="StoreCredit">Бонусные баллы которыми может расплотиться покупатель</param>
        /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
        /// <param name="Phone1">Номер телефона</param>
        /// <param name="Address1">Адресс</param>
        /// <param name="FstSaleDate">Первая покупка</param>
        /// <param name="LstSaleDate">Последняя покупка</param>
        /// <param name="EmailAddr">Электронная почта</param>
        /// <param name="Active">Активность клиента</param>
        public Customer(EnSourceType SourceType, long CustSid, long CustSidPrizm, string FirstName, string LastName, string CustId, decimal MaxDiscPerc, decimal StoreCredit, decimal CalcScPerc, string Phone1, string Address1, DateTime? FstSaleDate, DateTime? LstSaleDate, string EmailAddr, int Active)
        {
            base.InitBaseObject(SourceType, CustSid, CustSidPrizm, FirstName, LastName, CustId, MaxDiscPerc, StoreCredit, CalcScPerc, Phone1, Address1, FstSaleDate, LstSaleDate, EmailAddr, Active);
        }
    }
}
