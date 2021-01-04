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
    public class TreeNodeString : ControlTreeNode
    {
        public TreeNodeString(string name, string value)
        {
            Configurate(name, value);
        }
        private void Configurate(string name, string value)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + (string)Tag;
            TextBox textBox = new TextBox();
            textBox.Text = (string)Tag;
            textBox.TextChanged += new EventHandler(OnValueChanged);
            Control = textBox;
          
        }
        public void UpdateValue(string value)
        {
            Tag = value;
            Text = Name + ": " + (string)Tag;
            TextBox textBox = (TextBox)Control;
            textBox.Text = value;
        }
        private void OnValueChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)Control;
            Tag = textBox.Text;
            Text = Name + ": " + (string)Tag;
            if (ValueChanged != null)
                ValueChanged(this);
        }
    }
}
