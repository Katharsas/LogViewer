using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace LogViewerGui.view
{
    /// <summary>
    /// Interaction logic for LogViewData.xaml
    /// </summary>
    public partial class LogViewData : UserControl
    {
        public LogViewData()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Allows to apply cell template to EVERY cell, even if rows cannot be specified in XAML,
        /// because complete DataTable including rows is dynamic (see LogAtomConverter in LogViewer Assembly).
        /// </summary>
        private void DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var column = new DataRowColumn(e.PropertyName);
            column.Header = e.Column.Header;
            column.CellTemplate = (DataTemplate)Resources["MyTemplate1"];
            e.Column = column;
        }

        public class DataRowColumn : DataGridTemplateColumn
        {
            public DataRowColumn(string column) { ColumnName = column; }
            public string ColumnName { get; private set; }
            protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
            {
                var row = (DataRowView)dataItem;
                var item = row[ColumnName];
                cell.DataContext = item;
                var element = base.GenerateElement(cell, item);
                return element;
            }
        }
    }
}
