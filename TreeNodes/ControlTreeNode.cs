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
    public delegate void TreeNodeValueChangedHandler(ControlTreeNode sender);
    public delegate void TreeNodeClickedHandler(TreeNodeMouseClickEventArgs e);
    public delegate void TreeNodeSelectedChangedHandler(ControlTreeNode sender);

    public class ControlTreeNode : TreeNode
    {
        public Control Control;
        public bool RescaleControl = true;
        bool prevSelectedState = false;
        public TreeNodeValueChangedHandler ValueChanged;
        public TreeNodeClickedHandler NodeClicked;
        public TreeNodeSelectedChangedHandler SelectedChanged;

        public ControlTreeNode() { }
        public ControlTreeNode(string name)
        {
            Name = name;
            Text = name;
        }

        public void CollapseControls()
        {
            if (TreeView != null && Control != null)
                if (TreeView.Controls.Contains(Control))
                    TreeView.Controls.Remove(Control);
            foreach (ControlTreeNode node in Nodes)
                node.CollapseControls();
        }

        public bool ContainsSelected()
        {
            if (IsSelected)
                return true;
            else
            {
                foreach (ControlTreeNode node in Nodes)
                {
                    if (node.ContainsSelected())
                        return true;
                }
            }
            return false;
        }
        public void MakeSelected()
        {
            if(TreeView != null)
            { TreeView.SelectedNode = this;}
        }

        public void OnNodeClicked(TreeNodeMouseClickEventArgs e)
        {
            if (NodeClicked != null)
                NodeClicked(e);
        }
        public void UpdateSelected()
        {
            if (ContainsSelected() != prevSelectedState)
            {
                prevSelectedState = ContainsSelected();
                if (SelectedChanged != null)
                    SelectedChanged(this);
            }
            foreach (ControlTreeNode node in Nodes)
                node.UpdateSelected();
        }
    }
}
