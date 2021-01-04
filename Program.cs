using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Windows;

namespace MCModeler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Configuration.EnableObjectTracking = true;
            var form1 = new MainForm();
            Renderer.Initialize3D(form1.panelRender.Handle, form1.panelRender.Width, form1.panelRender.Height);
            form1.Initialize3D();
            MessagePump.Run(form1, Renderer.RenderFrame);
            Renderer.CleanUp();
        }
    }
}
