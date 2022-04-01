using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgoritmDM
{
    public partial class FConnetSetupPrizm : Form
    {
        // Список типов провайдеров
        private List<string> cmbBoxPrvTypList = new List<string>();

        // Текущий провайдер
        private Com.ProviderPrizm CurrentPrv;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FConnetSetupPrizm()
        {
            InitializeComponent();

            // Подгружаем список возможных провайдеров
            this.cmbBoxPrvTyp.Items.Clear();
            cmbBoxPrvTypList = Com.ProviderPrizmFarm.GetListProviderPrizmName();
            foreach (string item in cmbBoxPrvTypList)
            {
                this.cmbBoxPrvTyp.Items.Add(item);
            }

            // Если всего один тип провайдеров существует то устанавливаем по умолчанию этот тип
            if (this.cmbBoxPrvTyp.Items.Count == 1) this.cmbBoxPrvTyp.SelectedIndex = 0;
        }

        // Чтение формы
        private void FConnetSetupPrizm_Load(object sender, EventArgs e)
        {
            // Получаем текущий провайдер
            this.CurrentPrv = Com.ProviderPrizmFarm.CurProviderPrizm;

            // Если текущий провайдер есть и он не выбран то нужно указать его тип
            if (this.CurrentPrv != null)
            {
                for (int i = 0; i < this.cmbBoxPrvTyp.Items.Count; i++)
                {
                    if (this.cmbBoxPrvTyp.Items[i].ToString() == this.CurrentPrv.PlugInType) this.cmbBoxPrvTyp.SelectedIndex = i;
                }
            }

            //  Если текущий провайдер не установлен то на выход
            if (this.CurrentPrv == null) return;
            this.txtBoxConnectionString.Text = this.CurrentPrv.PrintConnectionString();
        }

        // Пользователь решил изменить 
        private void btnConfig_Click(object sender, EventArgs e)
        {
            if (this.cmbBoxPrvTyp.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали тип провайдера который вы будите использовать.");
                return;
            }

            // Создаём ссылку на подключение которое будем править
            Com.ProviderPrizm PrvTmp = null;
            //
            // Если текущий провайдер не установлен то иницилизируем его новый экземпляр или создаём его на основе уже существующего провайдера
            if (this.CurrentPrv == null || (this.CurrentPrv != null && this.CurrentPrv.PlugInType != this.cmbBoxPrvTyp.Items[this.cmbBoxPrvTyp.SelectedIndex].ToString()))
            {
                PrvTmp = Com.ProviderPrizmFarm.CreateNewProviderPrizm(this.cmbBoxPrvTyp.Items[this.cmbBoxPrvTyp.SelectedIndex].ToString());
            }
            else PrvTmp = Com.ProviderPrizmFarm.CreateNewProviderPrizm(this.CurrentPrv.PlugInType, this.CurrentPrv.ConnectionString);
            // 
            // Запускаем правку нового подключения
            bool HashSaveProvider = ((Com.PlgProviderPrizm.Lib.ProviderPrizmInterface)PrvTmp).SetupProviderPrizm();

            // Пользователь сохраняет данный провайдер в качестве текущего
            if (HashSaveProvider)
            {
                Com.ProviderPrizmFarm.SetupCurrentProvider(PrvTmp);
            }

            // Перечитываем текущую форму
            FConnetSetupPrizm_Load(null, null);
        }
    }
}
