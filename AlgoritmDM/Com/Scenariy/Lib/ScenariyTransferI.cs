using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Com.Data;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com.Scenariy.Lib
{
    /// <summary>
    /// Интерфейс для всех сценариев кроме универсального, чтобы заставить их реализовать методы для обработки полученных данных
    /// </summary>
    public interface ScenariyTransferI
    {
        /// <summary>
        /// Прокачивание чека по цепочке сценариев
        /// </summary>
        /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
        /// <param name="Chk">Чек который был на входе</param>
        /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
        /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
        /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
        /// <returns>Успех обработки функции</returns>
        bool transfCheck(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, Check Chk, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate);
    }
}
