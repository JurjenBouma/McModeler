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
    class Keyboard
    {
        KeyboardState keyboardState;
        public Keyboard()
        {
            keyboardState = new KeyboardState();
            Device.RegisterDevice(UsagePage.Generic, UsageId.Keyboard, DeviceFlags.None);
            Device.KeyboardInput += new EventHandler<KeyboardInputEventArgs>(ReadKeyInput);
        }

        public KeyboardState GetState()
        {
            return new KeyboardState(keyboardState);
        }
        public void Reset() { keyboardState = new KeyboardState(); }

        private void ReadKeyInput(object sender, KeyboardInputEventArgs e)
        {
            keyboardState.SetKeyState(e.Key, e.State);
        }
    }
}
