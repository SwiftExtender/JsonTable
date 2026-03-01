using Avalonia.Controls;

namespace TableJson.Views
{
    public partial class MacrosCodeWindow : Window
    {
        public MacrosCodeWindow()
        {
            InitializeComponent();
            DataGrid grid = GetGrid();
        }
        private DataGrid GetGrid()
        {
            DataGrid grid = this.FindControl<DataGrid>("mgrid");
            grid.SelectionMode = DataGridSelectionMode.Single;
            return grid;
        }
    }
}
