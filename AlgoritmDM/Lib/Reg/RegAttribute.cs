using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace AlgoritmDM.Lib.Reg
{
    /// <summary>
    /// Элемент атрибута в реестре
    /// </summary>
    public class RegAttribute
    {
        /// <summary>
        /// Индекс элемента в коллекции
        /// </summary>
        public int Index = -1;

        /// <summary>
        /// Значение из реестра с которым работаем
        /// </summary>
        private string _Nam;

        /// <summary>
        /// Значение из реестра с которым работаем
        /// </summary>
        public string Nam
        {
            get
            {
                return this._Nam;
            }
            private set { }
        }


        /// <summary>
        /// Значение из реестра с которым работаем
        /// </summary>
        private string _Val;

        /// <summary>
        /// Значение из реестра с которым работаем
        /// </summary>
        public string Val
        {
            get 
            {
                return this._Val;
            }
            private set {}
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Nam">Имя атрибута</param>
        /// <param name="Val">Значение атрибута</param>
        public RegAttribute(string Nam, string Val)
        {
            this._Nam = Nam;
            this._Val = Val;
        }

        /// <summary>
        /// Коллекция атрибутов
        /// </summary>
        public class AttributeList
        {
            private List<RegAttribute> AttrL = new List<RegAttribute>();

            /// <summary>
            /// Получение компонента по его ID
            /// </summary>
            /// <param name="i">Индекс</param>
            /// <returns>Возвращаем RegAttribute если он найден</returns>
            public RegAttribute this[int i]
            {
                get
                {
                    RegAttribute rez=null;
                    lock (this.AttrL)
                    {
                        rez = this.AttrL[i];
                    }
                    return rez;
                }
                private set { }
            }

            /// <summary>
            /// Получение компонента по его имени
            /// </summary>
            /// <param name="i">Индекс</param>
            /// <returns>Возвращаем RegAttribute если он найден</returns>
            public RegAttribute this[string s]
            {
                get
                {
                    RegAttribute rez=null;
                    lock (this.AttrL)
                    {
                        foreach (RegAttribute item in this.AttrL)
                        {
                            if (item._Nam == s)
                            {
                                rez = item;
                                break;
                            }
                        }
                    }
                    return rez;
                }
                private set { }
            }

            /// <summary>
            /// Конструктор
            /// </summary>
            public AttributeList()
            {
            }

            /// <summary>
            /// Количчество объектов в контейнере
            /// </summary>
            public int Count 
            { 
                get 
                {
                    int rez;
                    lock (this.AttrL)
                    {
                        rez=AttrL.Count;
                    }
                    return rez;
                } 
                private set { } 
            }

            /// <summary>
            /// Добавление нового атрибута
            /// </summary>
            /// <param name="newAttr">Новый атрибут</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Add(RegAttribute newAttr, bool HashExeption)
            {
                bool rez = false;

                try
                {
                    lock (this.AttrL)
                    {
                        bool flag = false;

                        foreach (RegAttribute item in this.AttrL)
                        {
                            if (item.Nam == newAttr.Nam) flag = true;
                        }

                        if (!flag)
                        {
                            newAttr.Index = AttrL.Count;
                            this.AttrL.Add(newAttr);
                            rez = true;
                        }
                        else
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Атрибут с именем {0} уже существует.", newAttr.Nam));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить атрибут {0} произошла ошибка: {1}", newAttr.Nam, ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Удаление атрибута
            /// </summary>
            /// <param name="delAttr">Атибут который нужно удалить из списка</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Remove(RegAttribute delAttr, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.AttrL)
                    {
                        int delIndex = delAttr.Index;
                        this.AttrL.RemoveAt(delIndex);

                        for (int i = delIndex; i < this.AttrL.Count; i++)
                        {
                            this.AttrL[i].Index = i;
                        }

                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить атрибут {0} произошла ошибка: {1}", delAttr.Nam, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Обновление данных атрибута. Ключом является Nam он не обнавляется
            /// </summary>
            /// <param name="updUser">Пользователь у которого нужно изменить данные</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Update(RegAttribute updAttr, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.AttrL)
                    {
                        int ubpIndex = -1;
                        for (int i = 0; i < AttrL.Count; i++)
                        {
                            if (this.AttrL[i]._Nam == updAttr.Nam) ubpIndex = i;
                        }

                        if (ubpIndex == -1)
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные атрибута {0} с таким именем атрибута в текущем спискене найдено.", updAttr.Nam));
                        }
                        else
                        {
                            this.AttrL[ubpIndex]._Val = updAttr._Val;

                            rez = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные атрибута {0} произошла ошибка: {1}", updAttr._Nam, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Для обращения по индексатору
            /// </summary>
            /// <returns>Возвращаем стандарнтый индексатор</returns>
            public IEnumerator GetEnumerator()
            {
                IEnumerator rez;
                lock (this.AttrL)
                {
                    rez = this.AttrL.GetEnumerator();
                }
                return rez;
            }

        }
    }
}
