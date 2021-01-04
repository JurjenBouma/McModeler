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
using System.IO;
using System.Globalization;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MCModeler
{
    class FloatTexBox : TextBox
    {
        float _value;
        public float Min;
        public float Max;
        public float Increment;
        public int Decimals;
        public EventHandler ValueChanged;

        public float Value
        {
            get { return _value; }
            set { _value = value; Text = value.ToString(); }
        }
        public FloatTexBox()
        {
            _value = 0;
            Text = "0";
            Min = float.MinValue;
            Max = float.MaxValue;
            Increment = 0;
            Decimals = 15;
            this.MouseDoubleClick += new MouseEventHandler(Dubbel_Clicked);
            this.KeyPress += new KeyPressEventHandler(Key_Pressed);
            this.LostFocus += new EventHandler(ReadValue);
        }
        private void Dubbel_Clicked(object sender,MouseEventArgs e) { this.SelectAll();}
        private void Key_Pressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)(Keys.Return))
                this.Parent.Focus();
        }
        private void ReadValue(object sender, EventArgs e)
        {
            float newValue;
            try
            { newValue = Convert.ToSingle(Text, CultureInfo.CreateSpecificCulture("en-US")); }
            catch 
            {
                if (Text.Length == 0)
                    _value = 0;
                Text = _value.ToString(CultureInfo.CreateSpecificCulture("en-US"));
                return; 
            }
            if (Increment != 0)
                newValue -= newValue % Increment;
            if (newValue >= Min && newValue <= Max)
            {
                    _value = (float)Math.Round(newValue,Decimals);
                    Text = _value.ToString(CultureInfo.CreateSpecificCulture("en-US"));
                    if (ValueChanged != null)
                        ValueChanged(this, EventArgs.Empty);
            }
            else if (newValue < Min)
            {
                _value = Min;
                Text = _value.ToString(CultureInfo.CreateSpecificCulture("en-US"));
                if (ValueChanged != null)
                    ValueChanged(this, EventArgs.Empty);
            }
            else if (newValue > Max)
            {
                _value = Max;
                Text = _value.ToString(CultureInfo.CreateSpecificCulture("en-US"));
                if (ValueChanged != null)
                    ValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
