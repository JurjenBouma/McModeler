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
    public class TreeNodeModel: ControlTreeNode
    {
        Model _linkedModel;
        public TreeNodeModel(Model model):base(model.Name)
        {
            Link(model);
            Configurate();
        }
        private void Configurate()
        {
            ControlTreeNode elementsNode = new ControlTreeNode("Elements");
            ContextMenuStrip menuStripBox = new ContextMenuStrip();
            ToolStripMenuItem addBox = new ToolStripMenuItem("Add Element");
            addBox.Click += new EventHandler(On_Add_Box_Click);
            menuStripBox.Items.Add(addBox);
            elementsNode.ContextMenuStrip = menuStripBox;
            for (int i = 0; i < _linkedModel.Boxes.Count; i++)
            {
                TreeNodeElement boxNode = _linkedModel.Boxes[i].TreeNode;
                elementsNode.Nodes.Add(boxNode);
            }

            ControlTreeNode texturesNode = new ControlTreeNode("Textures");
            ContextMenuStrip menuStripTex= new ContextMenuStrip();
            ToolStripMenuItem addTex = new ToolStripMenuItem("Add Texture");
            addTex.Click += new EventHandler(On_Add_Tex_Click);
            menuStripTex.Items.Add(addTex);
            texturesNode.ContextMenuStrip = menuStripTex;
            for (int i = 0; i < _linkedModel.Textures.Count; i++)
            {
                if (_linkedModel.Textures[i].Name != "MissingT")
                {
                    texturesNode.Nodes.Add(_linkedModel.Textures[i].TreeNode);
                }
            }

            TreeNodeBool ambientNode = new TreeNodeBool("AmbientOcclusion", _linkedModel.AmbientOcclusion);
            ambientNode.ValueChanged += new TreeNodeValueChangedHandler(On_Ambient_Changed);

            elementsNode.Expand();
            texturesNode.Expand();
            Nodes.Add(elementsNode);
            Nodes.Add(texturesNode);
            Nodes.Add(ambientNode);
            Expand();
        }
        public void Link(Model model) { _linkedModel = model; }

        public void UpdateName()
        {
            this.Name = _linkedModel.Name;
            this.Text = _linkedModel.Name;
        }

        public void UpdateTextureOptions()
        {
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "Elements")
                {
                    foreach (TreeNodeElement elementNode in node.Nodes)
                    {
                        elementNode.UpdateTextureOptions();
                    }
                }

            }
        }

        public void AddTreeNodeElement(TreeNodeElement elementNode)
        {
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "Elements")
                    node.Nodes.Add(elementNode);
            }
        }

        public void RemoveTreeNodeElement(TreeNodeElement elementNode)
        {
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "Elements")
                    node.Nodes.Remove(elementNode);
            }
        }

        public void AddTreeNodeTexture(TreeNodeTexture texNode)
        {
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "Textures")
                    node.Nodes.Add(texNode);
            }
        }

        public void RemoveTreeNodeTexture(TreeNodeTexture texNode)
        {
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "Textures")
                    node.Nodes.Remove(texNode);
            }
        }

        private void On_Ambient_Changed(ControlTreeNode node) { _linkedModel.AmbientOcclusion = (bool)node.Tag; }
        private void On_Add_Box_Click(object sender, EventArgs e) 
        {
            int numTextures = _linkedModel.GetTextureNames().Length;
             string textureName = "#";
            if(numTextures > 0)
                textureName = "#" + _linkedModel.GetTextureNames()[numTextures-1];
            _linkedModel.AddBox(new Box(Vector3.Zero, new Vector3(16, 16, 16), new ElementRotation(), textureName, "New Cube", _linkedModel));
        }
        private void On_Add_Tex_Click(object sender, EventArgs e)
        {
            OpenFileDialog openTexDialog = new OpenFileDialog();
            openTexDialog.Filter = "PNG Images|*.png";
            if(openTexDialog.ShowDialog() == DialogResult.OK)
            {
                _linkedModel.AddTexture("Texture" + _linkedModel.GetTextureNames().Length.ToString(),openTexDialog.FileName);
            }
        }
    }
}
