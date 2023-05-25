using Newtonsoft.Json.Converters;
using System.Globalization;
using TimeSheet.Enums;
using TimeSheet.Entities;
using System.Text;
using System.Diagnostics.Metrics;

namespace TimeSheet
{
    public partial class FormTable : Form
    {
        public FormTable()
        {
            InitializeComponent();

            tabControlPanel.Controls.Clear();
            tabControlPanel.Enabled = false;

            gridView.ReadOnly = false; // Запрет на редактирование таблицы
            gridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // выравнивание элементов таблицы по центру

            departmentsList.MouseDoubleClick += DepartmentsList_DoubleClick;
            gridView.CellFormatting += dataGridView_CellFormatting;
        }
        /// <summary>
        /// Инициализация вкладок на панели - [Январь, Декабрь]
        /// </summary>
        private void InitializeTabsInControlPanel()
        {
            var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

            foreach (var monthName in monthNames)
            {
                if (string.IsNullOrEmpty(monthName))
                {
                    continue;
                }
                var month = char.ToUpper(monthName[0]) + monthName.Substring(1).ToLower();
                var tabPage = CreateTabPage(month);
                tabControlPanel.Controls.Add(tabPage);
            }

            var currMonthIdx = DateTime.Now.Month - 1;
            tabControlPanel.SelectedIndex = currMonthIdx;
        }
        /// <summary>
        /// Создать объект новой вкладки
        /// </summary>
        /// <param name="text">Текст вкладки</param>
        /// <returns>Созданный объект вкладки</returns>
        private TabPage CreateTabPage(string text)
        {
            var tabPage = new TabPage();
            tabPage.Location = new System.Drawing.Point(4, 24);
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.Size = new System.Drawing.Size(1043, 542);
            tabPage.Text = text;
            tabPage.UseVisualStyleBackColor = true;
            //tabPage.Enter += OnTabPageClick;
            return tabPage;
        }
        /// <summary>
        /// Отобразить сетку по выбранному департаменту и месяцу
        /// </summary>
        private async void LoadGridView()
        {
            var department = CurrentDepartment;
            if (department == null) return;

            // Создание заголовков
            InitializeGridHeaders();

            int year = DateTime.Now.Year;
            var taskEmployees = DataBase.GetEmployeesByDepartment(department.Id);
            var taskProductionCalendar = DataBase.GetProductionCalendarForMonth(year, this.CurrentMonth);

            _employeesSelectedDepartmend = await taskEmployees;
            _selectedProductionCalendar = await taskProductionCalendar;
            _attendanceRecordsStorage = await DataBase.GetAttendanceRecords(year, this.CurrentMonth, _employeesSelectedDepartmend);

            // Вывод в таблицу ...
            foreach (var employee in _employeesSelectedDepartmend)
            {
                var row = new DataGridViewRow();
                var codeCounter = new Dictionary<Marker, int>();

                // Информация о работнике
                foreach (var item in new object[] { employee.FullName, employee.Position.GetDescription(), employee.Id })
                {
                    var cell = new DataGridViewTextBoxCell();
                    cell.Value = item;
                    row.Cells.Add(cell);
                }

                // Информации о посещении
                for (int day = 1; day <= this.CurrentDaysInMonth; ++day)
                {
                    var cell = new DataGridViewTextBoxCell();
                    var marker_id = _attendanceRecordsStorage.GetMarkerId(employee, day);
                    var marker = _markerStorage.GetMarkerById(marker_id);

                    // Считаем уникальные отметки для сотрудника, которые в последующем будут использованы в "Итого"
                    if (marker != null)
                    {
                        if (codeCounter.ContainsKey(marker))
                        {
                            ++codeCounter[marker];
                        }
                        else
                        {
                            codeCounter[marker] = 1;
                        }
                    }

                    // Вывод информации о посещении
                    cell.Value = marker?.Code ?? String.Empty;
                    row.Cells.Add(cell);
                }

                // Итого
                // собираем уникальные отметки в строку
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var key in codeCounter.Keys.OrderBy(key => key.Code))
                {
                    stringBuilder.Append($"{key.Code}({codeCounter[key]});");
                }
                // выводим стркоку с уникальными отметками о посещении
                var total = new DataGridViewTextBoxCell();
                total.Value = stringBuilder.ToString();
                row.Cells.Add(total);

                gridView.Rows.Add(row);
            }
        }
        /// <summary>
        /// Инициализация заголовков в сетке 
        /// </summary>
        private void InitializeGridHeaders()
        {
            if (gridView.Columns.Count > 0)
            {
                gridView.Columns.Clear();
            }
            foreach (var header in new dynamic[] { new { Text="ФИО" }, new { Text="Должность" }, new { Text="Табельный №"} })
            {
                var column = new DataGridViewTextBoxColumn();
                column.HeaderText = header.Text;
                column.AutoSizeMode = column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                //column.Width = header.Width;
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
                //column.Width = 180;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.ReadOnly = true;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridView.Columns.Add(column);
            }
        }
        ///<summary>
        /// Инициализация списка департаментов. Загрузка списка из БД и вывод на экран
        /// </summary>
        private async Task InitializeDepartmentList()
        {
            Thread.Sleep(1000);
            departmentsList.Items.Clear();

            _departments = await DataBase.GetDepartments();
            foreach (var department in _departments)
            {
                departmentsList.Items.Add(department.Name);
            }
        }
        /// <summary>
        /// Событие позволяющее отследить клик по вкладке с месяцами
        /// </summary>
        private void OnTabPageClick(object? sender, EventArgs e)
        {
            var obj = sender as TabControl;
            if (obj == null) throw new Exception("Ошибка #001. Обратитесь к разработчику");
            obj.TabPages[obj.SelectedIndex].Controls.Add(panel1);
            LoadGridView();
        }
        /// <summary>
        /// Событие позволяющее отследить двойной щелчек мыши по одному из доступных департаменов в списке
        /// </summary>
        private void DepartmentsList_DoubleClick(object? sender, MouseEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null) return;

