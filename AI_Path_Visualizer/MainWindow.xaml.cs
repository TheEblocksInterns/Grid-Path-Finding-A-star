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

namespace AI_Path_Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>.
    /// 
    public partial class MainWindow : Window
    {
        public double dpiX;
        public double dpiY;
        public Cell startCell;
        public Cell endCell;
        public Mode currentMode;
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(this);
            dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
        }

      

        private void PlaceStart_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.PlacingStart;
            modeTextBlock.Text = "Current mode: Placing Start Point";
        }

        private void PlaceEnd_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.PlacingEnd;
            modeTextBlock.Text = "Current mode: Placing End Point";
        }

        private void PlaceWalls_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.PlacingWalls;
            modeTextBlock.Text = "Current mode: Placing Walls";
        }

        private void CreateGrid_Click(object sender, RoutedEventArgs e)
        {
            int width = int.Parse(widthTextBox.Text);
            int height = int.Parse(heightTextBox.Text);

            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();

            double cmInPixelsX = dpiX / 2.54; // there are 2.54 cm in an inch
            double cmInPixelsY = dpiY / 2.54;

            for (int i = 0; i < width; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cmInPixelsX) });
            }

            for (int i = 0; i < height; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cmInPixelsY) });
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Cell cell = new Cell { CellType = CellType.Empty };
                    Grid.SetColumn(cell, i);
                    Grid.SetRow(cell, j);
                    grid.Children.Add(cell);
                }
            }
        }

        private void FindPath()
        {
            // Initialize the open list with the start cell and the closed list as empty
            List<Cell> openList = new List<Cell> { startCell };
            List<Cell> closedList = new List<Cell>();

            while (openList.Count > 0)
            {
                // Get the cell from the open list with the lowest F cost (G cost + H cost)
                Cell currentCell = GetCellWithLowestFCost(openList);

                // If the current cell is the end cell, we've found a path
                if (currentCell == endCell)
                {
                    // Trace back the path from the end cell to the start cell via each cell's parent
                    Cell cell = endCell;
                    while (cell != startCell)
                    {
                        cell.CellType = CellType.Path;
                        cell = cell.Parent;
                    }
                    return;
                }

                // Otherwise, move the current cell from the open list to the closed list
                openList.Remove(currentCell);
                closedList.Add(currentCell);

                // Check all the current cell's neighbors
                foreach (Cell neighbor in GetNeighbors(currentCell))
                {
                    // If the neighbor is a wall or is in the closed list, ignore it
                    if (neighbor.CellType == CellType.Wall || closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    // If the neighbor is not in the open list or the current path is shorter, update the neighbor
                    if (!openList.Contains(neighbor) || currentCell.GCost + 1 < neighbor.GCost)
                    {
                        neighbor.GCost = currentCell.GCost + 1;
                        neighbor.HCost = GetDistance(neighbor, endCell);
                        neighbor.Parent = currentCell;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
        }

        private List<Cell> GetNeighbors(Cell cell)
        {
            List<Cell> neighbors = new List<Cell>();

            int[,] directions = new int[,]
            {
                { -1, 0 }, // Up
                { 1, 0 }, // Down
                { 0, -1 }, // Left
                { 0, 1 } // Right
            };

            for (int i = 0; i < 4; i++)
            {
                int dx = directions[i, 0];
                int dy = directions[i, 1];

                int newX = Grid.GetColumn(cell) + dx;
                int newY = Grid.GetRow(cell) + dy;

                if (newX >= 0 && newX < grid.ColumnDefinitions.Count &&
                    newY >= 0 && newY < grid.RowDefinitions.Count)
                {
                    Cell neighbor = (Cell)grid.Children[newX * grid.RowDefinitions.Count + newY];
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private Cell GetCellWithLowestFCost(List<Cell> cells)
        {
            Cell cellWithLowestFCost = cells[0];

            foreach (Cell cell in cells)
            {
                if (cell.FCost < cellWithLowestFCost.FCost)
                {
                    cellWithLowestFCost = cell;
                }
            }

            return cellWithLowestFCost;
        }
        private int GetDistance(Cell cell1, Cell cell2)
        {
            int dx = Math.Abs(Grid.GetColumn(cell1) - Grid.GetColumn(cell2));
            int dy = Math.Abs(Grid.GetRow(cell1) - Grid.GetRow(cell2));

            return dx + dy;
        }

        private void Find_shortPath(object sender, RoutedEventArgs e)
        {
            FindPath();
        }
    }


}
