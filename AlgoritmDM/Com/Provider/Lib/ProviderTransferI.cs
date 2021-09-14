using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Data;
using AlgoritmDM.Com.Data.Lib;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com.Provider.Lib
{
    /// <summary>
    /// Интерфейс для всех провайдеров кроме универсального, чтобы заставить их реализовать методы для выкачивания или закачивания данных
    /// </summary>
    public interface ProviderTransferI
    {
        /// <summary>
        /// Выкачивание чеков
        /// </summary>
        /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
        /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
        /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
        /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
        /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
        /// <returns>Успех обработки функции</returns>
        bool getCheck(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate, long? FilCustSid);

        /// <summary>
        /// Заполнение справочника текущих пользователей
        /// </summary>
        /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
        /// <returns>Успех обработки функции</returns>
        bool getCustumers(Func<Customer, bool> FuncTarget);

        /// <summary>
        /// Заполнение справочника причин скидок
        /// </summary>
        /// <returns>Успех обработки функции</returns>
        bool getDiscReasons();

        /// <summary>
        /// Установка расчитанной скидки в базе у конкртеного клиента
        /// </summary>
        /// <param name="Cst">Клиент</param>
        /// <param name="CalkMaxDiscPerc">Процент скидки который мы устанавливаем</param>
        /// <returns>Успех обработки функции</returns>
        bool AployDMCalkMaxDiscPerc(CustomerBase Cst, decimal CalkMaxDiscPerc);

        /// <summary>
        /// Установка бонусного бала в базе у конкртеного клиента
        /// </summary>
        /// <param name="Cst">Клиент</param>
        /// <param name="CalkStoreCredit">Бонусный бал который мы устанавливаем</param>
        /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
        /// <param name="OldCalkStoreCredit">Старый бонусный бал который мы устанавливаем</param>
        /// <param name="OldCalcScPerc">Старый процент по которому считался бонусный бал который мы устанавливаем</param>
        /// <returns>Успех обработки функции</returns>
        bool AployDMCalkStoreCredit(CustomerBase Cst, decimal CalkStoreCredit, decimal CalcScPerc, decimal OldCalkStoreCredit, decimal OldCalcScPerc);
    }
}
