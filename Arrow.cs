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
    public class Arrow
    {
        public bool Visible;
        Vector3 _direction;
        VertexCol[] _vertices;
        uint[] _indices;
        Buffer _vertexBuffer;
        Buffer _indexBuffer;
        Color4 _color;
        Matrix _translation;
        Matrix _rotation;
        Matrix _scale;
        Vector3 defaultDir = new Vector3(1,0,0);
        Vector3 _dragPoint;
        bool _isDragging;

        public BoundingBox Bounds;
        int _indicesLenght;

        public Vector3 Direction { get { return _direction; } set { _direction = value; _direction.Normalize(); _rotation = GetRotation(_direction); } }
        public Color4 Color { get { return _color; } set { SetNewColor(value); } }
        public Vector3 Position { get { return Vector3.TransformCoordinate(Vector3.Zero, _translation); } set {_translation = GetTranslation(value); } }
        public bool IsDragging { get { return _isDragging; } }

        public Arrow()
        {
            _direction = new Vector3(1, 0, 0);
            _direction.Normalize();
            _translation = Matrix.Identity;
            _rotation = Matrix.Identity;
            _scale = GetScaling(9, 1);
            _color = new Color4(1,0,0);
            Visible = true;
            _vertices = GetVerts();
            _indices = GetIndices();
            SetVertexBuffer(_vertices);
            SetIndexBuffer(_indices);
        }
        public Arrow(Color4 color,Vector3 position,Vector3 direction,float lenght,float diameter)
        {
            _direction = direction;
            _direction.Normalize();
            _color = color;
            Visible = true;
            _vertices = GetVerts();
            _indices = GetIndices();
            SetVertexBuffer(_vertices);
            SetIndexBuffer(_indices);
            _translation = GetTranslation(position);
            _rotation = GetRotation(_direction);
            _scale = GetScaling(lenght, diameter);
        }
        private void SetNewColor(Color4 color)
        {
            Dispose();
            _color = color;
            for(int i = 0; i< _vertices.Length;i++)
            {
                _vertices[i].Color = _color;
            }
            SetVertexBuffer(_vertices);
            SetIndexBuffer(_indices);
        }
        public void SetSize(float lenght,float diameter)
        {
             _scale = GetScaling(lenght, diameter);
        }
        private Matrix GetScaling(float lenght,float diameter)
        {
            Matrix scaling = Matrix.Scaling(new Vector3(lenght, diameter, diameter));
            return scaling;
        }
        private Matrix GetRotation(Vector3 direction)
        {
            float angle = -Global.FindAngle(defaultDir, direction);
            Vector3 axis = Global.FindRotationAxis(direction, defaultDir);
            return Matrix.RotationAxis(axis, angle);
        }
        private Matrix GetTranslation(Vector3 position)
        {
            position = SetToBounds(position);
            Matrix trans = Matrix.Translation(position);
            return trans;
        }
        private VertexCol[] GetVerts()
        {
            VertexCol[] verts = new VertexCol[]
            {
                new VertexCol(new Vector3(0.8f,-0.5f,-0.5f),_color),//0
                new VertexCol(new Vector3(0,-0.5f,-0.5f),_color),
                new VertexCol(new Vector3(0,0.5f,-0.5f),_color),
                new VertexCol(new Vector3(0.8f,0.5f,-0.5f),_color),

                new VertexCol(new Vector3(0.8f,-0.5f,0.5f),_color),//4
                new VertexCol(new Vector3(0,-0.5f,0.5f),_color),
                new VertexCol(new Vector3(0,0.5f,0.5f),_color),
                new VertexCol(new Vector3(0.8f,0.5f,0.5f),_color),

                new VertexCol(new Vector3(0.8f,-1,-1),_color),//8
                new VertexCol(new Vector3(0.8f,-1, 1),_color),
                new VertexCol(new Vector3(0.8f,1,1),_color),
                new VertexCol(new Vector3(0.8f,1,-1),_color),

                new VertexCol(new Vector3(1,0,0),_color),
            };
            return verts;
        }
        private uint[] GetIndices()
        {
            uint[] indices = new uint[] 
            { 
                0,1,2,
                0,2,3,

                1,5,6,
                2,1,6,

                4,7,6,
                6,5,4,

                6,7,3,
                6,3,2,

                1,0,4,
                1,4,5,

                8,9,10,
                8,10,11,

                12,8,11,
                12,9,8,
                12,10,9,//tip
                12,11,10
            };
            _indicesLenght = indices.Length;
            return indices;
        }

        private Vector3 SetToBounds(Vector3 vector)
        {
            if (Bounds.Minimum != Vector3.Zero || Bounds.Maximum != Vector3.Zero)
            {
                if (vector.X < Bounds.Minimum.X)
                    vector.X = Bounds.Minimum.X;
                if (vector.Y < Bounds.Minimum.Y)
                    vector.Y = Bounds.Minimum.Y;
                if (vector.Z < Bounds.Minimum.Z)
                    vector.Z = Bounds.Minimum.Z;
                if (vector.X > Bounds.Maximum.X)
                    vector.X = Bounds.Maximum.X;
                if (vector.Y > Bounds.Maximum.Y)
                    vector.Y = Bounds.Maximum.Y;
                if (vector.Z > Bounds.Maximum.Z)
                    vector.Z = Bounds.Maximum.Z;
            }
            return vector;
        }

        private BoundingBox boundingBox
        {
            get
            {
                Vector3 from = new Vector3(1, -0.5f, -0.5f);
                Vector3 to = new Vector3(0, 0.5f, 0.5f);

                Vector3 scaledFrom = Vector3.TransformCoordinate(from, _scale);
                Vector3 scaledTo = Vector3.TransformCoordinate(to, _scale);

                BoundingBox boundBox = new BoundingBox(scaledFrom, scaledTo);
                return boundBox;
            }
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

        public bool Pick(float x, float y, out Vector3 hitPoint)
        {
            float result;
            return Pick(x, y, out hitPoint, out result); ;
        }

        public bool Pick(float x, float y)
        {
            Vector3 waste;
            float result;
            return Pick(x, y, out waste, out  result); ;
        }

        public bool Pick(float x, float y, out Vector3 hitPoint, out float dist)
        {
            bool hit = false;
            hitPoint = Vector3.Zero;
            Ray pickRay = Renderer.camera.GetPickingRay(new Vector2(x, y), _rotation * _translation);

            float distance = float.MaxValue;
            if (Visible)
            {
                if (Ray.Intersects(pickRay, boundingBox, out distance))
                {
                    if (distance > 0)
                    {
                        hit = true;
                        hitPoint = pickRay.Position + (pickRay.Direction * distance);
                    }
                }
            }
            if (hit)
                dist = distance;
            else
                dist = -1;
            return hit;
        }

        public void StartDrag(Vector3 dragPoint) { _isDragging = true; _dragPoint = dragPoint; }
        public void EndDrag() { _isDragging = false; }

        public void Drag(Vector2 preMousePos, Vector2 curMousePos)
        {
            if(_isDragging)
                MoveArrow(preMousePos, curMousePos, _dragPoint, out _dragPoint);
        }

        public void MoveArrow(Vector2 preMousePos, Vector2 curMousePos, Vector3 oldDragPoint, out Vector3 newDragPoint)
        {
            newDragPoint = oldDragPoint;
            if (Visible)
            {
                //X move
                Ray rayc = Renderer.camera.GetPickingRay(new Vector2(preMousePos.X, preMousePos.Y), Matrix.Identity);
                Ray raya = Renderer.camera.GetPickingRay(new Vector2(curMousePos.X, preMousePos.Y), Matrix.Identity);
                Vector3 right = Renderer.camera.Right;

                float A = (float)(Math.Abs(Global.FindAngle(-rayc.Direction, right)));
                float B = (float)(Math.Abs(Global.FindAngle(rayc.Direction, raya.Direction)));
                float C = (float)(Math.PI) - A - B;

                Vector3 disVector = rayc.Position - oldDragPoint;
                float dist = disVector.Length();
                float c = dist;
                float b = (float)((c * Math.Sin(B)) / Math.Sin(C));

                int dir = 1;
                if (curMousePos.X < preMousePos.X)
                    dir = -1;
                Vector3 moveAmountX = right * b * dir;

                //Y move
                raya = Renderer.camera.GetPickingRay(new Vector2(preMousePos.X, curMousePos.Y), Matrix.Identity);
                Vector3 up = Renderer.camera.Up;

                A = (float)(Math.Abs(Global.FindAngle(-rayc.Direction, up)));
                B = (float)(Math.Abs(Global.FindAngle(rayc.Direction, raya.Direction)));
                C = (float)(Math.PI) - A - B;

                b = (float)((c * Math.Sin(B)) / Math.Sin(C));

                dir = 1;
                if (curMousePos.Y > preMousePos.Y)
                    dir = -1;
                Vector3 moveAmountY = up * b * dir;

                float amount = Vector3.Dot(moveAmountX, _direction) + Vector3.Dot(moveAmountY, _direction);
                Vector3 moveVector = _direction * amount;
                if (curMousePos != preMousePos)
                {
                    Position +=moveVector;
                    newDragPoint = Vector3.TransformCoordinate(oldDragPoint, Matrix.Translation(moveVector));
                }

            }
        }
        public void Draw()
        {
            if (Visible)
            {
                Renderer.SetInputLayout("Color");
                Renderer.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Renderer.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, VertexCol.Stride, 0));
                Renderer.context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
                Renderer.SetWVP(_scale* _rotation * _translation);
                for (int p = 0; p < Renderer.ColoredTechnique.Description.PassCount; p++)
                {
                    Renderer.ColoredTechnique.GetPassByIndex(p).Apply(Renderer.context);
                    Renderer.context.DrawIndexed(_indicesLenght, 0, 0);
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
