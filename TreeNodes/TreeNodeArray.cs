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
    public class TreeNodeArray : ControlTreeNode
    {
        public TreeNodeArray(string name, object value, Array options)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + Tag.ToString();
            ComboBox comboBox = new ComboBox();
            foreach (object obj in options)
                comboBox.Items.Add(obj);
            comboBox.SelectedItem = value;
            comboBox.SelectedValueChanged += new EventHandler(OnValueChanged);
            Control = comboBox;
        }
        public void UpdateOptions(Array options)
        {
            ComboBox combo = (ComboBox)Control;
            combo.Items.Clear();
             foreach (object obj in options)
                 combo.Items.Add(obj);
        }
        public void UpdateValue(object value)
        {
            Tag = value;
            Text = Name + ": " + Tag.ToString();
            ComboBox combo = (ComboBox)Control;
            combo.SelectedItem = Tag;
        }
        private void OnValueChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)Control;
            Tag = combo.SelectedItem;
            Text = Name + ": " + Tag.ToString();
            if (ValueChanged != null)
                ValueChanged(this);
        }
    }
}