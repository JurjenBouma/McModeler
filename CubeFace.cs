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
    public delegate void FacePropertyChangedHandler(CubeFace face);
    public class CubeFace
    {
        bool _visible;
        string _textureName;
        public CullFace CullFace;
        public int TintIndex;
        VertexTex[] _vertices;
        uint[] _indices;
        Buffer _vertexBuffer;
        Buffer _indexBuffer;
        FaceOrientation _orientation;
        int _rotation;
        Vector4 _uV;    
        public FacePropertyChangedHandler PropertyChanged;
        TreeNodeFace _treeNode;
        Box _parentBox;
        Matrix _transformation;

        public Box ParentBox {get { return _parentBox; }}
        public FaceOrientation Orientation { get { return _orientation; } }
        public Vector4 UV { get { return _uV; } set { _uV = value; OnChange(); UpdateUv(); } }
        public int Rotation { get { return _rotation; } set { _rotation = value; UpdateUv(); OnChange(); } }
        public TreeNodeFace TreeNode { get { return _treeNode; } set {_treeNode = value; _treeNode.Link(this); } }
        public string TextureName { get {return _textureName;}set{_textureName = value;}}
        public bool Visible { get { return _visible; } set { _visible = value; OnChange(); } }

        public CubeFace(Vector3 from, Vector3 to, FaceOrientation orientation, Vector4 uV, string textureName, bool visible, Box parentBox)
        {
            _orientation = orientation;
            _textureName = textureName;
            _visible = visible;
            CullFace = CullFace.none;
            _rotation = 0;
            TintIndex = -1;
            _uV = uV;
            _parentBox = parentBox;
            _treeNode = new TreeNodeFace(this);

            SetVertices(from, to);
            SetIndices();

            SetVertexBuffer(_vertices);
            SetIndexBuffer(_indices);
        }
        public CubeFace(JsonFace face, Vector3 from, Vector3 to, Box parentBox)
        {
            _orientation = face.Orientation;
            _textureName = face.TextureName;
            _visible = face.Visible;
            CullFace = face.CullFace;
            _rotation = face.Rotation;
            TintIndex = face.TintIndex;
            _uV = face.UV;
            _parentBox = parentBox;
            _treeNode = new TreeNodeFace(this);

            SetVertices(from, to);
            SetIndices();

            SetVertexBuffer(_vertices);
            SetIndexBuffer(_indices);
        }

        public JsonFace GetJsonFace()
        {
            JsonFace face = new JsonFace();
            face.CullFace = CullFace;
            face.Orientation = _orientation;
            face.Rotation = _rotation;
            face.TextureName = TextureName;
            face.TintIndex = TintIndex;
            face.UV = _uV;
            face.Visible = _visible;
            return face;
        }

        public void Resize(Vector3 from, Vector3 to)
        {
            SetTransform(from, to);
        }

        private void SetIndices()
        {
            _indices = new uint[]{
                0,1,2,
                0,2,3
            };
        }

        private void SetVertices(Vector3 from, Vector3 to)
        {
            _vertices = new VertexTex[]{
                new VertexTex(new Vector3(1 ,0 ,0),GetTexCoord(_uV,0,_rotation)),
                new VertexTex(new Vector3(0 ,0 ,0),GetTexCoord(_uV,1,_rotation)),
                new VertexTex(new Vector3(0 ,1 ,0),GetTexCoord(_uV,2,_rotation)),
                new VertexTex(new Vector3(1 ,1 ,0),GetTexCoord(_uV,3,_rotation))
            };
            SetTransform(from,to);
        }

        private void UpdateUv()
        {
            for(int i=0;i< _vertices.Length; i++)
            {
                _vertices[i].UV = GetTexCoord(_uV, i, _rotation);
            }
            _vertexBuffer.Dispose();
            SetVertexBuffer(_vertices);
        }

        private void SetVertexBuffer(VertexTex[] verts)
        {
            BufferDescription vBufferDesc = new BufferDescription(
                VertexTex.Stride * verts.Length,
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

        private Vector2 GetTexCoord(Vector4 uV, int vert, int rotation)
        {
            Vector2[] coords = new Vector2[]{
                new Vector2(uV.X/16,uV.W/16),
                new Vector2(uV.Z/16,uV.W/16),
                new Vector2(uV.Z/16,uV.Y/16),
                new Vector2(uV.X/16,uV.Y/16)
            };
            int offset = rotation / 90;
            int index = (vert + (coords.Length - offset)) % coords.Length;
            return coords[index];
        }

        private void SetTransform(Vector3 from, Vector3 to)
        {
            Matrix rotation = GetRotationMatrix();
            Matrix scaling = GetScalingMatrix(from, to);
            Matrix translation = GetTranslationMatrix(from, to);
            _transformation = rotation * scaling * translation;
        }

        private Matrix GetTranslationMatrix(Vector3 from, Vector3 to)
        {
            Matrix translation = Matrix.Identity;
            float translationX=0;
            float translationY=0;
            float translationZ=0;
            switch (_orientation)
            {
                case FaceOrientation.south:
                    translationX = to.X;
                    translationY = from.Y;
                    translationZ = to.Z;
                    break;
                case FaceOrientation.north:
                    translationX = from.X;
                    translationY = from.Y;
                    translationZ = from.Z;
                    break;
                case FaceOrientation.up:
                    translationX = from.X;
                    translationY = to.Y;
                    translationZ = from.Z;
                    break;
                case FaceOrientation.down:
                    translationX = from.X;
                    translationY = from.Y;
                    translationZ = to.Z;
                    break;
                case FaceOrientation.east:
                    translationX = to.X;
                    translationY = from.Y;
                    translationZ = from.Z;
                    break;
                case FaceOrientation.west:
                    translationX = from.X;
                    translationY = from.Y;
                    translationZ = to.Z;
                    break;
            }
            translation = Matrix.Translation(translationX, translationY, translationZ);
            return translation;
        }
        private Matrix GetScalingMatrix(Vector3 from,Vector3 to)
        {
            float scalingX = to.X - from.X;
            float scalingY = to.Y - from.Y;
            float scalingZ = to.Z - from.Z;

            return Matrix.Scaling(scalingX, scalingY, scalingZ);
        }
        private Matrix GetRotationMatrix()
        {
            Matrix rotation = Matrix.Identity;
            switch (_orientation)
            {
                case FaceOrientation.north:
                    rotation = Matrix.Identity;
                    break;
                case FaceOrientation.south:
                    rotation = Matrix.RotationY((float)Math.PI);
                    break;
                case FaceOrientation.up:
                    rotation = Matrix.RotationX((float)Math.PI * 0.5f);
                    break;
                case FaceOrientation.down:
                    rotation = Matrix.RotationX(-(float)Math.PI * 0.5f);
                    break;
                case FaceOrientation.east:
                    rotation = Matrix.RotationY(-(float)Math.PI * 0.5f);
                    break;
                case FaceOrientation.west:
                    rotation = Matrix.RotationY((float)Math.PI * 0.5f);
                    break;
            }
            return rotation;
        }

        public void Draw(EffectTechnique technique,int pass,Matrix objectTransform)
        {
            if (_visible)
            {
                Renderer.SetWVP(_transformation * objectTransform);
                Renderer.SetInputLayout("Textured");
                Renderer.context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                Renderer.context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, VertexTex.Stride, 0));
                Renderer.context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
                _parentBox.ParentModel.SetActiveTexture(TextureName);

                technique.GetPassByIndex(pass).Apply(Renderer.context);
                Renderer.context.DrawIndexed(36, 0, 0);
            }
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }

        private void OnChange()
        {
            if (PropertyChanged != null)
                PropertyChanged(this);
        }
    }
}
