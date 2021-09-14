using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    /// <summary>
    /// Класс для конвертации моих собственных событий
    /// </summary>
    public static class EventConverter
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        //public EventConverter() { }


        /// <summary>
        /// Получаем список событий в текстовом виде с указанным разделитеолем
        /// </summary>
        /// <param name="chr">Текстовый вариант нашего события</param>
        //public string Convert(Char chr)
        //{
        //    string MyEventSpisok = null;

        //    foreach (EventEn item in Enum.GetValues(typeof(EventEn)))
        //    {
        //        if (MyEventSpisok != null) MyEventSpisok = MyEventSpisok + chr.ToString() + item.ToString();
        //        else MyEventSpisok = item.ToString();
        //    }
        //    return MyEventSpisok;
        //}

        /// <summary>
        /// Конвертация в объект MyEvent
        /// </summary>
        /// <param name="Event">Текстовый вариант нашего события</param>
        /// <returns>Возврашаем костомизированное сопытие из перечисления</returns>
        public static EventEn Convert(string Event) //:this ()
        {
            if (Event != null && Event.Trim() != string.Empty)
            {
                foreach (EventEn item in Enum.GetValues(typeof(EventEn)))
                {
                    if (item.ToString() == Event.Trim()) return item;
                }
            }
            throw new ApplicationException("Не смогли преобразовать: " + Event);
        }

        /// <summary>
        /// Конвертация в RoleEn
        /// </summary>
        /// <param name="Role">Роль указанная в виде строки</param>
        /// <param name="DefaultRole">Роль которую вернуть в случае невозможности отпарсить её</param>
        /// <returns>Возвращает костамизированную роль в которой может работает пользователь</returns>
        public static RoleEn Convert(string Role, RoleEn DefaultRole)
        {
            if (Role != null && Role.Trim() != string.Empty)
            {
                foreach (RoleEn item in RoleEn.GetValues(typeof(RoleEn)))
                {
                    if (item.ToString() == Role.Trim()) return item;
                }
            }
            return DefaultRole;
        }

        /// <summary>
        /// Получение списка всехвозможных вариантов энумератора CongigurationActionSaleEn
        /// </summary>
        /// <returns>Список вариантов</returns>
        public static List<string> GetListCongigurationActionSaleEn()
        {
            List<string> rez = new List<string>();
            foreach (CongigurationActionSaleEn item in CongigurationActionSaleEn.GetValues(typeof(CongigurationActionSaleEn)))
            {
                rez.Add(item.ToString());
            }
            return rez;
        }

        /// <summary>
        /// Конвертация в ModeEn
        /// </summary>
        /// <param name="Mode">Режим работы в виде строки</param>
        /// <param name="DefaultMode">Режим работы который вернуть в случае невозможности отпарсить её</param>
        /// <returns>Возвращает костамизированную роль в которой может работает пользователь</returns>
        public static ModeEn Convert(string Mode, ModeEn DefaultMode)
        {
            if (Mode != null && Mode.Trim() != string.Empty)
            {
                foreach (ModeEn item in ModeEn.GetValues(typeof(ModeEn)))
                {
                    if (item.ToString() == Mode.Trim()) return item;
                }
            }
            return DefaultMode;
        }

        /// <summary>
        /// Конвертация в CongigurationActionSaleEn
        /// </summary>
        /// <param name="Mode">Действия сценария по отношению к предыдущему занчению скидки в виде строки</param>
        /// <param name="DefaultMode">Действия сценария по отношению к предыдущему занчению скидки, который вернуть в случае невозможности отпарсить её</param>
        /// <returns>Возвращает костамизированную действие сценария по отношению к предыдущему занчению скидки</returns>
        public static CongigurationActionSaleEn Convert(string Mode, CongigurationActionSaleEn DefaultMode)
        {
            if (Mode != null && Mode.Trim() != string.Empty)
            {
                foreach (CongigurationActionSaleEn item in CongigurationActionSaleEn.GetValues(typeof(CongigurationActionSaleEn)))
                {
                    if (item.ToString() == Mode.Trim()) return item;
                }
            }
            return DefaultMode;
        }

    }
}
