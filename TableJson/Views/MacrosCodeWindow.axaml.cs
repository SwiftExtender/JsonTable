using Avalonia.Controls;
using System.Linq;

namespace TableJson.Views
{
    public partial class MacrosCodeWindow : Window
    {
        public MacrosCodeWindow()
        {
            InitializeComponent();
            DataGrid grid = GetGrid();
            //if (grid.Source.Items.Count() > 0)
            //{

            //}
            //grid.RowSelection.SelectedItem
            //var initRow = GetGridActiveRow();
        }
        private DataGrid GetGrid()
        {
            DataGrid grid = this.FindControl<DataGrid>("mgrid");
            return grid;
        }
    }
}
