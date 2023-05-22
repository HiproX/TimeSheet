using System.Globalization;

namespace TimeSheet
{
    public partial class FormTable : Form
    {
        public FormTable()
        {
            InitializeComponent();
            DataBase.Connect();
            InitPanelControl();
            InitGridHeaders();
        }
        private void InitPanelControl()
        {
            DateTimeFormatInfo dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            //string[] months = { "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь" };  

            foreach (var monthName in dateTimeFormat.MonthNames)
            {
                if (string.IsNullOrEmpty(monthName))
                {
                    continue;
                }
                var month = char.ToUpper(monthName[0]) + monthName.Substring(1).ToLower();
                var tabPage = new TabPage();
                tabControlPanel.Controls.Add(tabPage);
                //tabPage.Controls.Add(this.tableLayoutPanel1);
                tabPage.Location = new System.Drawing.Point(4, 24);
                tabPage.Padding = new System.Windows.Forms.Padding(3);
                tabPage.Size = new System.Drawing.Size(1043, 542);
                tabPage.TabIndex = 0;
                tabPage.Text = month;
                tabPage.UseVisualStyleBackColor = true;
                tabPage.Controls.Add(panel1);
                tabPage.Enter += OnTabPageClick;
            }
        }

        private void OnTabPageClick(object? sender, EventArgs e)
        {
            var obj = sender as TabPage;
            if (obj == null) return;
            obj.Controls.Add(panel1);
        }

        private void InitGridHeaders()
        {
            foreach (var header in new string[] { "ФИО", "Должность", "Табельный №" })
            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = header;
                column.Width = 100;
                //column.ReadOnly = true;
                column.Frozen = true;
                column.DefaultCellStyle.BackColor = Color.IndianRed;
                dataGridView1.Columns.Add(column);
            }
            foreach (var number in new int[] {1, 2, 3, 4, 5})
            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = number.ToString();
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.ReadOnly = true;
                dataGridView1.Columns.Add(column);
            }

            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = "Итого";
                column.Width = 100;
                column.ReadOnly = true;
                dataGridView1.Columns.Add(column);
            }
        }
    }
}