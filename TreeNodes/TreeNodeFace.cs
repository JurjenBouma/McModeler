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
    public class TreeNodeFace : ControlTreeNode
    {
        CubeFace _linkedFace;
        public TreeNodeFace(CubeFace face):base(face.Orientation.ToString())
        {
            Link(face);
            Configurate();
        }

        private void Configurate()
        {
            TreeNodeBool visableNode = new TreeNodeBool("Visable", _linkedFace.Visible);
            visableNode.ValueChanged += new TreeNodeValueChangedHandler(On_Vis_Changed);

            TreeNodeArray texNode = new TreeNodeArray("Texture", _linkedFace.TextureName.Substring(1), _linkedFace.ParentBox.ParentModel.GetTextureNames());
            texNode.ValueChanged += new TreeNodeValueChangedHandler(On_Tex_Changed);

            TreeNodeVector4 uVNode = new TreeNodeVector4("UV", _linkedFace.UV);
            uVNode.ValueChanged += new TreeNodeValueChangedHandler(On_Uv_Changed);

            TreeNodeInt rotationNode = new TreeNodeInt("Rotation", _linkedFace.Rotation, 0, 270, 90, true);
            rotationNode.ValueChanged += new TreeNodeValueChangedHandler(On_Rot_Changed);

            TreeNodeArray cullNode = new TreeNodeArray("CullFace", _linkedFace.CullFace, Enum.GetValues(typeof(CullFace)));
            cullNode.ValueChanged += new TreeNodeValueChangedHandler(On_Cull_Changed);

            TreeNodeInt tintIndexNode = new TreeNodeInt("TintIndex", _linkedFace.TintIndex, -1, 10, 1, true);
            tintIndexNode.ValueChanged += new TreeNodeValueChangedHandler(On_Tint_Changed);

            Nodes.Add(visableNode);
            Nodes.Add(texNode);
            Nodes.Add(uVNode);
            Nodes.Add(rotationNode);
            Nodes.Add(cullNode);
            Nodes.Add(tintIndexNode);
        }

        public void UpdateValues()
        {
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "Visable")
                {
                    TreeNodeBool visableNode = (TreeNodeBool)node;
                    visableNode.UpdateValue(_linkedFace.Visible);
                }
                if (node.Name == "Texture")
                {
                    TreeNodeArray texNode = (TreeNodeArray)node;
                    texNode.UpdateValue(_linkedFace.TextureName.Substring(1));
                }
                if (node.Name == "UV")
                {
                    TreeNodeVector4 uvNode = (TreeNodeVector4)node;
                    uvNode.UpdateValue(_linkedFace.UV);
                }
                if (node.Name == "Rotation")
                {
                    TreeNodeInt rotNode = (TreeNodeInt)node;
                    rotNode.UpdateValue(_linkedFace.Rotation);
                }
                if (node.Name == "CullFace")
                {
                    TreeNodeArray cullNode = (TreeNodeArray)node;
                    cullNode.UpdateValue(_linkedFace.CullFace);
                }
                if (node.Name == "TintIndex")
                {
                    TreeNodeInt tintNode = (TreeNodeInt)node;
                    tintNode.UpdateValue(_linkedFace.TintIndex);
                }
            }
        }

        public void UpdateTextureOptions()
        {
            foreach (ControlTreeNode node in Nodes)
                if (node.Name == "Texture")
                {
                    TreeNodeArray texNode = (TreeNodeArray)node;
                    texNode.UpdateOptions(_linkedFace.ParentBox.ParentModel.GetTextureNames());
                    texNode.UpdateValue(_linkedFace.TextureName.Substring(1));
                }
        }
        public void Link(CubeFace face)
        {
            _linkedFace = face;
        }
        private void On_Vis_Changed(ControlTreeNode node) { _linkedFace.Visible = (bool)node.Tag; }
        private void On_Tex_Changed(ControlTreeNode node) { _linkedFace.TextureName = "#"+(string)node.Tag; }
        private void On_Uv_Changed(ControlTreeNode node) { _linkedFace.UV = (Vector4)node.Tag; }
        private void On_Rot_Changed(ControlTreeNode node) {_linkedFace.Rotation = (int)node.Tag; }
        private void On_Cull_Changed(ControlTreeNode node) { _linkedFace.CullFace = (CullFace)node.Tag; }
        private void On_Tint_Changed(ControlTreeNode node) { _linkedFace.TintIndex = (int)node.Tag; }
    }
}
