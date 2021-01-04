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
    class KeyboardState
    {
        List<Keys> _pressedKeys;
        List<Keys> _releasedKeys;
        
        public KeyboardState()
        {
            _pressedKeys = new List<Keys>();
            _releasedKeys = new List<Keys>();
            _releasedKeys.AddRange(Enum.GetValues(typeof(Keys)).Cast<Keys>());
        }

        public KeyboardState(KeyboardState state)
        {
            _pressedKeys = new List<Keys>(state._pressedKeys);
            _releasedKeys = new List<Keys>(state._releasedKeys);
        }

        public void SetKeyState(Keys key,KeyState state)
        {
            if (state == KeyState.Pressed || state == KeyState.SystemKeyPressed)
            {
                _releasedKeys.Remove(key);
                if(!_pressedKeys.Contains(key))
                    _pressedKeys.Add(key);
            }
            else if (state == KeyState.Released || state == KeyState.SystemKeyReleased)
            {
                if (!_releasedKeys.Contains(key))
                _releasedKeys.Add(key);
                _pressedKeys.Remove(key);
            }
        }
        public bool IsKeyPressed(Keys key)
        {
            if (_pressedKeys.Contains(key))
                return true;
            return false;
        }
        public bool IsKeyReleased(Keys key)
        {
            if (_releasedKeys.Contains(key))
                return true;
            return false;
        }
    }
}
