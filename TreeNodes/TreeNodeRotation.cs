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
using MCModeler.TreeNodes;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MCModeler.TreeNodes
{
    public class TreeNodeRotation: ControlTreeNode
    {
        ElementRotation _linkedRotation;
        public TreeNodeRotation(ElementRotation rot) : base("Rotation")
        {
             Link(rot);
             Configurate();
        }

        private void Configurate()
        {
            TreeNodeArray axisNode = new TreeNodeArray("Axis", _linkedRotation.Axis, Enum.GetValues(typeof(Axis)));
            axisNode.ValueChanged += new TreeNodeValueChangedHandler(On_Axis_Changed);

            TreeNodeFloat angleNode = new TreeNodeFloat("Angle", _linkedRotation.Angle, -45, 45, 22.5f);
            angleNode.ValueChanged += new TreeNodeValueChangedHandler(On_Angle_Changed);

            TreeNodeVector3 originNode = new TreeNodeVector3("Origin", _linkedRotation.Origin);
            originNode.ValueChanged += new TreeNodeValueChangedHandler(On_Origin_Changed);

            TreeNodeBool rescaleNode = new TreeNodeBool("Rescale", _linkedRotation.Rescale);
            rescaleNode.ValueChanged += new TreeNodeValueChangedHandler(On_Rescale_Changed);

            Nodes.Add(axisNode);
            Nodes.Add(angleNode);
            Nodes.Add(originNode);
            Nodes.Add(rescaleNode);
        }

        public void UpdateValues()
        {
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "Axis")
                {
                    TreeNodeArray axNode = (TreeNodeArray)node;
                    axNode.UpdateValue(_linkedRotation.Axis);
                }
                if (node.Name == "Angle")
                {
                    TreeNodeFloat angNode = (TreeNodeFloat)node;
                    angNode.UpdateValue(_linkedRotation.Angle);
                }
                  if (node.Name == "Origin")
                {
                    TreeNodeVector3 orNode = (TreeNodeVector3)node;
                    orNode.UpdateValue(_linkedRotation.Origin);
                }
                  if (node.Name == "Rescale")
                {
                    TreeNodeBool rescaleNode = (TreeNodeBool)node;
                    rescaleNode.UpdateValue(_linkedRotation.Rescale);
                }
            }
        }

        public void Link(ElementRotation rot)
        {
            _linkedRotation = rot;
        }

        private void On_Axis_Changed(ControlTreeNode node) { _linkedRotation.Axis = (Axis)node.Tag; }
        private void On_Angle_Changed(ControlTreeNode node) { _linkedRotation.Angle = (float)node.Tag; }
        private void On_Origin_Changed(ControlTreeNode node) { _linkedRotation.Origin = (Vector3)node.Tag; }
        private void On_Rescale_Changed(ControlTreeNode node) { _linkedRotation.Rescale = (bool)node.Tag; }
    }
}
