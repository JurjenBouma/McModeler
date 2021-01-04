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
using MCModeler.Editors;
using System.IO;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MCModeler
{
    public partial class MainForm : Form
    {
        Mouse mouse;
        MouseState preMouseState;
        MouseState curMouseState;
        Vector2 curPanelMousePos;
        Vector2 prePanelMousePos;

        Keyboard keyboard;
        KeyboardState preKBState;
        KeyboardState curKBState;
        bool clickedObject = false;

        Mover mover;
        Shaper shaper;

        public MainForm()
        {
            InitializeComponent();
            mouse = new Mouse();

            keyboard = new Keyboard();
            ResetInputDevices();

            Renderer.Update += new Renderer.UpdateLoop(ReadUserInput);
        }

        public void Initialize3D()
        {
            mover = new Mover();
            mover.AllowFreeMove = false;

            shaper = new Shaper();

            Renderer.ActiveEditor = shaper;

        }

        void ReadUserInput()
        {
            if (panelRender.Focused)
            {
                UpdateCurrentInputStates();
                if (curPanelMousePos.X > 0 && curPanelMousePos.X < panelRender.Width && curPanelMousePos.Y > 0 && curPanelMousePos.Y < panelRender.Height)
                {
                    if (curMouseState.LeftButtonDown && !preMouseState.LeftButtonDown)
                    {
                        clickedObject = Pick3D();
                    }
                    if (!curMouseState.LeftButtonDown && preMouseState.LeftButtonDown)
                    {
                        clickedObject = false;
                        Renderer.ActiveEditor.Reset();
                    }
                    if (curMouseState.LeftButtonDown)
                    {
                        if (!clickedObject)
                            RotateCamera();
                        else
                            Renderer.ActiveEditor.Edit(prePanelMousePos, curPanelMousePos);
                    }
                    ZoomCamera();
                    Renderer.ActiveEditor.DoMouseEffect(curPanelMousePos);
                }
                else
                {
                    Renderer.ActiveEditor.Reset();
                    clickedObject = false;
                }

                UpdatePreviousInputStates();
            }
        }

        private bool Pick3D()
        {
            float boxDist = -1;
            bool hitBox = false;
            int boxIndex = -1;
            if (Renderer.GetActiveModel() != null)
            {
                Vector3 waste;
                boxIndex = Renderer.GetActiveModel().Pick(curPanelMousePos.X, curPanelMousePos.Y, out waste, out boxDist);
                if (boxIndex >= 0)
                    hitBox = true;
            }

            float editorDist = -1;
            bool hitMover = Renderer.ActiveEditor.TryDrag(curPanelMousePos, out editorDist);

            if (boxDist < 0)
                boxDist = float.MaxValue;

            if (hitBox && !hitMover)
                Renderer.GetActiveModel().SetSelectedBox(boxIndex);

            return hitMover | hitBox;
        }

        private void UpdateCurrentInputStates()
        {
                curMouseState = mouse.GetState();
                curKBState = keyboard.GetState();
                Point mousePos = Cursor.Position;
                mousePos = panelRender.PointToClient(mousePos);
                curPanelMousePos = new Vector2(mousePos.X, mousePos.Y);
        }

        private void UpdatePreviousInputStates()
        {
            preMouseState = curMouseState;
            preKBState = new KeyboardState(curKBState);
            prePanelMousePos = curPanelMousePos;
        }

        private void ZoomCamera()
        {
            int wheelDelta = curMouseState.WheelValue -preMouseState.WheelValue;
            int wheelRotations = wheelDelta / 120;
            Renderer.camera.ZoomCamera(wheelRotations);
        }

        private void RotateCamera()
        {
            float changeX = curMouseState.MousePosition.X - preMouseState.MousePosition.X;
            float changeY = curMouseState.MousePosition.Y - preMouseState.MousePosition.Y;

            Renderer.camera.Yaw(-changeX * 0.005f);
            Renderer.camera.Pitch(-changeY * 0.005f);
        }

        private void ResetInputDevices()
        {
            mouse.Reset();
            Point mousePos = Cursor.Position;
            mousePos = panelRender.PointToClient(mousePos);
            curPanelMousePos = new Vector2(mousePos.X, mousePos.Y);
            prePanelMousePos = new Vector2(mousePos.X, mousePos.Y);

            curMouseState = mouse.GetState();
            preMouseState = mouse.GetState();

            keyboard.Reset();
            curKBState = keyboard.GetState();
            preKBState = keyboard.GetState();
        }

        private void Form1_SizeChanged(object sender, EventArgs e) { Renderer.Resize(panelRender.Width, panelRender.Height); }

        private void openToolStripMenuItem_Click(object sender, EventArgs e){ openFileDialogModel.ShowDialog(); }
        private void openFileDialogModel_FileOk(object sender, CancelEventArgs e)
        {
            LoadModel(openFileDialogModel.FileName);
        }
        private void LoadModel(string fileName)
        {
            Model model = new Model(new JsonStructure(fileName));
            Renderer.SetActiveModel(model);
            treeViewModel.Reset();
            treeViewModel.Nodes.Add(model.TreeNode);
            model.SelectedBoxChanged += new SelectedBoxChangedHandler(On_Selected_Box_Changed);
            Renderer.ActiveEditor.Enabled = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            saveFileDialogModel.ShowDialog(); 
        }
        private void saveFileDialogModel_FileOk(object sender, CancelEventArgs e)
        {
            Renderer.GetActiveModel().FilePath = saveFileDialogModel.FileName;
            FileInfo info = new FileInfo(saveFileDialogModel.FileName);
            Renderer.GetActiveModel().Name = info.Name;
            JsonStructure json = new JsonStructure(Renderer.GetActiveModel());
            json.FilePath = saveFileDialogModel.FileName;
            json.Save();
        }
        private void panelRender_MouseEnter(object sender, EventArgs e) { panelRender.Focus();}
        private void panelRender_MouseLeave(object sender, EventArgs e) { menuStrip1.Focus(); }
        private void panelRender_Enter(object sender, EventArgs e) { ResetInputDevices();}
        private void panelRender_Leave(object sender, EventArgs e) {ResetInputDevices();}
        private void Form1_Move(object sender, EventArgs e) { ResetInputDevices(); }
        private void MainForm_Deactivate(object sender, EventArgs e){ResetInputDevices(); }
       
        private void panelRender_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop,false);
            if(Global.IsPath(files[0]))
                LoadModel(files[0]);
        }

        private void panelRender_DragEnter(object sender, DragEventArgs e) {e.Effect = DragDropEffects.All; }
        private void panelLeft_MouseEnter(object sender, EventArgs e) {panelRight.Focus();  }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Model model = new Model("Model");
            Renderer.SetActiveModel(model);
            treeViewModel.Reset();
            treeViewModel.Nodes.Add(model.TreeNode);
            model.SelectedBoxChanged += new SelectedBoxChangedHandler(On_Selected_Box_Changed);
            Renderer.ActiveEditor.Enabled = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mover.Dispose();
            shaper.Dispose();
        }

        private void On_Selected_Box_Changed(Model sender, Box selectedBox)
        {
            Renderer.ActiveEditor.Enabled = true;
            Renderer.ActiveEditor.SetBox(selectedBox);
        }

        private void translateAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Renderer.GetActiveModel() != null)
            {
                Renderer.ActiveEditor.Reset();
                mover.Enabled = true;
                mover.AllowFreeMove = false;
                mover.SetBox(Renderer.GetActiveModel().GetSelectedBox());
                Renderer.ActiveEditor = mover;
            }
        }

        private void shapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Renderer.GetActiveModel() != null)
            {
                Renderer.ActiveEditor.Reset();
                shaper.Enabled = true;
                shaper.SetBox(Renderer.GetActiveModel().GetSelectedBox());
                Renderer.ActiveEditor = shaper;
            }
        }

    }
}
