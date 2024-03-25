using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AI_Path_Visualizer
{
    public class Cell : Button
    {
        private CellType cellType;

        public CellType CellType
        {
            get { return cellType; }
            set
            {
                cellType = value;
                UpdateAppearance();
            }
        }

        public int GCost { get; set; }
        public int HCost { get; set; }
        public Cell Parent { get; set; }
        public int FCost { get { return GCost + HCost; } }

        public Cell()
        {
            this.PreviewMouseLeftButtonDown += Cell_PreviewMouseLeftButtonDown;
        }


        private void Cell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            switch (mainWindow.currentMode)
            {
                case Mode.PlacingStart:
                    if (mainWindow.startCell != null)
                    {
                        mainWindow.startCell.CellType = CellType.Empty;
                    }
                    CellType = CellType.Start;
                    mainWindow.startCell = this;
                    break;
                case Mode.PlacingEnd:
                    if (mainWindow.endCell != null)
                    {
                        mainWindow.endCell.CellType = CellType.Empty;
                    }
                    CellType = CellType.End;
                    mainWindow.endCell = this;
                    break;
                case Mode.PlacingWalls:
                    CellType = CellType.Wall;
                    break;
            }
        }


        private void UpdateAppearance()
        {
            switch (CellType)
            {
                case CellType.Start:
                    this.Background = Brushes.Red;
                    break;
                case CellType.End:
                    this.Background = Brushes.Green;
                    break;
                case CellType.Wall:
                    this.Background = Brushes.Blue;
                    break;
                case CellType.Path:
                    this.Background = Brushes.Orange;
                    break;
                default:
                    this.Background = Brushes.White;
                    break;
            }
        }
    }
    public enum CellType
    {
        Empty,
        Start,
        End,
        Wall,
        Path
    }
    public enum Mode
    {
        PlacingStart,
        PlacingEnd,
        PlacingWalls
    }


}
