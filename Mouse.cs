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
using SlimDX.Multimedia;
using SlimDX.RawInput;
using Device = SlimDX.RawInput.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;
namespace MCModeler
{
    public struct MouseState
    {
        public bool LeftButtonDown;
        public bool MiddleButtonDown;
        public bool RightButtonDown;
        public bool Button4Down;
        public bool Button5Down;
        public Vector2 MousePosition;
        public int WheelValue;

        public MouseState(bool left,bool middle,bool right , bool button4 , bool button5,Vector2 pos,int wheel)
        {
            LeftButtonDown = left;
            MiddleButtonDown = middle;
            RightButtonDown = right;
            Button4Down = button4;
            Button5Down = button5;
            MousePosition = pos;
            WheelValue = wheel;
        }
    }

    public delegate void MouseEventH(MouseState mouseState);
    class Mouse
    {
        MouseState _mouseState;
        public MouseEventH OnLeftDown;
        public MouseEventH OnLeftUp;
        public MouseEventH OnRightDown;
        public MouseEventH OnRightUp;
        public MouseEventH OnMoved;

        public Mouse()
        {
            _mouseState = new MouseState();
            Device.RegisterDevice(UsagePage.Generic, UsageId.Mouse, DeviceFlags.None);
            Device.MouseInput += new EventHandler<MouseInputEventArgs>(ReadMouseInput);
        }

        public MouseState GetState(){return _mouseState; }
        public void SetMouseStatePosition(int x, int y) { _mouseState.MousePosition = new Vector2(x, y); }

        public void Reset() { _mouseState = new MouseState(false,false,false,false,false,Vector2.Zero,0); }

        void ReadMouseInput(object sender, MouseInputEventArgs e)
        {
            Vector2 pos = new Vector2(e.X, e.Y);

            if (pos.X != 0 || pos.Y != 0)
                if (OnMoved != null)
                    OnMoved(_mouseState);
            _mouseState.MousePosition += pos;

            _mouseState.WheelValue += e.WheelDelta;

            if (e.ButtonFlags == MouseButtonFlags.LeftDown)
            {
                _mouseState.LeftButtonDown = true;
                if (OnLeftDown != null)
                    OnLeftDown(_mouseState);
            }
            if (e.ButtonFlags == MouseButtonFlags.LeftUp)
            {
                _mouseState.LeftButtonDown = false;
                if (OnLeftUp != null)
                    OnLeftUp(_mouseState);
            }
            if (e.ButtonFlags == MouseButtonFlags.MiddleDown)
                _mouseState.MiddleButtonDown = true;
            if (e.ButtonFlags == MouseButtonFlags.MiddleUp)
                _mouseState.MiddleButtonDown = false;

            if (e.ButtonFlags == MouseButtonFlags.RightDown)
            {
                _mouseState.RightButtonDown = true;
                if (OnRightDown != null)
                    OnRightDown(_mouseState);
            }
            if (e.ButtonFlags == MouseButtonFlags.RightUp)
            {
                _mouseState.RightButtonDown = false;
                if (OnRightUp != null)
                    OnRightUp(_mouseState);
            }
            if (e.ButtonFlags == MouseButtonFlags.Button4Down)
                _mouseState.Button4Down = true;
            if (e.ButtonFlags == MouseButtonFlags.Button4Up)
                _mouseState.Button4Down = false;

            if (e.ButtonFlags == MouseButtonFlags.Button5Down)
                _mouseState.Button5Down = true;
            if (e.ButtonFlags == MouseButtonFlags.Button5Up)
                _mouseState.Button5Down = false;
        }
    }
}
