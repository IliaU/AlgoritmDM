using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Базовый компонент причины скитдки
    /// </summary>
    public class DiscReasonBase
    {
        /// <summary>
        /// Индекс элемента в списке клиентов
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Идентификатор причины скидки
        /// </summary>
        public Int64 DiscReasonId { get; private set; }

        /// <summary>
        /// Строковое иписания причины скидки
        /// </summary>
        public string DiscReasonName { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public DiscReasonBase()
        {
            this.Index = -1;
        }

        /// <summary>
        /// Инициализация базовых объектов
        /// </summary>
        /// <param name="DiscReasonId">Идентификатор причины скидки</param>
        /// <param name="DiscReasonName">Строковое иписания причины скидки</param>
        protected void InitBaseObject(Int64 DiscReasonId, string DiscReasonName)
        {
            this.DiscReasonId = DiscReasonId;
            this.DiscReasonName = DiscReasonName;
        }

        /// <summary>
        /// Базовый класс для общего списка причин скидок
        /// </summary>
        public class DiscReasonListBase : IEnumerable
        {
            /// <summary>
            /// Внутренний список 
            /// </summary>
            private static List<DiscReasonBase> DReasL = new List<DiscReasonBase>();

            /// <summary>
            /// Количчество объектов в контейнере
            /// </summary>
            public int Count { get { return DReasL.Count; } private set { } }

            /// <summary>
            /// Добавление новой причины скидки
            /// </summary>
            /// <param name="delDReas">Причина скидки которую нужно добавить в список</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            public bool Add(DiscReasonBase newDReas, bool HashExeption)
            {
                bool rez = false;

                try
                {
                    lock (DReasL)
                    {
                        bool flag = false;

                        foreach (DiscReasonBase item in DReasL)
                        {
                            if (item.DiscReasonId == newDReas.DiscReasonId) flag = true;
                        }

                        if (!flag)
                        {
                            newDReas.Index = DReasL.Count;
                            DReasL.Add(newDReas);
                            rez = true;
                        }
                        else
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Причина скидки с Id {0} уже существует.", newDReas.DiscReasonId.ToString()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить причину скидки с Id {0} произошла ошибка: {1}", newDReas.DiscReasonId.ToString(), ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Удаление причины скидки
            /// </summary>
            /// <param name="delCust">Причина скидки которую нужно удалить из списка</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Remove(DiscReasonBase delDReas, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (DReasL)
                    {
                        int delIndex = delDReas.Index;
                        DReasL.RemoveAt(delIndex);

                        for (int i = delIndex; i < DReasL.Count; i++)
                        {
                            DReasL[i].Index = i;
                        }

                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить причину скидки и идентификатором {0} и именем {1} произошла ошибка: {2}", delDReas.DiscReasonId.ToString(), delDReas.DiscReasonName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Обновление причины скидки. Заменяет по идентификатору в новой причине скидки
            /// </summary>
            /// <param name="updDReas">Новая причина скидки</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Update(DiscReasonBase updDReas, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (DReasL)
                    {
                        // Находим индекс причины скидки по которому нужно обновить элемент
                        int ubpIndex = -1;
                        for (int i = 0; i < DReasL.Count; i++)
                        {
                            if (DReasL[i].DiscReasonId == updDReas.DiscReasonId) ubpIndex = i;
                        }


                        if (ubpIndex >= 0)
                        {
                            DReasL[ubpIndex] = updDReas;

                            rez = true;
                        }
                        else
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить причину скидки с индексом {0} в списке объекта с таким Index не найдено.", ubpIndex.ToString()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить причину скидки с идентиыфикатором {0} и именем {1} произошла ошибка: {2}", updDReas.DiscReasonId, updDReas.DiscReasonName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Отчистка списка
            /// </summary>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Clear(bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (DReasL)
                    {
                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось список причин скидок. Произошла ошибка: {0}", ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Получение компонента по его ID
            /// </summary>
            /// <param name="i">Введите индекст</param>
            /// <returns></returns>
            protected DiscReasonBase getCustComponent(int i) { return DReasL[i]; }

            /// <summary>
            /// Для обращения по индексатору
            /// </summary>
            /// <returns>Возвращаем стандарнтый индексатор</returns>
            public IEnumerator GetEnumerator()
            {
                return DReasL.GetEnumerator();
            }

        }
    }
}
