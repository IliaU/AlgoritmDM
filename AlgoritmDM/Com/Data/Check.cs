using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AlgoritmDM.Lib;

namespace AlgoritmDM.Com.Data
{
    /// <summary>
    /// Реализует строку чека
    /// </summary>
    public class Check
    {
        /// <summary>
        /// Тип источника откуда пришли данные
        /// </summary>
        EnSourceType SourceType = EnSourceType.Retail;

        /// <summary>
        /// Сид чека
        /// </summary>
        public Int64 InvcSid { get; private set; }

        /// <summary>
        /// Тип чека
        /// </summary>
        public int InvcType { get; private set; }

        /// <summary>
        /// Номер чека
        /// </summary>
        public int InvcNo { get; private set; }

        /// <summary>
        /// Номер позиции в чеке
        /// </summary>
        public int ItemPos { get; private set; }

        /// <summary>
        /// Дата записи в базу
        /// </summary>
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        /// Дата покупки
        /// </summary>
        public DateTime PostDate { get; private set; }
        /// <summary>
        /// Бар код
        /// </summary>
        public string Alu { get; private set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description1 { get; private set; }

        /// <summary>
        /// Дополнительное описание
        /// </summary>
        public string Description2 { get; private set; }

        /// <summary>
        /// Размер
        /// </summary>
        public string Siz { get; private set; }

        /// <summary>
        /// Кол-во
        /// </summary>
        public decimal Qty { get; private set; }

        /// <summary>
        /// Id покупателя
        /// </summary>
        public long? CustSid { get; private set; }

        /// <summary>
        /// Номер магазина
        /// </summary>
        public int StoreNo { get; private set; }

        /// <summary>
        /// Идентификатор причины распродажи
        /// </summary>
        public long DiscReasonId { get; private set; }

        /// <summary>
        /// Идентификатор товара
        /// </summary>
        public long ItemSid { get; private set; }

        /// <summary>
        /// Цена за единицу товара реальная по прайсу за еденицу товара
        /// </summary>
        public decimal OrigPrice { get; private set; }

        /// <summary>
        /// Цена за единицу товара с учётом скидки
        /// </summary>
        public decimal Price { get; private set; }

        /// <summary>
        /// Процент скидки который пользователь мог получить по этой позиции. например OrigPrice/100*UsrDiscPerc получим Price. (Тоесть то что у него на тот момент было настроено)
        /// </summary>
        public decimal UsrDiscPerc { get; private set; }

        /// <summary>
        /// Получил ли пользователь скидку по этой позиции. В базе в поле UsrDiscPerc может быть указана скидка но она не факт что применена. Это значение которое было у клиента у него в таблицу customer
        /// </summary>
        public bool HashUsrDiscPerc
        {
            get
            {
                return (UsrDiscPerc == 0 ? false : (OrigPrice == Price ? false : true));
            }
            private set { }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="SourceType">Тип источника откуда пришли данные</param>
        /// <param name="InvcSid">SID чека</param>
        /// <param name="InvcType">Тип чека</param>
        /// <param name="InvcNo">Номер чека</param>
        /// <param name="ItemPos">Номер позиции в чеке</param>
        /// <param name="CreatedDate">Дата записи в базу</param>
        /// <param name="PostDate">Дата покупки</param>
        /// <param name="Alu">Бар код</param>
        /// <param name="Description1">Описание</param>
        /// <param name="Description2">Дополнительное описание</param>
        /// <param name="Siz">Размер</param>
        /// <param name="Qty">Кол-во</param>
        /// <param name="CustSid">Id покупателя</param>
        /// <param name="StoreNo">Номер магазина</param>
        /// <param name="DiscReasonId">Идентификатор причины распродажи</param>
        /// <param name="ItemSid">Идентификатор товара</param>
        /// <param name="OrigPrice">Цена за единицу товара реальная по прайсу за еденицу товара</param>
        /// <param name="Price">Цена за единицу товара с учётом скидки</param>
        /// <param name="UsrDiscPerc">Процент скидки который получил пользователь по этой позиции. например OrigPrice/100*UsrDiscPerc получим Price.</param>
        public Check(EnSourceType SourceType, Int64 InvcSid, int InvcType, int InvcNo, int ItemPos, DateTime CreatedDate, DateTime PostDate, string Alu, string Description1, string Description2, string Siz, decimal Qty, long? CustSid, int StoreNo, long DiscReasonId, long ItemSid, decimal OrigPrice, decimal Price, decimal UsrDiscPerc)
        {
            this.SourceType = SourceType;
            this.InvcSid = InvcSid;
            this.InvcType = InvcType;
            this.InvcNo = InvcNo;
            this.ItemPos = ItemPos;
            this.CreatedDate = CreatedDate;
            this.PostDate = PostDate;
            this.Alu = Alu;
            this.Description1 = Description1;
            this.Description2 = Description2;
            this.Siz = Siz;
            this.Qty = Qty;
            this.CustSid = CustSid;
            this.StoreNo = StoreNo;
            this.DiscReasonId = DiscReasonId;
            this.ItemSid = ItemSid;
            this.OrigPrice = OrigPrice;
            this.Price = Price;
            this.UsrDiscPerc = UsrDiscPerc;
        }

        public string Print()
        {
            return "InvcSid\tInvcType\tInvcNo\tCreatedDate\tPostDate\tAlu\tDescription1\tDescription2\tSiz\tQty\tCustSid\tStoreNo\tDiscReasonId\tItemSid\tOrigPrice\tPrice\tUsrDiscPerc\r\n"
                + this.InvcSid.ToString() + "\t"
                + this.InvcType.ToString() + "\t"
                + this.InvcNo.ToString() + "\t"
                   + this.CreatedDate.ToString() + "\t"
                   + this.PostDate.ToString() + "\t"
                   + (this.Alu == null ? "NULL" : this.Alu.ToString()) + "\t"
                   + (this.Description1==null? "NULL" :this.Description1.ToString()) + "\t"
                   + (this.Description2==null? "NULL" :this.Description2.ToString()) + "\t"
                   + (this.Siz==null? "NULL" :this.Siz.ToString()) + "\t"
                   + this.Qty.ToString() + "\t"
                   + (this.CustSid == null ? "NULL" : this.CustSid.ToString()) + "\t"
                   + this.StoreNo.ToString() + "\t"
                   + this.DiscReasonId.ToString() + "\t"
                   + this.ItemSid.ToString() + "\t"
                   + this.OrigPrice.ToString() + "\t"
                   + this.Price.ToString() + "\t"
                   + this.UsrDiscPerc.ToString();
        }
    }
}
