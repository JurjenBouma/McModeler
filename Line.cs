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

namespace MCModeler
{
    class Line
    {
        public bool Visible;
        Buffer _vertexBuffer;
        Buffer _indexBuffer;

        public Line(Vector3 from,Vector3 to,Color4 color)
        {
            Visible = true;
            SetVertexBuffer(new VertexCol[] { new VertexCol(from, color), new VertexCol(to, color) });
            SetIndexBuffer(new uint[] { 0, 1 });
        }

        private void SetVertexBuffer(VertexCol[] verts)
        {
            BufferDescription vBufferDesc = new BufferDescription(
                VertexCol.Stride * verts.Length,
                ResourceUsage.Immutable,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
            _vertexBuffer = new Buffer(Renderer.device, new DataStream(verts, true, false), vBufferDesc);

        }

        private void SetIndexBuffer(uint[] indices)
        {
            BufferDescription iBufferDesc = new BufferDescription(
               sizeof(uint) * indices.Length,
               ResourceUsage.Immutable,
               BindFlags.IndexBuffer,
               CpuAccessFlags.None,
               ResourceOptionFlags.None,
               0);

            _indexBuffer = new Buffer(Renderer.device, new DataStream(indices, false, false), iBufferDesc);
        }

        public void Draw()
        {
            if (Visible)
            {
                for (int p = 0; p < Renderer.ColoredTechnique.Description.PassCount; p++)
                {
                    Renderer.SetInputLayout("Color");
                    Renderer.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
                    Renderer.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, VertexCol.Stride, 0));
                    Renderer.context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);

                    Renderer.ColoredTechnique.GetPassByIndex(p).Apply(Renderer.context);
                    Renderer.context.DrawIndexed(36, 0, 0);
                }
            }
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}
