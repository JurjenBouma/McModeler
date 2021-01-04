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


namespace MCModeler.TreeNodes
{
    public class TreeNodeInt: ControlTreeNode
    {
        public TreeNodeInt(string name, int value)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + ((int)Tag).ToString();
            SetControl((decimal)int.MinValue, (decimal)int.MaxValue, 1, false);
        }
        public TreeNodeInt(string name, int value, int min, int max, int increment, bool readOnly)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + ((int)Tag).ToString();
            SetControl((decimal)min, (decimal)max, (decimal)increment, readOnly);
        }
        private void SetControl(decimal min, decimal max, decimal increment, bool readOnly)
        {
            NumericUpDown numeric = new NumericUpDown();
            numeric.Maximum = max;
            numeric.Minimum = min;
            numeric.Value = (decimal)((int)Tag);
            numeric.Increment = increment;
            numeric.ReadOnly = readOnly;
            numeric.ValueChanged += new EventHandler(OnValueChanged);
            Control = numeric;
        }
        public void UpdateValue(int value)
        {
            Tag = value;
            Text = Name + ": " + ((int)Tag).ToString();
            NumericUpDown numeric = (NumericUpDown)Control;
            numeric.Value = (int)Tag;
        }
        private void OnValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numeric = (NumericUpDown)Control;
            Tag = (int)Math.Round(numeric.Value, 0, MidpointRounding.AwayFromZero);
            Text = Name + ": " + (Tag).ToString();
            if (ValueChanged != null)
                ValueChanged(this);
        }
    }
}