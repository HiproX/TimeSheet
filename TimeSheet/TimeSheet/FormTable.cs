using Newtonsoft.Json.Converters;
using System.Globalization;
using TimeSheet.Enums;

namespace TimeSheet
{
    public partial class FormTable : Form
    {
        public FormTable()
        {
            InitializeComponent();

            tabControlPanel.Controls.Clear();
            tabControlPanel.SelectedTab = null; // or .SelectedIndex = -1
            tabControlPanel.Enabled = false;
            gridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            departmentsList.MouseDoubleClick += DepartmentsList_DoubleClick;
            gridView.CellFormatting += dataGridView_CellFormatting;

            DataBase.Connect();
            InitTabsInControlPanel();
            InitDepartmentList();
        }
        /// <summary>
        /// Инициализация вкладок на панели - [Январь, Декабрь]
        /// </summary>
        private void InitTabsInControlPanel()
        {
            tabControlPanel.Controls.Clear();
            var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            foreach (var monthName in monthNames)
            {
                if (string.IsNullOrEmpty(monthName))
                {
                    continue;
                }
                var month = char.ToUpper(monthName[0]) + monthName.Substring(1).ToLower();
                var tabPage = new TabPage();
                tabControlPanel.Controls.Add(tabPage);
                tabPage.Location = new System.Drawing.Point(4, 24);
                tabPage.Padding = new System.Windows.Forms.Padding(3);
                tabPage.Size = new System.Drawing.Size(1043, 542);
                tabPage.TabIndex = 0;
                tabPage.Text = month;
                tabPage.UseVisualStyleBackColor = true;
                tabPage.Controls.Add(panel1);
                tabPage.Enter += OnTabPageClick;
            }
            var currMonthIdx = DateTime.Now.Month - 1;
        }
        
        private void OnTabPageClick(object? sender, EventArgs e)
        {
            var obj = sender as TabPage;
            if (obj == null) return;
            obj.Controls.Add(panel1);
            LoadGridView();
        }
        /// <summary>
        /// Отобразить сетку по выбранному департаменту и месяцу
        /// </summary>
        private void LoadGridView()
        {
            // If Months selected (protection from when the TabPage is not selected)
            if (tabControlPanel.SelectedIndex != -1)
            {
                var department = CurrentDepartment;
                if (department == null) return;

                // Creating headers with employee data
                InitGridHeaders();

                _employeesSelectedDepartmend = DataBase.GetEmployeesByDepartment(department.Id);
                _selectedProductionCalendar = DataBase.GetProductionCalendarForMonth(DateTime.Now.Year, this.CurrentMonth);

                // Вывод в таблицу ...
                foreach (var employee in _employeesSelectedDepartmend)
                {
                    var row = new DataGridViewRow();

                    // Информация о работнике
                    foreach (var item in new object[] { employee.Name, employee.Position.GetDescription(), employee.Id })
                    {
                        var cell = new DataGridViewTextBoxCell();
                        cell.Value = item;
                        row.Cells.Add(cell);
                    }

                    // Информации о посещении
                    for (int day = 0; day < this.CurrentDaysInMonth; ++day)
                    {
                        var cell = new DataGridViewTextBoxCell();
                        cell.Value = day;
                        row.Cells.Add(cell);
                    }

                    // Итого
                    var total = new DataGridViewTextBoxCell();
                    total.Value = $"total_{employee.Id}";
                    row.Cells.Add(total);

                    gridView.Rows.Add(row);
                }

                tabControlPanel.Enabled = true;
            }
        }
        /// <summary>
        /// Инициализация заголовков в сетке 
        /// </summary>
        private void InitGridHeaders()
        {
            if (gridView.Columns.Count > 0)
            {
                gridView.Columns.Clear();
            }
            foreach (var header in new string[] { "ФИО", "Должность", "Табельный №" })
            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = header;
                column.Width = 100;
                //column.ReadOnly = true;
                column.Frozen = true;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridView.Columns.Add(column);
            }

