using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace AlgoritmDM.Com.Scenariy.Lib
{
    /// <summary>
    /// Базовый объект для хранения данных прренадлежащих сценарию
    /// </summary>
    public class ScenariyDataBase : EventArgs
    {
        /// <summary>
        /// Индекс элемента в списке конфигурации
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Ссылка на сценарий которому принадлежит этот элемент
        /// </summary>
        public ScenariyBase ScnB { get; private set; }

        /// <summary>
        /// Максимальный процент скидки
        /// </summary>
        public decimal? CalcMaxDiscPerc;

        /// <summary>
        /// Процент используемый при начислении баллов по бонусной программе
        /// </summary>
        public decimal? CalcScPerc;

        /// <summary>
        /// Бонусные баллы, которыми может расплатится клиент
        /// </summary>
        public decimal? CalcStoreCredit;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ScnB">Ссылка на сценарий которому принадлежит этот элемент</param>
        public ScenariyDataBase(ScenariyBase ScnB)
        {
            this.SetupBase(ScnB);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ScenariyDataBase() :this(null)
        { }

        /// <summary>
        /// Установка базовых объектов
        /// </summary>
        /// <param name="ScnB">Ссылка на сценарий которому принадлежит этот элемент</param>
        protected void SetupBase(ScenariyBase ScnB)
        {
            this.Index = -1;
            this.ScnB = ScnB;
        }

        /// <summary>
        /// Базовый класс для списка объект для хранения данных прренадлежащих сценарию
        /// </summary>
        public class ScenaryBaseList : IEnumerable
        {
            /// <summary>
            /// Внутренний список 
            /// </summary>
            private List<ScenariyDataBase> ScnDBL = new List<ScenariyDataBase>();

            /// <summary>
            /// Количчество объектов в контейнере
            /// </summary>
            public int Count { get { return this.ScnDBL.Count; } private set { } }

            /// <summary>
            /// Вытаскивает элемент по его индексу в эом контернере
            /// </summary>
            /// <param name="i">Индекс</param>
            /// <returns>Елемент</returns>
            public ScenariyDataBase this[int i]
            {
                get { return ((ScenariyDataBase)this.ScnDBL[i]); }
                private set { }
            }

            /// <summary>
            /// Вытаскивает элемент по имени сценария
            /// </summary>
            /// <param name="s">Индекс</param>
            /// <returns>Елемент</returns>
            public ScenariyDataBase this[string s]
            {
                get
                {
                    foreach (ScenariyDataBase item in this.ScnDBL)
                    {
                        if (item.ScnB.ScenariyName == s) return item;
                    }
                    return null;
                }
                private set { }
            }

            /// <summary>
            /// Добавление нового объект для хранения данных прренадлежащих сценарию
            /// </summary>
            /// <param name="newScnDB">Объект для хранения данных который нужно добавить в список</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            public bool Add(ScenariyDataBase newScnDB, bool HashExeption)
            {
                bool rez = false;

                try
                {
                    lock (this.ScnDBL)
                    {
                        bool flag = false;

                        foreach (ScenariyDataBase item in this.ScnDBL)
                        {
                            if (item.ScnB.ScenariyName == newScnDB.ScnB.ScenariyName) flag = true;
                        }

                        if (!flag)
                        {
                            newScnDB.Index = this.ScnDBL.Count;
                            this.ScnDBL.Add(newScnDB);
                            rez = true;
                        }
                        else
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Сценарий с таким именем {0} в этом списке уже существует.", newScnDB.ScnB.ScenariyName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить объект хранения для сценария с именем {0} произошла ошибка: {1}", newScnDB.ScnB.ScenariyName, ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Удаление объект для хранения данных прренадлежащих сценарию
            /// </summary>
            /// <param name="delScnDB">бъект для хранения данных который нужно удалить из списка</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            public bool Remove(ScenariyDataBase delScnDB, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.ScnDBL)
                    {
                        int delIndex = delScnDB.Index;
                        this.ScnDBL.RemoveAt(delIndex);

                        for (int i = delIndex; i < this.ScnDBL.Count; i++)
                        {
                            this.ScnDBL[i].Index = i;
                        }

                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить объект хранения для сценария с именем {0} произошла ошибка: {1}", delScnDB.ScnB.ScenariyName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Обновление объекта для хранения данных пренадлежащих сценарию. Ключом является сценарий
            /// </summary>
            /// <param name="updScnDB">Сценарий у которого нужно изменить данные</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            public bool Update(ScenariyDataBase updScnDB, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.ScnDBL)
                    {
                        int ubpIndex = -1;
                        for (int i = 0; i < this.ScnDBL.Count; i++)
                        {
                            if (this.ScnDBL[i].ScnB.ScenariyName == updScnDB.ScnB.ScenariyName) ubpIndex = i;
                        }

                        if (ubpIndex == -1)
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить объект для хранения данных сценария {0} с таким именем сценария в текущем спискене найдено.", updScnDB.ScnB.ScenariyName));
                        }
                        else
                        {
                            //ScenaryL[ubpIndex].Password = updScenary.Password;
                            rez = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить объект для хранения данных сценария {0} произошла ошибка: {1}", updScnDB.ScnB.ScenariyName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Получение компонента по его ID
            /// </summary>
            /// <param name="i">Введите идентификатор</param>
            /// <returns></returns>
            public ScenariyDataBase getScenaryDataComponent(int i) { return this.ScnDBL[i]; }

            /// <summary>
            /// Для обращения по индексатору
            /// </summary>
            /// <returns>Возвращаем стандарнтый индексатор</returns>
            public IEnumerator GetEnumerator()
            {
                return this.ScnDBL.GetEnumerator();
            }

        }
    }
}
