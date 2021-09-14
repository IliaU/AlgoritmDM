using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;
using System.Collections;

namespace AlgoritmDM.Lib.Reg
{
    /// <summary>
    /// Узел эелемента
    /// </summary>
    public class Reg
    {
        #region Private атрибуты

            /// <summary>
            /// Ключ реестра к которомку принадлежит данный лист
            /// </summary>
            private Reg ParentReg;

        #endregion

        #region Protected атрибуты
            /// <summary>
            /// Имя ключа реестра
            /// </summary>
            protected string _RegName;
            
        #endregion

        #region Public атрибуты

            /// <summary>
            /// Индекс элемента в коллекции
            /// </summary>
            public int Index { get; private set; }

            /// <summary>
            /// Имя ключа реестра
            /// </summary>
            public string RegName
            {
                get
                {
                    return this._RegName;
                }
                private set { }
            }

            /// <summary>
            /// Список атрибутов этой папки
            /// </summary>
            public RegAttribute.AttributeList Attributes { get; private set; }

            /// <summary>
            /// Список дочерних ключей
            /// </summary>
            public RegList RegItems { get; private set; }

        #endregion


        #region Внешние методы

            /// <summary>
            /// Конструктор
            /// </summary>
            public Reg()
            {
                this.Index = -1;
                this.ParentReg = null;
                this.Attributes = new RegAttribute.AttributeList();
                this.RegItems = new RegList(this);
            }    
            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="RegName">Имя ключа реестра</param>
            public Reg(string RegName):this ()
            {
                if (string.IsNullOrWhiteSpace(RegName)) throw new ApplicationException("Нельзя создать ключь с пустым именем");
                this._RegName = RegName;
            }


        #endregion

        #region Protected методы

            /// <summary>
            /// Объект для блокировки в многопоточном режиме
            /// </summary>
            protected virtual object myLock()
            {
                if (ParentReg == null) return null;
                else return ParentReg.myLock();
            }

            /// <summary>
            /// Корневой объект реестра
            /// </summary>
            protected virtual RegistryKey WinReg()
            {
                if (ParentReg == null) return null;
                else return ParentReg.WinReg();
            }

