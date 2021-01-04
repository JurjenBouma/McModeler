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


namespace MCModeler
{
    public class CustomTreeView : TreeView
    {
        int mouseX;
        ControlTreeNode prevSelectedNode;

        public CustomTreeView()
        {
            this.DrawMode = TreeViewDrawMode.Normal;
            this.MouseCaptureChanged += new EventHandler(RePositionControls);
            this.NodeMouseClick += new TreeNodeMouseClickEventHandler(Node_Click);
            this.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(Node_Double_Click);
            this.BeforeExpand += new TreeViewCancelEventHandler(Before_Expand);
            this.BeforeCollapse += new TreeViewCancelEventHandler(Before_Collapse);
            this.AfterCollapse += new TreeViewEventHandler(After_Collapse);
            this.MouseMove += new MouseEventHandler(Track_MouseX);
            this.AfterSelect += new TreeViewEventHandler(Afther_Node_Select);
        }
        private void Node_Click(object sender, TreeNodeMouseClickEventArgs e)
        {
            ControlTreeNode node = (ControlTreeNode)e.Node;
            Controls.Clear();
            node.OnNodeClicked(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                this.ContextMenuStrip = e.Node.ContextMenuStrip;
        }
        private void Node_Double_Click(object sender, TreeNodeMouseClickEventArgs e)
        {
            ControlTreeNode node = (ControlTreeNode)e.Node;
            Controls.Clear();
            if (mouseX > e.Node.Bounds.X)
            {
                if (node.Control != null)
                {
                    SetNodeControlBounds(node);

                    Controls.Add(node.Control);
                    node.Control.Focus();
                }
            }
        }
        private void SetNodeControlBounds(ControlTreeNode node)
        {
            if (node != null)
            {
                if (node.Control != null)
                {
                    if (node.RescaleControl)
                        node.Control.Size = new System.Drawing.Size(node.Bounds.Width + 6, node.Control.Height);
                    node.Control.Location = new Point(node.Bounds.X-1,node.Bounds.Y-2);
                    int controlRight = node.Control.Location.X + node.Control.Size.Width;
                    if (controlRight > this.Width - 20)
                    {
                        int moveLeft = controlRight - (this.Width - 21);
                        node.Control.Location = new Point( node.Control.Location.X - moveLeft, node.Control.Location.Y);
                    }
                }
            }
        }
        private void Afther_Node_Select(object sender, TreeViewEventArgs e)
        {
            foreach (ControlTreeNode node in Nodes)
                node.UpdateSelected();
        }
        private void RePositionControls(object sender, EventArgs e) { SetNodeControlBounds((ControlTreeNode)this.SelectedNode); }
        private void Track_MouseX(object sender, MouseEventArgs e) { mouseX = e.X; }
        private void Before_Collapse(object sender, TreeViewCancelEventArgs e) {if (mouseX > e.Node.Bounds.X) e.Cancel = true; }
        private void Before_Expand(object sender, TreeViewCancelEventArgs e) {if (mouseX > e.Node.Bounds.X)e.Cancel = true;}
        
        private void After_Collapse(object sender, TreeViewEventArgs e)
        {
            ControlTreeNode node = (ControlTreeNode)e.Node;
            node.CollapseControls();
        }
        public void Reset()
        {
            foreach(ControlTreeNode node in Nodes)
            {
                node.CollapseControls();
            }
            Nodes.Clear();
        }
    }
}
