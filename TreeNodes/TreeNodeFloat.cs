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
using System.Globalization;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;


namespace MCModeler.TreeNodes
{
    public class TreeNodeFloat : ControlTreeNode
    {
        public TreeNodeFloat(string name, float value)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + ((float)Tag).ToString();
            SetControl(float.MinValue, float.MaxValue,0);
        }
        public TreeNodeFloat(string name, float value, float min, float max,float increment)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + ((float)Tag).ToString(CultureInfo.CreateSpecificCulture("en-US"));
            SetControl(min, max,increment);
        }
        private void SetControl(float min, float max,float increment)
        {
            FloatTexBox floatBox = new FloatTexBox();
            floatBox.Max = max;
            floatBox.Min = min;
            floatBox.Increment = increment;
            floatBox.Value = ((float)Tag);
            floatBox.ValueChanged += new EventHandler(OnValueChanged);
            Control = floatBox;
        }
        public void UpdateValue(float value)
        {
            Tag = value;
            Text = Name + ": " + ((float)Tag).ToString(CultureInfo.CreateSpecificCulture("en-US"));
            FloatTexBox floatBox = (FloatTexBox)Control;
            floatBox.Value = ((float)Tag);
        }
        private void OnValueChanged(object sender, EventArgs e)
        {
            FloatTexBox floatBox = (FloatTexBox)Control;
            Tag = floatBox.Value;
            Text = Name + ": " + ((float)Tag).ToString(CultureInfo.CreateSpecificCulture("en-US"));
            if (ValueChanged != null)
                ValueChanged(this);
        }
    }
}