            /// <summary>
            /// Метод для сохранения эелементов узла в реестре
            /// </summary>
            protected void Save()
            {
                try
                {
                    string dir = FullPath();

                    // Запись в реестра
                    RegistryKey hk = WinReg();
                    lock (this.myLock())
                    {
                        //hk.OpenSubKey(dir, true);
                        //hk.SetValue(this.RegName, dir);
                        //hk.Close();

                        // Сохраняем ключ если он уже есть то ошибки не будет и содержимое останется внутри
                        hk = hk.OpenSubKey(dir, true);
                        hk.CreateSubKey(this.RegName);
                        hk.Close();

                        // Переходим в наш объект и читаем атрибуты и подпапки
                        hk = WinReg();
                        hk = hk.OpenSubKey(dir + @"\" + this.RegName, true);
                        // Проверяем подпапки если их нет то грохаем их
                        foreach (string item in hk.GetSubKeyNames())
                        {
                            Reg Regtmp = this.RegItems[item];
                            if (Regtmp == null) hk.DeleteSubKey(item);
                        }
                        // Проверяем атрибуты если их нет то грохаем их
                        foreach (string item in hk.GetValueNames())
                        {
                            RegAttribute Attrtmp = this.Attributes[item];
                            if (Attrtmp == null) hk.DeleteValue(item);
                        }
                        // Пробегаем по нашим дочернем узлам и создаём если уже существует то ничего не будет
                        foreach (Reg item in this.RegItems)
                        {
                            hk.CreateSubKey(item.RegName);
                            item.Save();
                        }
                        // Пробегаем по атрибутам и сохраняем их
                        foreach (RegAttribute item in this.Attributes)
                        {
                            hk.SetValue(item.Nam, item.Val);
                        }
                    }
                }
                catch (Exception) { throw; }
                finally { }
            }
            
            /// <summary>
            /// Полный путь к объекту
            /// </summary>
            /// <returns>Полный путь к объекту</returns>
            protected virtual string FullPath()
            {
                if (ParentReg == null) return null;
                else return ParentReg.FullPath();
            }

        #endregion

        /// <summary>
        /// Коллекция ключей реестра
        /// </summary>
        public class RegList
        {
            /// <summary>
            /// Объект храняфий список ключей
            /// </summary>
            private List<Reg> RegL = new List<Reg>();

            /// <summary>
            /// Ключ реестра к которомку принадлежит данный лист
            /// </summary>
            private Reg ParentReg;

            // <summary>
            /// Получение компонента по его ID
            /// </summary>
            /// <param name="i">Индекс</param>
            /// <returns>Возвращаем RegAttribute если он найден</returns>
            public Reg this[int i]
            {
                get
                {
                    Reg rez=null;
                    lock (this.RegL)
                    {
                        rez = this.RegL[i];
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
            public Reg this[string s]
            {
                get
                {
                    Reg rez=null;
                    lock (this.RegL)
                    {
                        foreach (Reg item in this.RegL)
                        {
                            if (item._RegName == s)
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
            /// <param name="Reg">Ключ реестра к которомку принадлежит данный лист</param>
            public RegList(Reg Reg)
            {
                this.ParentReg = Reg;
            }

            /// <summary>
            /// Количчество объектов в контейнере
            /// </summary>
            public int Count 
            { 
                get 
                {
                    int rez;
                    lock (this.RegL)
                    {
                        rez = RegL.Count;
                    }
                    return rez;
                } 
                private set { } 
            }

            /// <summary>
            /// Добавление нового ключа
            /// </summary>
            /// <param name="newReg">Новый ключ</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            public bool Add(Reg newReg, bool HashExeption)
            {
                bool rez = false;

                try
                {
                    lock (this.RegL)
                    {
                        bool flag = false;

                        foreach (Reg item in this.RegL)
                        {
                            if (item._RegName == newReg._RegName) flag = true;
                        }

                        if (!flag)
                        {
                            newReg.Index = RegL.Count;
                            newReg.ParentReg = this.ParentReg;
                            this.RegL.Add(newReg);
                            newReg.Save();
                            rez = true;
                        }
                        else
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Ключ с именем {0} уже существует.", newReg._RegName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить ключь {0} произошла ошибка: {1}", newReg._RegName, ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Удаление ключа
            /// </summary>
            /// <param name="delReg">Ключ который нужно удалить из списка</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            public bool Remove(Reg delReg, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.RegL)
                    {
                        int delIndex = delReg.Index;
                        this.RegL.RemoveAt(delIndex);

                        for (int i = delIndex; i < this.RegL.Count; i++)
                        {
                            this.RegL[i].Index = i;
                        }

                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить ключь {0} произошла ошибка: {1}", delReg._RegName, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Обновление ключа. Ключом является Nam он не обнавляется
            /// </summary>
            /// <param name="updReg">Пользователь у которого нужно изменить данные</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            public bool Update(Reg updReg, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.RegL)
                    {
                        int ubpIndex = -1;
                        for (int i = 0; i < this.RegL.Count; i++)
                        {
                            if (this.RegL[i]._RegName == updReg._RegName) ubpIndex = i;
                        }

                        if (ubpIndex == -1)
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные ключа {0} с таким именем ключа в текущем спискене найдено.", updReg._RegName));
                        }
                        else
                        {
                            this.RegL[ubpIndex] = updReg;

                            rez = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные ключа {0} произошла ошибка: {1}", updReg._RegName, ex.Message));
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
                lock (this.RegL)
                {
                    rez = this.RegL.GetEnumerator();
                }
                return rez;
            }

        }
    }
}
