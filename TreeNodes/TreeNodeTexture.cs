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
    public class TreeNodeTexture : TreeNodeString
    {
         ModelTexture _linkedTexture;
         public TreeNodeTexture(ModelTexture texture): base("",texture.Name)
        {
            Link(texture);
            Configurate();
        }
        private void Configurate()
        {
            this.ValueChanged += new TreeNodeValueChangedHandler(On_Name_Changed);

            ContextMenuStrip menuStripTex = new ContextMenuStrip();
            ToolStripMenuItem removeTex = new ToolStripMenuItem("Remove Texture");
            removeTex.Click += new EventHandler(On_Remove_Tex_Click);
            menuStripTex.Items.Add(removeTex);
            ContextMenuStrip = menuStripTex;

            if (!_linkedTexture.IsReference)
            {
                TreeNodeString filepathNode = new TreeNodeString("File", _linkedTexture.FilePath);
                filepathNode.ValueChanged += new TreeNodeValueChangedHandler(On_FilePath_Changed);

                TreeNodeString gamePathNode = new TreeNodeString("Game Path", _linkedTexture.GamePath);
                filepathNode.ValueChanged += new TreeNodeValueChangedHandler(On_GamePath_Changed);

                Nodes.Add(filepathNode);
                Nodes.Add(gamePathNode);
            }
            else
            {
                TreeNodeString refNameNode = new TreeNodeString("Reference", _linkedTexture.Reference);
                refNameNode.ValueChanged += new TreeNodeValueChangedHandler(On_Ref_Changed);
                Nodes.Add(refNameNode);
            }
        }
        public void Link(ModelTexture texture) { _linkedTexture = texture; }

        public void UpdateValues()
        {
            this.UpdateValue(_linkedTexture.Name);
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "File")
                {
                    TreeNodeString fileNode = (TreeNodeString)node;
                    fileNode.UpdateValue(_linkedTexture.FilePath);
                }
                if (node.Name == "Game Path")
                {
                    TreeNodeString gameNode = (TreeNodeString)node;
                    gameNode.UpdateValue(_linkedTexture.GamePath);
                }
            }
        }

        private void On_Name_Changed(ControlTreeNode node) { _linkedTexture.Name = (string)node.Tag; }
        private void On_FilePath_Changed(ControlTreeNode node) { _linkedTexture.FilePath = (string)node.Tag; }
        private void On_GamePath_Changed(ControlTreeNode node) { _linkedTexture.GamePath = (string)node.Tag; }
        private void On_Ref_Changed(ControlTreeNode node) { _linkedTexture.Reference = (string)node.Tag; }
        private void On_Remove_Tex_Click(object sender, EventArgs e) { _linkedTexture.ParentModel.RemoveTexture(_linkedTexture.Name); }
    }
}