            foreach (var number in Enumerable.Range(1, this.CurrentDaysInMonth).ToArray())
            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = number.ToString();
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.ReadOnly = true;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridView.Columns.Add(column);
            }


            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = "Итого";
                column.Width = 100;
                column.ReadOnly = true;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridView.Columns.Add(column);
            }
        }
        ///<summary>
        /// Инициализация списка департаментов. Загрузка списка из БД и вывод на экран
        /// </summary>
        private void InitDepartmentList()
        {
            departmentsList.Items.Clear();

            _departments = DataBase.GetDepartments();
            foreach (var department in _departments)
            {
                departmentsList.Items.Add(department.Name);
            }
        }
        private void DepartmentsList_DoubleClick(object? sender, MouseEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                int index = listBox.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    tabControlPanel.SelectedIndex = DateTime.Now.Month - 1;
                    LoadGridView();
                }
            }
        }
        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;

            // Установка цвета строк таблицы
            /*if (e.RowIndex >= 0)
            {
                // Odd rows
                if (e.RowIndex % 2 == 0)
                {
                    dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Tan;
                }
                // Even rows
                else
                {
                    dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Snow;
                }
            }*/

            // Цвет выделенных строк
            if (dataGridView.Rows[e.RowIndex].Selected)
            {
                dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
            }

            // Установить цвет столбцов для дней месяца
            if (e.ColumnIndex >= 2 && e.ColumnIndex < dataGridView.ColumnCount - 1) // Columns after index 2
            {
                string headerText = dataGridView.Columns[e.ColumnIndex].HeaderText;
                var success = Int32.TryParse(headerText, out int day);
                if (success)
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = GetDayCellColor(_selectedProductionCalendar[day - 1].Type);
                }
            }
        }
        /// <summary>
        /// Получить цвет ячкий по типу дня
        /// </summary>
        /// <param name="type">Тип дня</param>
        /// <returns>Цвет ячейки</returns>
        private Color GetDayCellColor(DayType type)
        {
            switch(type)
            {
                case DayType.WorkingDay: return Color.LightGreen;
                case DayType.DayOff: return Color.AntiqueWhite;
                case DayType.PreHoliday: return Color.LightSalmon;
                case DayType.Holiday: return Color.LightCoral;
                default: return Color.White;
            }
        }
        private void FormTable_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Список департаментов. По умолчанию это пустой список.
        /// </summary>
        private List<Department> _departments = new();
        /// <summary>
        /// Работники выбранного департамента. По умолчанию это пустой список.
        /// </summary>
        private List<Employee> _employeesSelectedDepartmend = new();
        /// <summary>
        /// Календарь выбраного месяц. По умолчанию это пустой список.
        /// </summary>
        private List<ProductionCalendar> _selectedProductionCalendar = new();
        /// <summary>
        /// Количество дней в выбранном месяце на вкладке. Возвращает количество дней. Например: для Января - 31
        /// </summary>
        private int CurrentDaysInMonth
        {
            get
            {
                var selectedMonthNumber = tabControlPanel.SelectedIndex + 1;
                var daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, selectedMonthNumber);
                return daysInCurrentMonth;
            }
        }
        /// <summary>
        /// Выбранный месяц на вкладке. Если вкладка активна возвращает номер выбранного месяца
        /// в диапазоне [1, 12], иначе возвращает -1
        /// </summary>
        private int CurrentMonth
        {
            get
            {
                var selectedMonthIndex = tabControlPanel.SelectedIndex;
                if (selectedMonthIndex == -1) return selectedMonthIndex;
                return selectedMonthIndex + 1;
            }
        }
        /// <summary>
        /// Возвращает ссылку на объект выбранного департамента. Если департамент не выбран, то возвращает null
        /// </summary>
        private Department? CurrentDepartment
        {
            get
            {
                int index = departmentsList.SelectedIndex;

                if (index == -1) return null;
                else return _departments[index];
            }
        }
    }
}