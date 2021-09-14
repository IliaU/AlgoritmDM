using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Список специальных настроек которые существуют в нашей системе. Передаются по имени как уникальные от любого сценария
    /// </summary>
    public sealed class ConfigurationParams : EventArgs
    {
        private static ConfigurationParams MyObj;
        private static List<string[]> CfgL;

        /// <summary>
        /// Получение экземпляра нашего синглетон
        /// </summary>
        /// <returns></returns>
        public static ConfigurationParams GetInstatnce()
        {
            if (MyObj == null)
            {
                MyObj = new ConfigurationParams();
                CfgL = new List<string[]>();
            }
            return MyObj;
        }

        /// <summary>
        /// Количчество объектов в контейнере
        /// </summary>
        public int Count
        {
            get
            {
                int rez;
                lock (CfgL)
                {
                    rez = CfgL.Count;
                }
                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает конфигурацию по её имени в эом контернере
        /// </summary>
        /// <param name="i">ConfigurationName</param>
        /// <returns>Конфигурация</returns>
        public string this[string s]
        {
            get
            {
                string rez = null;
                lock (MyObj)
                {
                    lock (CfgL)
                    {
                        // Проверка на наличие этого элемента в списке
                        for (int i = 0; i < CfgL.Count; i++)
                        {
                            string[] item = CfgL[i];
                            if (item[0] == s)
                            {
                                return item[1];
                            }
                        }
                    }
                }
                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Добавить параметр конфигурации
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public bool Add(string Name, string Value, bool HashExeption)
        {
            bool rez = false;

            try
            {
                string[] newCfg = { Name, Value };

                lock (CfgL)
                {
                    // Проверка на наличие этого элемента в списке
                    for (int i = 0; i < CfgL.Count; i++)
                    {
                        string[] item = CfgL[i];
                        if (item[0] == Name)
                        {
                            CfgL[i] = newCfg;
                            return true;
                        }
                    }
                    

                    CfgL.Add(newCfg);
                    rez = true;
                }
            }
            catch (Exception ex)
            {
                if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить элемент параметра {0} произошла ошибка: {1}", Name, ex.Message));
            }
            return rez;
        }

        /// <summary>
        /// Для обращения по индексатору
        /// </summary>
        /// <returns>Возвращаем стандарнтый индексатор</returns>
        public IEnumerator GetEnumerator()
        {
            IEnumerator rez = null;
            lock (CfgL)
            {
                rez = CfgL.GetEnumerator();
            }
            return rez;
        }
    }
}
