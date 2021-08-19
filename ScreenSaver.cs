using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ScreenSaver
{
    public partial class ScreenSaver : Form

    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        private Point mouseLocation;
        private bool previewMode = false;
        private Random rand = new Random();


        public ScreenSaver()
        {
            InitializeComponent();
        }

        public ScreenSaver(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
        }

        public ScreenSaver(IntPtr PreviewWndHandle)
        {

            InitializeComponent();
            this.Bounds = Bounds;


            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);

            // Make text smaller
            textLabel.Font = new System.Drawing.Font("Comic Sans MS", 6);

            previewMode = true;


        }



          private void ScreenSaver_Load(object sender, EventArgs e)
        {

            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Costi0n_ScreenSaver");

            if (key == null)
                textLabel.Text = "TOTEM ASL RIETI - TOCCA LO SCHERMO PER ATTIVARE";
            else
                textLabel.Text = (string)key.GetValue("text");

            Cursor.Hide();
            TopMost = true;

            moveTimer.Interval = 3000;
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            moveTimer.Start();


        }

        private void ScreenSaver_MouseMove(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                if (!mouseLocation.IsEmpty)
                {
                    // Terminate if mouse is moved a significant distance
                    if (Math.Abs(mouseLocation.X - e.X) > 5 ||
                        Math.Abs(mouseLocation.Y - e.Y) > 5)
                        Application.Exit();
                }
                 // Update current mouse location
                 mouseLocation = e.Location;
            }

        }

        private void ScreenSaver_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void ScreenSaver_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void moveTimer_Tick(object sender, EventArgs e)
        {
            textLabel.Left = rand.Next(Math.Max(1, Bounds.Width - textLabel.Width));
            textLabel.Top = rand.Next(Math.Max(1, Bounds.Height - textLabel.Height));
        }
    }
}
