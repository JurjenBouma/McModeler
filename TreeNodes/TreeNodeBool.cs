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
    public class TreeNodeBool: ControlTreeNode
    {
        public TreeNodeBool(string name, bool value)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + ((bool)Tag).ToString();     
            ComboBox dropDown = new ComboBox();
            dropDown.Items.Add(true);
            dropDown.Items.Add(false);
            dropDown.SelectedItem = (bool)Tag;
            dropDown.SelectedValueChanged += new EventHandler(OnValueChanged);
            Control = dropDown;
            this.ImageIndex = 3;
        }
        public void UpdateValue(bool value)
        {
            Tag = value;
            Text = Name + ": " + ((bool)Tag).ToString();
            ComboBox dropDown = (ComboBox)Control;
            dropDown.SelectedItem = Tag;
            this.ImageIndex = 3;
        }
        private void OnValueChanged(object sender, EventArgs e)
        {
            ComboBox dropDown = (ComboBox)Control;
            Tag = (bool)dropDown.SelectedItem;
            Text = Name + ": " + ((bool)Tag).ToString();
            if (ValueChanged != null)
                ValueChanged(this);
        }
    }
}