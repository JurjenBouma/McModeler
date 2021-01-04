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
    public class TreeNodeElement: TreeNodeString
    {
        Box _linkedBox;
        public TreeNodeElement(Box box):base("",box.Name)
        {
            Link(box);
            Configurate();
        }
        private void Configurate()
        {
            this.ImageIndex = 2;
            base.ValueChanged += new TreeNodeValueChangedHandler(On_Name_Changed);
            base.SelectedChanged += new TreeNodeSelectedChangedHandler(On_Selected_Changed);

            ContextMenuStrip menuStripBox = new ContextMenuStrip();
            ToolStripMenuItem removeBox = new ToolStripMenuItem("Remove Element");
            removeBox.Click += new EventHandler(On_Remove_Box_Click);
            menuStripBox.Items.Add(removeBox);
            ContextMenuStrip = menuStripBox;

            ControlTreeNode facesNode = new ControlTreeNode("Faces");
            facesNode.SelectedChanged += new TreeNodeSelectedChangedHandler(On_Selected_Changed);
            for (int i = 0; i < _linkedBox.Faces.Count; i++)
            {
                TreeNodeFace faceNode = _linkedBox.Faces[i].TreeNode;
                facesNode.Nodes.Add(faceNode);
            }

            TreeNodeBool shadeNode = new TreeNodeBool("Shade", _linkedBox.Shade);
            shadeNode.SelectedChanged += new TreeNodeSelectedChangedHandler(On_Selected_Changed);
            shadeNode.ValueChanged += new TreeNodeValueChangedHandler(On_Shade_Changed);

            TreeNodeVector3 fromNode = new TreeNodeVector3("From", _linkedBox.From,-16,32,2);
            fromNode.SelectedChanged += new TreeNodeSelectedChangedHandler(On_Selected_Changed);
            fromNode.ValueChanged += new TreeNodeValueChangedHandler(On_From_Changed);

            TreeNodeVector3 toNode = new TreeNodeVector3("To", _linkedBox.To,-16,32,2);
            toNode.SelectedChanged += new TreeNodeSelectedChangedHandler(On_Selected_Changed);
            toNode.ValueChanged += new TreeNodeValueChangedHandler(On_To_Changed);

            TreeNodeRotation rotNode = _linkedBox.Rotation.TreeNode;
            rotNode.SelectedChanged += new TreeNodeSelectedChangedHandler(On_Selected_Changed);

            Nodes.Add(facesNode);
            Nodes.Add(fromNode);
            Nodes.Add(toNode);
            Nodes.Add(shadeNode);
            Nodes.Add(rotNode);
        }
        public void Link(Box box){ _linkedBox = box; }

        public void UpdateValues()
        {
            this.UpdateValue(_linkedBox.Name);
            foreach (ControlTreeNode node in Nodes)
            {
               if (node.Name == "Faces")
                { 
                    foreach (TreeNodeFace faceNode in node.Nodes)
                    {
                        faceNode.UpdateValues();
                    }
                }
               if (node.Name == "Shade")
               {
                   TreeNodeBool shadeNode = (TreeNodeBool)node;
                   shadeNode.UpdateValue(_linkedBox.Shade);
               }
               if (node.Name == "From")
               {
                   TreeNodeVector3 fromNode = (TreeNodeVector3)node;
                   fromNode.UpdateValue(_linkedBox.From);
               }
               if (node.Name == "To")
               {
                   TreeNodeVector3 toNode = (TreeNodeVector3)node;
                  toNode.UpdateValue(_linkedBox.To);
               }
               if (node.Name == "Rotation")
               {
                   TreeNodeRotation rotNode = (TreeNodeRotation)node;
                   rotNode.UpdateValues();
               }
            }
        }

        public void UpdateTextureOptions()
        {
            foreach (ControlTreeNode node in Nodes)
            {
                if (node.Name == "Faces")
                {
                    foreach (TreeNodeFace faceNode in node.Nodes)
                    {
                        faceNode.UpdateTextureOptions();
                    }
                }

            }
        }
        private void On_Name_Changed(ControlTreeNode node) { _linkedBox.Name = (string)node.Tag; }
        private void On_Shade_Changed(ControlTreeNode node) { _linkedBox.Shade = (bool)node.Tag; }
        private void On_From_Changed(ControlTreeNode node) { _linkedBox.From = (Vector3)node.Tag; }
        private void On_To_Changed(ControlTreeNode node) { _linkedBox.To = (Vector3)node.Tag; }
        private void On_Remove_Box_Click(object sender, EventArgs e) { _linkedBox.ParentModel.RemoveBox(_linkedBox); }
        private void On_Node_Clicked(TreeNodeMouseClickEventArgs e){if (TreeView.SelectedNode == this) _linkedBox.IsSelected = true; }
        private void On_Selected_Changed(ControlTreeNode sender) 
        {
            if (this.ContainsSelected())
                _linkedBox.ParentModel.SetSelectedBox(_linkedBox);
            else if(_linkedBox.ParentModel.GetSelectedBox() ==_linkedBox)
                _linkedBox.ParentModel.SetSelectedBox(-1);
        }
    }
}
