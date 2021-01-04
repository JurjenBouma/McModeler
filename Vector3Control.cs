﻿using System;
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
using System.IO;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MCModeler
{
    class Vector3Control : Panel
    {
        Vector3 _value;
        Label labelX;
        Label labelY;
        Label labelZ;
        FloatTexBox floatBoxX;
        FloatTexBox floatBoxY;
        FloatTexBox floatBoxZ;
        Size defaultSize;
        float defaultFontSize = 6.4f;

        public EventHandler ValueChanged;

        public Vector3 Value
        {

            get { return _value; }
            set 
            {
                floatBoxX.Value = value.X;
                floatBoxY.Value = value.Y;
                floatBoxZ.Value = value.Z;
                _value = value; 
            }
        }

        public Vector3Control()
        {
            Configurate(float.MinValue,float.MaxValue,15);
        }
        public Vector3Control(float min ,float max,int decimals)
        {
            Configurate(min,max,decimals);
        }

        private void Configurate(float min,float max, int decimals)
        {
            labelX = new Label();
            labelX.Text = "X:";
            labelX.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, defaultFontSize, FontStyle.Regular);
            labelX.TextAlign = ContentAlignment.MiddleLeft;
            labelX.Location = new Point(0, 0);
            labelX.Size = new Size(14, 20);

            floatBoxX = new FloatTexBox();
            floatBoxX.Location = new Point(labelX.Location.X + labelX.Size.Width, 0);
            floatBoxX.Size = new Size(20, 20);
            floatBoxX.Min = min;
            floatBoxX.Max = max;
            floatBoxX.Decimals = decimals;
            floatBoxX.ValueChanged += new EventHandler(OnValueChanged);

            labelY = new Label();
            labelY.Text = "Y:";
            labelY.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, defaultFontSize, FontStyle.Regular);
            labelY.TextAlign = ContentAlignment.MiddleLeft;
            labelY.Location = new Point(floatBoxX.Location.X + floatBoxX.Size.Width, 0);
            labelY.Size = new Size(14, 20);

            floatBoxY = new FloatTexBox();
            floatBoxY.Location = new Point(labelY.Location.X + labelY.Size.Width, 0);
            floatBoxY.Size = new Size(20, 20);
            floatBoxY.Min = min;
            floatBoxY.Max = max;
            floatBoxY.Decimals = decimals;
            floatBoxY.ValueChanged += new EventHandler(OnValueChanged);

            labelZ = new Label();
            labelZ.Text = "Z:";
            labelZ.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, defaultFontSize, FontStyle.Regular);
            labelZ.TextAlign = ContentAlignment.MiddleLeft;
            labelZ.Location = new Point(floatBoxY.Location.X + floatBoxY.Size.Width, 0);
            labelZ.Size = new Size(14, 20);

            floatBoxZ = new FloatTexBox();
            floatBoxZ.Location = new Point(labelZ.Location.X + labelZ.Size.Width, 0);
            floatBoxZ.Size = new Size(20, 20);
            floatBoxZ.Min = min;
            floatBoxZ.Max = max;
            floatBoxZ.Decimals = decimals;
            floatBoxZ.ValueChanged += new EventHandler(OnValueChanged);

            this.Controls.Add(labelX);
            this.Controls.Add(floatBoxX);
            this.Controls.Add(labelY);
            this.Controls.Add(floatBoxY);
            this.Controls.Add(labelZ);
            this.Controls.Add(floatBoxZ);

            this.Height = 20;
            this.Width = floatBoxZ.Location.X + floatBoxZ.Size.Width;
            this.SetBounds(0, 0, Width, Height);
            defaultSize = this.Size;

            this.SizeChanged += new EventHandler(ResizeControls);
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            _value.X = floatBoxX.Value;
            _value.Y = floatBoxY.Value;
            _value.Z = floatBoxZ.Value;

            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        private void ResizeControls(object sender ,EventArgs e)
        {
            float widthRatio = (float)this.Width / defaultSize.Width;

            labelX.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, defaultFontSize * widthRatio, FontStyle.Regular);
            labelX.Location = new Point(0, 0);
            labelX.Size = new Size((int)(15 * widthRatio), 20);

            floatBoxX.Location = new Point(labelX.Location.X + labelX.Size.Width, 0);
            floatBoxX.Size = new Size((int)(20 * widthRatio),20 );

            labelY.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, defaultFontSize *  widthRatio, FontStyle.Regular);
            labelY.Location = new Point(floatBoxX.Location.X + floatBoxX.Size.Width, 0);
            labelY.Size = new Size((int)(15 * widthRatio), 20);

            floatBoxY.Location = new Point(labelY.Location.X + labelY.Size.Width, 0);
            floatBoxY.Size = new Size((int)(20 * widthRatio), 20 );

            labelZ.Font = new System.Drawing.Font(FontFamily.GenericSansSerif, defaultFontSize * widthRatio, FontStyle.Regular);
            labelZ.Location = new Point(floatBoxY.Location.X + floatBoxY.Size.Width, 0);
            labelZ.Size = new Size((int)(15 * widthRatio), 20);

            floatBoxZ.Location = new Point(labelZ.Location.X + labelZ.Size.Width, 0);
            floatBoxZ.Size = new Size(Width-(labelZ.Location.X+ labelZ.Width), 20 );
        }
    }
}
