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
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.Direct2D;
using SlimDX.Windows;
using SlimDX.DXGI;
using SlimDX.D3DCompiler;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;


namespace MCModeler
{
    class CoordinateGrid
    {
        public bool Visible;
        List<Line> _linesGrid;
        List<Line> _linesCompass;
        public bool DrawCompass;

        public CoordinateGrid(Color4 color)
        {
            Visible = true;
            DrawCompass = true;
            AddGrid(color);
            AddCompass(color);
        }
        private void AddCompass(Color4 color)
        {
            _linesCompass = new List<Line>();
            Color4 compassColor = new Color4(0.3f, color.Red, color.Green * 0.95f, color.Blue * 0.95f);
            //N
            _linesCompass.Add(new Line(new Vector3(7.4f, 0, -9), new Vector3(7.4f, 0, -7), compassColor));
            _linesCompass.Add(new Line(new Vector3(8.6f, 0, -9), new Vector3(8.6f, 0, -7), compassColor));
            _linesCompass.Add(new Line(new Vector3(7.4f, 0, -9), new Vector3(8.6f, 0, -7), compassColor));
            
            //S
            _linesCompass.Add(new Line(new Vector3(7.4f, 0, 23), new Vector3(8.6f, 0, 23), compassColor));
            _linesCompass.Add(new Line(new Vector3(7.4f, 0, 24), new Vector3(8.6f, 0, 24), compassColor));
            _linesCompass.Add(new Line(new Vector3(7.4f, 0, 25), new Vector3(8.6f, 0, 25), compassColor));
            _linesCompass.Add(new Line(new Vector3(7.4f, 0, 23), new Vector3(7.4f, 0, 24), compassColor));
            _linesCompass.Add(new Line(new Vector3(8.6f, 0, 24), new Vector3(8.6f, 0, 25), compassColor));

            //E
            _linesCompass.Add(new Line(new Vector3(23.4f, 0, 7.2f), new Vector3(23.4f, 0, 9.2f), compassColor));
            _linesCompass.Add(new Line(new Vector3(23.4f, 0, 7.2f), new Vector3(24.6f, 0, 7.2f), compassColor));
            _linesCompass.Add(new Line(new Vector3(23.4f, 0, 8.2f), new Vector3(24.6f, 0, 8.2f), compassColor));
            _linesCompass.Add(new Line(new Vector3(23.4f, 0, 9.2f), new Vector3(24.6f, 0, 9.2f), compassColor));

            //W
            _linesCompass.Add(new Line(new Vector3(-9f, 0, 7), new Vector3(-8.6f, 0, 9), compassColor));
            _linesCompass.Add(new Line(new Vector3(-8.5f, 0, 9), new Vector3(-8.0f, 0, 8.3f), compassColor));
            _linesCompass.Add(new Line(new Vector3(-8.0f, 0, 8.3f), new Vector3(-7.5f, 0, 9), compassColor));
            _linesCompass.Add(new Line(new Vector3(-7.5f, 0, 9), new Vector3(-7.0f, 0, 7), compassColor));
        }
        private void AddGrid(Color4 color)
        {
            _linesGrid = new List<Line>();
            for (int i = -16; i <= 32; i += 16)//FullBlockX
            {
                _linesGrid.Add(new Line(new Vector3(i, 0, -16), new Vector3(i, 0, 32), color));
            }
            for (int i = -16; i <= 32; i += 16)//FullBlockZ
            {
                _linesGrid.Add(new Line(new Vector3(-16, 0, i), new Vector3(32, 0, i), color));
            }

            for (int i = -16; i <= 32; i += 4)//QuarterBlockX
            {
                _linesGrid.Add(new Line(new Vector3(i, 0, -16), new Vector3(i, 0, 32), new Color4(0.25f, color.Red, color.Green, color.Blue)));
            }
            for (int i = -16; i <= 32; i += 4)//QuarterBlockZ
            {
                _linesGrid.Add(new Line(new Vector3(-16, 0, i), new Vector3(32, 0, i), new Color4(0.25f, color.Red, color.Green, color.Blue)));
            }

            for (int i = -16; i <= 32; i += 1)//unitX
            {
                _linesGrid.Add(new Line(new Vector3(i, 0, -16), new Vector3(i, 0, 32), new Color4(0.05f, color.Red, color.Green, color.Blue)));
            }
            for (int i = -16; i <= 32; i += 1)//unitZ
            {
                _linesGrid.Add(new Line(new Vector3(-16, 0, i), new Vector3(32, 0, i), new Color4(0.05f, color.Red, color.Green, color.Blue)));
            }
        }

        public void Draw()
        {
            if (Visible)
            {
                foreach (Line line in _linesGrid)
                    line.Draw();
                if(DrawCompass)
                    foreach (Line line in _linesCompass)
                        line.Draw();
            }
        }

        public void Dispose()
        {
            foreach (Line line in _linesGrid)
                line.Dispose();
            foreach (Line line in _linesCompass)
                line.Dispose();
        }
    }
}
