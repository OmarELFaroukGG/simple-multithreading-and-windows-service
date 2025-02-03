using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawCircleThreading
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Run(() => DrawCircle());
            Task.Run(() => DrawRectangle());
        }
        private void DrawCircle()
        {
            // Simulate some processing
            Thread.Sleep(1000);

            // Draw circle
            using (Graphics g = CreateGraphics())
            {
                g.FillEllipse(Brushes.Green, new Rectangle(50, 50, 100, 100));
            }

            // Update UI on the main thread
            UpdateUI(() => Text = "Circle Drawn");
        }

        private void DrawRectangle()
        {
            // Simulate some processing
            Thread.Sleep(2000);

            // Draw rectangle
            using (Graphics g = CreateGraphics())
            {
                g.FillRectangle(Brushes.Pink, new Rectangle(200, 50, 100, 100));
            }

            // Update UI on the main thread
            UpdateUI(() => Text = "Rectangle Drawn");
        }

        private void UpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                // Invoke on the main thread
                BeginInvoke(action);
            }
            else
            {
                // Execute directly if on the main thread
                action.Invoke();
            }
        }
    }
}
