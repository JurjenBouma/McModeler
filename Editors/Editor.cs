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
using System.IO;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MCModeler.Editors
{
    public abstract class Editor
    {
        public bool Enabled; 
        protected Box _editBox;

        public abstract void Dispose();
        public abstract void DrawUI();
        public abstract void SetBox(Box box);
        public abstract void Edit(Vector2 preMousePos, Vector2 curMousePos);
        public abstract void Reset();
        public abstract bool Pick(Vector2 curMousePos, out float dist);
        public abstract bool TryDrag(Vector2 curMousePos, out float dist);
        public abstract void DoMouseEffect(Vector2 curMousePos);
    }
}
