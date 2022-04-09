using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using AlgoritmDM.Com.Scenariy.Lib;
using AlgoritmDM.Com.Provider.Lib;
using AlgoritmDM.Com.Data.Lib;
using AlgoritmDM.Lib;

namespace AlgoritmDM.Com.Data.Lib
{
    /// <summary>
    /// Базовый класс нашего клиента
    /// </summary>
    public class CustomerBase
    {
        /// <summary>
        /// Индекс элемента в списке клиентов
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Тип источника откуда пришли данные
        /// </summary>
        EnSourceType SourceType = EnSourceType.Retail;

        /// <summary>
        /// Id покупателя
        /// </summary>
        public long CustSid;

        /// <summary>
        /// Id покупателя из второй базы типа Prizm
        /// </summary>
        public long CustSidPrizm;

        /// <summary>
        /// Имя Отчество
        /// </summary>
        public string FirstName;

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName;

        /// <summary>
        /// Номер карты
        /// </summary>
        public string CustId;

        /// <summary>
        /// Максимальный процент скидки
        /// </summary>
        public decimal MaxDiscPerc { get; private set; }

        /// <summary>
        /// Бонусные баллы которыми может расплотиться покупатель
        /// </summary>
        public decimal StoreCredit { get; private set; }

        /// <summary>
        /// Процент по которому считался бонусный бал который мы устанавливаем
        /// </summary>
        public decimal ScPerc { get; private set; }

        /// <summary>
        /// Рассчитанная скидка
        /// </summary>
        public decimal? CalkMaxDiscPerc { get; private set; }

        /// <summary>
        /// Рассчитанные бонусные баллы
        /// </summary>
        public decimal? CalkStoreCredit { get; private set; }

