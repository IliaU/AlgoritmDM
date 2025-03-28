﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using AlgoritmDM.Lib;
using AlgoritmDM.Com.Data;


namespace AlgoritmDM.Com.Provider.Lib
{
    /// <summary>
    /// Инитерфейс для всех провайдеров
    /// </summary>
    public interface ProviderI
    {
        /// <summary>
        /// Получение версии базы данных
        /// </summary>
        /// <returns>Возвращает версию базы данных в виде строки</returns>
        //string VersionDB();

        /// <summary>
        /// Процедура вызывающая настройку подключения
        /// </summary>
        /// <returns>Возвращает значение требуется ли сохранить подключение как основное или нет</returns>
        bool SetupConnectDB();

        /// <summary>
        /// Печать строки подключения с маскировкой секретных данных
        /// </summary>
        /// <returns>Строка подклюения с замасированной секретной информацией</returns>
        string PrintConnectionString();

        /// <summary>
        /// Объединение клиентов
        /// </summary>
        /// <param name="MergeClientMain">Основной клиент</param>
        /// <param name="MergeClientDonors">Клинеты доноры</param>
        void MergeClient(Customer MergeClientMain, List<Customer> MergeClientDonors);

        /// <summary>
        /// Установка дефолтного значения для ретейла из призма
        /// </summary>
        /// <param name="CustSid">Сид пользователя</param>
        /// <param name="DefaultCallOffSc">Значение которое выставляем в поле CallOffScDef</param>
        void SetCustomerDefaultCallOffSc(long CustSid, decimal DefaultCallOffSc);
    }
}
