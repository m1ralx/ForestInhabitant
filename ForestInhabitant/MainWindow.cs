using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForestInhabitant
{
    public partial class MainWindow : Form
    {
        public TableLayoutPanel LOut { get; set; }
        public IForest CurForest { get; set; }
        public ForestCell[,] Map { get; set; }
        public List<Inhabitant> Inhabitants { get; set; }
        public MainWindow(IForest forest, List<Inhabitant> inhabitants)
        {
            InitializeComponent();
            this.Text = "ForestInhabitant";
            this.MinimumSize = new Size(700, 420);
            this.Size = new Size(800, 600);
            LOut = new TableLayoutPanel();
            LOut.Parent = this;
            LOut.Dock = DockStyle.Fill;
            CurForest = forest;
            Inhabitants = inhabitants;
            //Map = map;
            FillLOut();
        }
        private Label[,] Labels { get; set; }
        public void UpdateLOut()
        {
            LOut.Controls.Clear();
            LOut.Margin = new Padding(0, 0, 0, 0);
            var width = CurForest.Width;
            var height = CurForest.Height;
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    Label label;
                    if (Labels[x, y] == null)
                        label = new Label();
                    else
                        label = Labels[x, y];
                    label.Parent = LOut;
                    //label.Anchor = AnchorStyles.None;
                    label.Font = new Font(FontFamily.GenericSerif, 40);
                    label.Size = new Size(60, 60);
                    
                    label.Text = CurForest[new Point(x, y)].GetSymbol();
                    label.BackColor = Color.Bisque;
                    LOut.Controls.Add(label, x, y);
                }
        }
        public void FillLOut()
        {
            var width = CurForest.Width;
            var height = CurForest.Height;
            Labels = new Label[width, height];
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    var label = new Label();
                    LOut.Controls.Add(label, x, y);
                }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var moves = new Dictionary<Keys, Point>
            {
                { Keys.Left, new Point(-1, 0)},
                { Keys.Right, new Point(1, 0)},
                { Keys.Up, new Point(0, -1)},
                { Keys.Down, new Point(0, 1)}
            };
            try
            {
                CurForest.MoveInhabitant(moves[e.KeyCode], Inhabitants.First(inh => inh.GetHitPoints() > 0));
            }
            catch
            {
            }
            
        }
    }
}