        /// <summary>
        /// Рассчитанные процент по которому считался бонусный бал который мы устанавливаем
        /// </summary>
        public decimal? CalkScPerc { get; private set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string Phone1;

        /// <summary>
        /// Адресс
        /// </summary>
        public string Address1;

        /// <summary>
        /// Первая покупка
        /// </summary>
        public DateTime? FstSaleDate;

        /// <summary>
        /// Последняя покупка
        /// </summary>
        public DateTime? LstSaleDate;

        /// <summary>
        /// Электронная почта
        /// </summary>
        public string EmailAddr;

        /// <summary>
        /// Активность клиента
        /// </summary>
        public int Active;

        /// <summary>
        /// Старндартные данные сценариев
        /// </summary>
        public ScenariyDataBase.ScenaryBaseList ScnDataList { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public CustomerBase()
        {
            this.Index = -1;
            this.ScnDataList = new ScenariyDataBase.ScenaryBaseList();
        }

        /// <summary>
        /// Инициализация базовых объектов
        /// </summary>
        /// <param name="SourceType">Тип источника откуда пришли данные</param>
        /// <param name="CustSid">Id покупателя</param>
        /// <param name="CustSidPrizm">Id покупателя из второй базы типа Prizm</param>
        /// <param name="FirstName">Имя Отчество</param>
        /// <param name="LastName">Фамилия</param>
        /// <param name="CustId">Номер карты</param>
        /// <param name="MaxDiscPerc">Максимальный процент скидки</param>
        /// <param name="StoreCredit">Бонусные баллы которыми может расплотиться покупатель</param>
        /// <param name="ScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
        /// <param name="Phone1">Номер телефона</param>
        /// <param name="Address1">Адресс</param>
        /// <param name="FstSaleDate">Первая покупка</param>
        /// <param name="LstSaleDate">Последняя покупка</param>
        /// <param name="EmailAddr">Электронная почта</param>
        /// <param name="Active">Активность клиента</param>
        protected void InitBaseObject(EnSourceType SourceType, long CustSid, long CustSidPrizm, string FirstName, string LastName, string CustId, decimal MaxDiscPerc, decimal StoreCredit, decimal ScPerc, string Phone1, string Address1, DateTime? FstSaleDate, DateTime? LstSaleDate, string EmailAddr, int Active)
        {
            this.SourceType = SourceType;
            this.CustSid = CustSid;
            this.CustSidPrizm = CustSidPrizm;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.CustId = CustId;
            this.MaxDiscPerc = MaxDiscPerc;
            this.StoreCredit = StoreCredit;
            this.ScPerc = ScPerc;
            this.Phone1 = Phone1;
            this.Address1 = Address1;
            this.FstSaleDate = FstSaleDate;
            this.LstSaleDate = LstSaleDate;
            this.EmailAddr = EmailAddr;
            this.Active = Active;

            this.Index = -1;
            this.ScnDataList = new ScenariyDataBase.ScenaryBaseList();
        }


        /// <summary>
        /// Класс для доступа к объекту для хранения доп инфориаци которую пишут сценрарии относительно клиента
        /// </summary>
        public class AccessForScenary
        {
            /// <summary>
            /// Сценарий с которым мы работаем
            /// </summary>
            private ScenariyBase ScnB;

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="ScnB">Ссылка на сценарий</param>
            public AccessForScenary(ScenariyBase ScnB)
            {
                this.ScnB = ScnB;
            }

            /// <summary>
            /// Получение объекта для хранения инфы нашего сценария
            /// </summary>
            /// <param name="CustB">Клиент с которым будем работать</param>
            /// <returns></returns>
            public ScenariyDataBase getScenariyDataBase(CustomerBase CustB)
            {
                ScenariyDataBase rez= null;

                // Пытаемся искать нужный контекст если вообще список контекстов существует
                if (CustB.ScnDataList != null)
                {
                    foreach (ScenariyDataBase item in CustB.ScnDataList)
                    {
                        if (item.ScnB.ScenariyName == this.ScnB.ScenariyName) rez = item;
                    }
                }
                return rez;     // если объект не найден то возвращаем null. Тогда сценарий сам создаст свой собственный объект отнаследованый от базового и пердаст в этот список.
            }

            /// <summary>
            /// Установка новых данных сценария
            /// </summary>
            /// <param name="CustB">Клиент в контекст которого мы устанавливаем данные</param>
            /// <param name="ScnDB">Данные которые мы устанавливаем можно отнаследовать и засунуть сто угодно</param>
            /// <returns></returns>
            public bool setScenariyDataBase(CustomerBase CustB, ScenariyDataBase ScnDB)
            {
                try
                {
                    ScenariyDataBase rez = null;

                    // Пытаемся найти сценарий в списке только если его не существует
                    if (CustB.ScnDataList != null)
                    {
                        foreach (ScenariyDataBase item in CustB.ScnDataList)
                        {
                            if (item.ScnB.ScenariyName == this.ScnB.ScenariyName) rez = item;
                        }
                    }

                    if (CustB.ScnDataList == null) CustB.ScnDataList = new ScenariyDataBase.ScenaryBaseList();

                    // Если объект существует то его заменяем новым если нет то просто добавляем
                    if (rez != null) CustB.ScnDataList.Update(ScnDB, true);
                    else CustB.ScnDataList.Add(ScnDB, true);

                    return true;

                }
                catch (Exception ae)
                {
                    Com.Log.EventSave(ae.Message, GetType().Name + ".setScenariyDataBase", EventEn.Error);
                    throw;
                }
            }

            /// <summary>
            /// Установка новой подсчитанной скидки
            /// </summary>
            /// <param name="CustB">Клиент с которым будем работать</param>
            /// <param name="CalkMaxDiscPerc">Максимальный процент скидки который установим указанному клиенту</param>
            public void SetupCalkMaxDiscPerc(CustomerBase CustB, decimal? CalkMaxDiscPerc)
            {
                CustB.CalkMaxDiscPerc = CalkMaxDiscPerc;
            }

            /// <summary>
            /// Установка новых бонусных баллов у клиента
            /// </summary>
            /// <param name="CustB">Клиент с которым будем работать</param>
            /// <param name="CalkStoreCredit">Бонусные баллы который устанавливаем клиенту</param>
            /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
            public void SetupCalkStoreCredit(CustomerBase CustB, decimal? CalkStoreCredit, decimal? CalkScPerc)
            {
                CustB.CalkStoreCredit = CalkStoreCredit;
                CustB.CalkScPerc = CalkScPerc;
            }
        }

        /// <summary>
        /// Класс через который будет иметь тоступ конфигурация к элементам клиента
        /// </summary>
        public class AccessForConfigurationList
        {
            /// <summary>
            /// Ссылка на конфигурацию, которой был предоставлен доступ
            /// </summary>
            private ConfigurationBase.ConfigurationBaseList CnfBL;

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="CnfBL">Ссылка конфигурации которой мы предостави доступ</param>
            public AccessForConfigurationList(ConfigurationBase.ConfigurationBaseList CnfBL)
            {
                this.CnfBL = CnfBL;
            }

            /// <summary>
            /// Отчистка от данных сценариев, которые хранятся в контексте клиентов
            /// </summary>
            /// <param name="CustB">Клиент с которым будем работать</param>
            public void ClearScnDataList(CustomerBase CustB)
            {
                CustB.ScnDataList = null;
            }

            /// <summary>
            /// Установка новой подсчитанной скидки
            /// </summary>
            /// <param name="CustB">Клиент с которым будем работать</param>
            /// <param name="CalkMaxDiscPerc">Максимальный процент скидки который установим указанному клиенту</param>
            public void SetupCalkMaxDiscPerc(CustomerBase CustB, decimal? CalkMaxDiscPerc)
            {
                CustB.CalkMaxDiscPerc = CalkMaxDiscPerc;
            }

            /// <summary>
            /// Установка новых бонусных баллов у клиента
            /// </summary>
            /// <param name="CustB">Клиент с которым будем работать</param>
            /// <param name="CalkStoreCredit">Бонусные баллы который устанавливаем клиенту</param>
            /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
            public void SetupCalkStoreCredit(CustomerBase CustB, decimal? CalkStoreCredit, decimal? CalkScPerc)
            {
                CustB.CalkStoreCredit = CalkStoreCredit;
                CustB.CalkScPerc = CalkScPerc;
            }
        }



        /// <summary>
        /// Базовый класс для компонента списка клиентов
        /// </summary>
        public abstract class CustomerBaseList : IEnumerable
        {
            /// <summary>
            /// Внутренний список 
            /// </summary>
            private static List<CustomerBase> CustL = new List<CustomerBase>();

            /// <summary>
            /// Количчество объектов в контейнере
            /// </summary>
            public int Count
            {
                get
                {
                    int rez;
                    lock (CustL)
                    {
                        rez = CustL.Count;
                    }
                    return rez;
                }
                private set { }
            }

            /// <summary>
            /// Отчистка списка клиентов
            /// </summary>
            /// <param name="HashExeption"></param>
            /// <returns></returns>
            protected bool Clear(bool HashExeption)
            {
                bool rez = false;

                try
                {
                    lock (CustL)
                    {
                        CustL.Clear();
                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не смогли отчистить список клиентов произошла ошибка: {0}", ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Добавление нового клиента
            /// </summary>
            /// <param name="newCust">Клиент которого нужно добавить в список</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Add(CustomerBase newCust, bool HashExeption)
            {
                bool rez = false;

                try
                {
                    bool flag = false;
                    lock (CustL)
                    {
                        foreach (CustomerBase item in CustL)
                        {
                            if (item.CustSid == newCust.CustSid) flag = true;
                        }
                    }
                    if (!flag)
                    {
                        lock (CustL)
                        {
                            newCust.Index = CustL.Count;
                            CustL.Add(newCust);
                            rez = true;
                        }
                    }
                    else
                    {
                        if (HashExeption) throw new ApplicationException(string.Format("Клиент {0} с таким сидом {1} или номером карты уже существует.", newCust.FirstName, newCust.CustSid.ToString()));
                    }
                    
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить клиента {0} с таким сидом {1} произошла ошибка: {2}", newCust.FirstName, newCust.CustSid.ToString(), ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Удаление клиента
            /// </summary>
            /// <param name="delCust">Клиент которого нужно удалить из списка</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Remove(CustomerBase delCust, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (CustL)
                    {
                        int delIndex = delCust.Index;
                        CustL.RemoveAt(delIndex);

                        for (int i = delIndex; i < CustL.Count; i++)
                        {
                            CustL[i].Index = i;
                        }

                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить клиента {0} с таким сидом {1} произошла ошибка: {2}", delCust.FirstName, delCust.CustSid.ToString(), ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Обновление данных клиента. Происходит подмена по указанному индексу
            /// </summary>
            /// <param name="ubpIndex">Индекс клиента который нужно подменить</param>
            /// <param name="updCust">Пользователь у которого нужно изменить данные</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Update(int ubpIndex, CustomerBase updCust, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    ApplicationException eNotFound=null;
                    lock (CustL)
                    {
                        if (ubpIndex < CustL.Count && ubpIndex >=0)
                        {
                            updCust.Index = ubpIndex;
                            CustL[ubpIndex] = updCust;

                            rez = true;
                        }
                        else
                        {
                            eNotFound = new ApplicationException(string.Format("Не удалось обновить данные индекса {0} в списке клиентов не найдено.", ubpIndex.ToString()));
                        }
                    }
                    if (eNotFound != null && HashExeption) throw eNotFound;
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить клиента {0} с таким сидом {1} произошла ошибка: {2}", updCust.FirstName, updCust.CustSid.ToString(), ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Получение компонента по его ID
            /// </summary>
            /// <param name="i">Введите идентификатор</param>
            /// <returns></returns>
            protected CustomerBase getCustComponent(int i) 
            {
                CustomerBase rez;
                lock (CustL)
                {
                    rez = CustL[i];
                }
                return rez; 
            }

            /// <summary>
            /// Установка новой скидки
            /// </summary>
            /// <param name="CustB">Клиент с которым будем работать</param>
            /// <param name="MaxDiscPerc">Максимальный процент скидки который установим указанному клиенту</param>
            protected void SetupMaxDiscPerc(CustomerBase CustB, decimal MaxDiscPerc)
            {
                CustB.MaxDiscPerc = MaxDiscPerc;
            }

            /// <summary>
            /// Установка новых бонусных баллов у клиента
            /// </summary>
            /// <param name="CustB">Клиент с которым будем работать</param>
            /// <param name="CalkStoreCredit">Бонусные баллы который устанавливаем клиенту</param>
            /// <param name="ScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
            public void SetupStoreCredit(CustomerBase CustB, decimal StoreCredit, decimal ScPerc)
            {
                CustB.StoreCredit = StoreCredit;
                CustB.ScPerc = ScPerc;
            }

            /// <summary>
            /// Для обращения по индексатору
            /// </summary>
            /// <returns>Возвращаем стандарнтый индексатор</returns>
            public IEnumerator GetEnumerator()
            {
                IEnumerator rez;
                lock (CustL)
                {
                    rez=CustL.GetEnumerator();
                }
                return rez;
            }

        }
    }
}
