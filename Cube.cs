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
    class Cube
    {
        public ShaderResourceView texture;
        Buffer VertexBuffer;
        Buffer IndexBuffer;

        public Cube()
        {
            VertexTex[] vertices = new VertexTex[]{
                new VertexTex(new Vector3(-1f,-1f,-1f),new Vector2(0,1)),
                new VertexTex(new Vector3(-1f,1f,-1f),new Vector2(0,0)),
                new VertexTex(new Vector3(1f,1f,-1f),new Vector2(1,0)),
                new VertexTex(new Vector3(1f,-1f,-1f),new Vector2(1,1)), 
                new VertexTex(new Vector3(-1f,-1f,1f),new Vector2(0,1)),
                new VertexTex(new Vector3(-1f,1f,1f),new Vector2(0,0)),
                new VertexTex(new Vector3(1f,1f,1f),new Vector2(1,0)),
                new VertexTex(new Vector3(1f,-1f,1f),new Vector2(1,1))};

            uint[] indices = new uint[]{
                0,1,2,
                0,2,3,

                4,6,5,
                4,7,6,

                4,5,1,
                4,1,0,

                3,2,6,
                3,6,7,

                1,5,6,
                1,6,2,

                4,0,3,
                4,3,7
            };

            texture = ShaderResourceView.FromFile(Renderer.device, "beacon.png");
            SetVertexBuffer(vertices);
            SetIndexBuffer(indices);
        }
        public void SetVertexBuffer(VertexTex[] verts)
        {
            BufferDescription vBufferDesc = new BufferDescription(
                VertexTex.Stride * verts.Length,
                ResourceUsage.Immutable,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
            VertexBuffer = new Buffer(Renderer.device, new DataStream(verts, true, false), vBufferDesc);

        }

        public void SetIndexBuffer(uint[] indices)
        {
            BufferDescription iBufferDesc = new BufferDescription(
               sizeof(uint) * indices.Length,
               ResourceUsage.Immutable,
               BindFlags.IndexBuffer,
               CpuAccessFlags.None,
               ResourceOptionFlags.None,
               0);

            IndexBuffer = new Buffer(Renderer.device, new DataStream(indices, false, false), iBufferDesc);
        }
        public void Draw(ref EffectTechnique technique)
        {
            for (int p = 0; p < technique.Description.PassCount; p++)
            {
                Renderer.SetInputLayout("Textured");
                Renderer.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Renderer.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, VertexTex.Stride, 0));
                Renderer.context.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
                Renderer.SetTexture(texture);

                technique.GetPassByIndex(p).Apply(Renderer.context);
                Renderer.context.DrawIndexed(36, 0, 0);
            }
        }

        public void Dispose()
        {
            texture.Dispose();
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
        }
    }
}