            int index = listBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                //tabControlPanel.SelectedIndex = DateTime.Now.Month - 1;
                if (!IsTabPagesInitialized)
                {
                    tabControlPanel.SelectedIndexChanged += OnTabPageClick;
                    tabControlPanel.Enabled = IsTabPagesInitialized = true;
                }
                OnTabPageClick(tabControlPanel, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Событие позволяющее настраивать внешний вид ячеек во время выполнения программы.
        /// Запускается, когда ячейку необходимо отформатировать для отображения
        /// </summary>
        private void dataGridView_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            var dataGridView = sender as DataGridView;
            if (dataGridView == null) return;

            /*/ Установка цвета строк таблицы
            if (e.RowIndex >= 0)
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
            }//*/

            /*/ Цвет выделенных строк
            if (dataGridView.Rows[e.RowIndex].Selected)
            {
                dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
            }//*/

            // Установить цвет столбцов для дней месяца у работников
            if (e.ColumnIndex >= 2 && e.ColumnIndex < dataGridView.ColumnCount - 1)
            {
                string headerText = dataGridView.Columns[e.ColumnIndex].HeaderText;
                var success = Int32.TryParse(headerText, out int day);
                --day;
                if (success)
                {
                    if (day < 0 || day >= _selectedProductionCalendar.Count) return;

                    var type = _selectedProductionCalendar[day].Type;
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = GetDayCellColor(type);
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
        private async void FormTable_Load(object sender, EventArgs e)
        {
            await FormTable_LoadAsync();
            InitializeTabsInControlPanel();
        }
        private async Task FormTable_LoadAsync()
        {
            var taskLoadMarkers = DataBase.GetMarkers();
            var taskInitDepartments = InitializeDepartmentList();

            _markerStorage = await taskLoadMarkers;
            await taskInitDepartments;

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
        /// Хранилище возможных видов отметоки с кодировкой о работе (предварительно нужно загружать из БД)
        /// </summary>
        private MarkerStorage _markerStorage;
        /// <summary>
        /// Хранилище учета записей о посещении (предварительно нужно загружать из БД)
        /// </summary>
        private AttendanceRecordsStorage _attendanceRecordsStorage;
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
        /// <summary>
        /// Были ли проинициализированы вкладки
        /// </summary>
        private bool IsTabPagesInitialized { get; set; } = false;
    }
}