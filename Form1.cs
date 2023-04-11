using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace conway
{
    public partial class Form1 : Form
    {
        public int[,] map;
        public int shiftx = 0, shifty = 0;
        public CheckBox [,] check_map;
        public Form1()
        {
            map = new int[9, 10]
            {
                {0,0,0,0,0,0,0,0,1,0},
                {0,0,1,1,0,0,0,0,0,0},
                {0,0,0,1,0,0,0,0,0,0},
                {0,0,0,1,0,0,0,0,0,0},
                {0,0,0,1,0,1,0,0,0,0},
                {0,0,0,1,0,1,1,0,0,0},
                {0,1,1,0,0,0,1,1,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0}
            };
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckBox rad;
            check_map = new CheckBox[10,10];
            int offset_x = 600, offset_y = 200;
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    rad = new CheckBox() { Size = new Size(15, 15), Location = new Point(offset_x + 20 * i, offset_y + 20 * j) };
                    check_map[i, j] = rad;
                    this.Controls.Add(rad); 
                } 
        }

       
        protected override void OnPaint( PaintEventArgs pe)
        {
            Graphics g = pe.Graphics;
            //g.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(xx, yy, 30, 30));
            for (int j = 10, n = 0; n < map.GetLength(0); j += 50, n++ )
                for (int i = 10, k = 0; k < map.GetLength(1); i += 50, k++)
                {
                    g.FillRectangle(new SolidBrush(( map[n,k] == 1 ) ? Color.Black : Color.White), new Rectangle(i + this.AutoScrollPosition.X, j + this.AutoScrollPosition.Y, 45, 45));
                }
            var msg = new Label();
            msg.Name = "msg";
            msg.Size = new Size(1, 1);
            msg.Text = "x";
            msg.Location = new Point(50 + (map.GetLength(0) - 1) * 50, 50 + (map.GetLength(1) - 1) * 50);
            this.Controls.Add(msg);
        }
        protected override void OnScroll(ScrollEventArgs se) 
        {
            this.Refresh();
        }

        public int[,] change(int[,] cel, string rotation, string mode)
        {
            int height = cel.GetLength(0), width = cel.GetLength(1);
            if (rotation == "left" || rotation == "right")
                width += (mode == "add") ? 1 : -1;
            if (rotation == "up" || rotation == "down")
                height += (mode == "add") ? 1 : -1;
            int[,] ans = new int[height, width];
            if (mode == "add")
                for (int i = 0; i < cel.GetLength(0); i++)
                    for (int j = 0; j < cel.GetLength(1); j++)
                        ans[(rotation == "up") ? i + 1 : i, (rotation == "left") ? j + 1 : j] = cel[i, j];
            if (mode == "remove")
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        ans[i, j] = cel[(rotation == "up") ? i + 1 : i, (rotation == "left") ? j + 1 : j];
            return ans;
        }
        public int[,] extend(int[,] cel)
        {
            bool left = true, up = true, right = true, down = true;
            for (int i = 1, j = 0; i < cel.GetLength(0) - 1; i++)
                if (cel[i - 1, j] + cel[i, j] + cel[i + 1, j] == 3)
                {
                    if (left)
                    {
                        cel = change(cel, "left", "add");
                        left = false;
                        j++;
                        shiftx = 1;
                    }
                    cel[i, 0] = 1;
                }

            for (int i = 1, j = cel.GetLength(1) - 1; i < cel.GetLength(0) - 1; i++)
                if (cel[i - 1, j] + cel[i, j] + cel[i + 1, j] == 3)
                {
                    if (right)
                    {
                        cel = change(cel, "right", "add");
                        right = false;
                    }
                    cel[i, j + 1] = 1;
                }

            for (int i = 0, j = 1; j < cel.GetLength(1) - 1; j++)
                if (cel[i, j - 1] + cel[i, j] + cel[i, j + 1] == 3)
                {
                    if (up)
                    {
                        cel = change(cel, "up", "add");
                        up = false;
                        i++;
                        shifty = 1;
                    }
                    cel[0, j] = 1;
                }
            for (int i = cel.GetLength(0) - 1, j = 1; j < cel.GetLength(1) - 1; j++)
                if (cel[i, j - 1] + cel[i, j] + cel[i, j + 1] == 3)
                {
                    if (down)
                    {
                        cel = change(cel, "down", "add");
                        down = false;
                    }
                    cel[i + 1, j] = 1;
                }
            return cel;
        }
        public int[,] truncate(int[,] cel)
        {
            int buf, i, j;
            for (i = 0; i < cel.GetLength(0); )
            {
                for (j = 0, buf = 0; j < cel.GetLength(1); j++)
                    buf += cel[i, j];
                if (buf == 0)
                    cel = change(cel, "up", "remove");
                else
                    break;
            }
            for (i = cel.GetLength(0) - 1; i >= 0; i--)
            {
                for (j = 0, buf = 0; j < cel.GetLength(1); j++)
                    buf += cel[i, j];
                if (buf == 0)
                    cel = change(cel, "down", "remove");
                else
                    break;
            }
            for (j = 0; j < cel.GetLength(1);)
            {
                for (i = 0, buf = 0; i < cel.GetLength(0); i++)
                    buf += cel[i, j];
                if (buf == 0)
                    cel = change(cel, "left", "remove");
                else
                    break;
            }
            for (j = cel.GetLength(1) - 1; j >= 0; j--)
            {
                for (i = 0, buf = 0; i < cel.GetLength(0); i++)
                    buf += cel[i, j];
                if (buf == 0)
                    cel = change(cel, "right", "remove");
                else
                    break;
            }
            return cel;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            var del = this.Controls["msg"];
            this.Controls.Remove(del);
            
            int[,] buf = (int[,])map.Clone();
            int[,] cells = (int[,])map.Clone();
            int sum;
            shiftx = 0;
            shifty = 0;
            cells = extend(cells);
            for (int i = 0; i < buf.GetLength(0); i++)
                for (int j = 0; j < buf.GetLength(1); j++)
                {
                    sum = 0;
                    sum += (i == 0 || j == 0) ? 0 : buf[i - 1, j - 1];
                    sum += (i == 0) ? 0 : buf[i - 1, j];
                    sum += (i == 0 || j == buf.GetLength(1) - 1) ? 0 : buf[i - 1, j + 1];
                    sum += (j == buf.GetLength(1) - 1) ? 0 : buf[i, j + 1];
                    sum += (j == buf.GetLength(1) - 1 || i == buf.GetLength(0) - 1) ? 0 : buf[i + 1, j + 1];
                    sum += (i == buf.GetLength(0) - 1) ? 0 : buf[i + 1, j];
                    sum += (j == 0 || i == buf.GetLength(0) - 1) ? 0 : buf[i + 1, j - 1];
                    sum += (j == 0) ? 0 : buf[i, j - 1];
                    if (buf[i, j] == 0 && sum == 3)
                        cells[i + shifty, j + shiftx] = 1;
                    if (buf[i, j] == 1 && (sum > 3 || sum < 2))
                        cells[i + shifty, j + shiftx] = 0;
                }
            cells = truncate(cells);
            map = (int[,])cells.Clone();
            this.Invalidate(); 
             
        }


        private void button2_Click(object sender, EventArgs e)
        {
            CheckBox rad;
            map = new int[10, 10];
            for (int i = 0; i < 10; i++) 
                for (int j = 0; j < 10; j++)
                {
                    rad = check_map[j, i];
                    map[i, j] = (rad.Checked)? 1 : 0;
                }

            this.Invalidate(); 
            
        }
    }
}
