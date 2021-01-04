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
    public class TreeNodeVector4: ControlTreeNode
    {
        public TreeNodeVector4(string name, Vector4 value)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + ((Vector4)Tag).ToString();
            SetControl(float.MinValue,float.MaxValue,15);
            RescaleControl = false;
        }
        public TreeNodeVector4(string name, Vector4 value,float min,float max,int decimals)
        {
            Tag = value;
            Name = name;
            Text = Name + ": " + ((Vector4)Tag).ToString();
            SetControl(min,max,decimals);
            RescaleControl = false;
        }
        private void SetControl(float min, float max, int decimals)
        {
            Vector4Control vectorBox = new Vector4Control(min,max,decimals);
            vectorBox.Value = ((Vector4)Tag);
            vectorBox.ValueChanged += new EventHandler(OnValueChanged);
            Control = vectorBox;
        }
        public void UpdateValue(Vector4 value)
        {
            Tag = value;
            Text = Name + ": " + ((Vector4)Tag).ToString();
            Vector4Control vectorBox = (Vector4Control)Control;
            vectorBox.Value = (Vector4)Tag;
        }
        private void OnValueChanged(object sender, EventArgs e)
        {
            Vector4Control vectorBox = (Vector4Control)Control;
            Tag = vectorBox.Value;
            Text = Name + ": " + ((Vector4)Tag).ToString();
            if (ValueChanged != null)
                ValueChanged(this);
        }
    }
}