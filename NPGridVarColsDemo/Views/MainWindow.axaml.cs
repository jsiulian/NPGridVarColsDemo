using Avalonia.Controls;
using Avalonia.Data;
using NP.Avalonia.Visuals.Behaviors.DataGridBehaviors;
using System.Collections.Generic;
using System.Linq;

namespace NPGridVarColsDemo.Views
{
    public partial class MainWindow : Window
    {
        private DataGrid? mainGrid = null;

        public DataGrid MainGrid
        {
            get
            {
                mainGrid ??= this.FindControl<DataGrid>("LstDataGrid");
                return mainGrid;
            }
        }

        public IEnumerable<IEnumerable<string?>> RawData
        {
            get
            {
                var items = this.MainGrid != null ? DataGridCollectionViewBehavior.GetItemsSource(this.MainGrid) : null;
                if (items == null)
                {
                    return new List<List<string?>>(0);
                }

                return items.Cast<IEnumerable<string?>>().ToList();
            }
            set
            {
                if (value != null)
                {
                    IEnumerable<IEnumerable<string?>>? data = value;

                    // Datagrid can only work with lists
                    DataGridCollectionViewBehavior.SetItemsSource(this.MainGrid, data.Select(x => x.ToList()).ToList());
                }
            }
        }

        public void SetDataGridColumns(IEnumerable<string> columns)
        {
            if (this.MainGrid.Columns.Any())
            {
                this.MainGrid.Columns.Clear();
            }
            int i = 0;
            foreach (var column in columns)
            {
                string bindingPath = $"[{i}]";
                DataGridTextColumn col = new DataGridTextColumn()
                {
                    Header = column,
                    Binding = new Binding(bindingPath),
                    IsReadOnly = false
                };

                var ii = i;
                object funcColPropertyGetter(object o)
                {
                    List<string?> l = (List<string?>)o;

                    return l.ElementAt(ii) ?? new object();
                }
                this.MainGrid.Columns.Add(col);

                /*
                 * Important: SetFilterPropName is set to a dummy property name to allow enabling the filter textbox, but that is in fact driven by ColumnPropGetter in this case
                 * Commenting this line out will disable the filter textbox, even though it is not needed.
                */ 
                DataGridFilteringBehavior.SetFilterPropName(col, nameof(List<string>.Capacity));
                DataGridFilteringBehavior.SetColumnPropGetter(col, funcColPropertyGetter);
                i++;
            }

        }

        public MainWindow()
        {
            InitializeComponent();

            DataGridFilteringBehavior.SetRowDataType(this.MainGrid, typeof(List<string?>));
            DataGridFilteringBehavior.SetHasFilters(this.MainGrid, true);

            // The grid is used to display spreadsheet type data, so the columns can vary.
            const int cols = 100;
            const int rows = 200;
            this.SetDataGridColumns(Enumerable.Range(0, cols).Select(x => $"Column{x + 1}"));
            this.RawData = Enumerable.Range(0, rows).Select(r =>
                Enumerable.Range(0, cols).Select(c => $"R{r + 1}C{c + 1}")
            );
        }
    }
}
