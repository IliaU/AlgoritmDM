using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AlgoritmDM.Com.Scenariy.BonusDM
{
    public partial class FInfo : Form
    {
        /// <summary>
        /// Типоризированный сценарий с которым мы работаем
        /// </summary>
        private BonusDMscn MainBDM;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="MainBDM">Ссылка на наш типоризированный объект который мы будем настраивать</param>
        public FInfo(BonusDMscn MainBDM)
        {
            this.MainBDM = MainBDM;
            InitializeComponent();
        }
    }
}
