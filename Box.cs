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
    public delegate void BoxLooksChangedEventHandler(Box sender);
    public class Box
    {
        ElementRotation _rotation;
        bool _shade;
        bool _isSelected;
        string _name;
        Vector3 _from;
        Vector3 _to;
        List<CubeFace> _faces;
        TreeNodeElement _treeNode;
        Model _parentModel;
        Matrix _tempMoveMatrix = Matrix.Identity;

        Vector3 _dragPoint;
        Vector2 _lastHitMousePos;
        bool _isDragging;

        public ElementRotation Rotation { get { return _rotation; } set { _rotation = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public bool Shade { get { return _shade; } set { _shade = value; } }
        public Model ParentModel{get { return _parentModel; } }
        public List<CubeFace> Faces {get { return _faces;} }
        public Vector3 From { get { return Vector3.TransformCoordinate(_from, _tempMoveMatrix); } set { _from = Round(SetToBounds(value)); UpdateFaces(); OnLooksChanged(); } }
        public Vector3 To { get { return Vector3.TransformCoordinate(_to, _tempMoveMatrix); ; } set { _to = Round(SetToBounds(value)); UpdateFaces(); OnLooksChanged(); } }
        public TreeNodeElement TreeNode { get { return _treeNode; } }
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; if(_isSelected && !_treeNode.ContainsSelected())_treeNode.MakeSelected();} }
        public Vector3 Center { get { return new Vector3((From.X + To.X) / 2, (From.Y + To.Y) / 2, (From.Z + To.Z) / 2); } }
        public float Width { get { return (float)Math.Abs(_to.X - _from.X); } }
        public float Height { get { return  (float)Math.Abs(_to.Y - _from.Y); } }
        public float Depth { get { return (float)Math.Abs(_to.Z - _from.Z); } }

        public BoxLooksChangedEventHandler LooksChanged;

        public Box(Vector3 from,Vector3 to,ElementRotation rotation,string textureName,string name,Model parentModel)
        {
            _from = from;
            _to = to;
            _shade = true;
            _rotation = rotation;
            _rotation.Changed += new OnRotationChangedHandler(OnLooksChanged);
            _name = name;
            _parentModel = parentModel;

            _faces = new List<CubeFace>
            {
                new CubeFace(from,to,FaceOrientation.up,new Vector4(0,0,16,16),textureName,true,this),
                 new CubeFace(from,to,FaceOrientation.down,new Vector4(0,0,16,16),textureName,true,this),
                 new CubeFace(from,to,FaceOrientation.north,new Vector4(0,0,16,16),textureName,true,this),
                 new CubeFace(from,to,FaceOrientation.south,new Vector4(0,0,16,16),textureName,true,this),
                 new CubeFace(from,to,FaceOrientation.east,new Vector4(0,0,16,16),textureName,true,this),
                 new CubeFace(from,to,FaceOrientation.west,new Vector4(0,0,16,16),textureName,true,this)
            };
            HookFaceEvents();
            _treeNode = new TreeNodeElement(this);
            IsSelected = _treeNode.IsSelected;
            _treeNode.UpdateValues();
        }
        public Box(JsonElement element,Model parentModel)
        {
            _from = element.From;
            _to = element.To;
            _shade = element.Shade;
            _rotation = element.Rotation;
            _rotation.Changed += new OnRotationChangedHandler(OnLooksChanged);
            _name = element.Name;
            _parentModel = parentModel;

            _faces = new List<CubeFace>();
            for (int i = 0; i < element.Faces.Count; i++)
            {
                _faces.Add(new CubeFace(element.Faces[i], _from, _to, this));
            }
            AddMissingFaces();
            HookFaceEvents();
            _treeNode = new TreeNodeElement(this);
            IsSelected = _treeNode.IsSelected;
            _treeNode.UpdateValues();
        }

        private void HookFaceEvents()
        {
            foreach (CubeFace face in _faces)
            {
                face.PropertyChanged += new FacePropertyChangedHandler(UpdateFace);
            }
        }

        private void AddMissingFaces()
        {
            foreach (FaceOrientation orientation in Enum.GetValues(typeof(FaceOrientation)))
            {
                if(!DoesContainFace(orientation))
                {
                    CubeFace face = new CubeFace(_from, _to, orientation, new Vector4(0, 0, 16, 16), "#", false, this);
                    _faces.Add(face);
                }
            }
        }
        private bool DoesContainFace(FaceOrientation orientation)
        {
            bool result = false;
            foreach(CubeFace face in _faces)
            {
                if (face.Orientation == orientation)
                    result = true;
            }
            return result;
        }

        public CubeFace GetFace(FaceOrientation orientation)
        {
            CubeFace result = null;
            foreach (CubeFace face in _faces)
            {
                if (face.Orientation == orientation)
                    result = face;
            }
            return result;
        }

        public JsonElement GetElement()
        {
            JsonElement element = new JsonElement();
            element.From = _from;
            element.To = _to;
            element.Shade = _shade;
            element.Rotation = new ElementRotation(Rotation);
            element.Name = _name;
            List<JsonFace> faces = new List<JsonFace>();
            foreach(CubeFace cF in _faces)
            {
                JsonFace face = cF.GetJsonFace();
                if(face != null)
                    faces.Add(face);
            }
            element.Faces = faces;
            return element;
        }

        private Matrix GetRescaleMatrix(Matrix rotation)
        {
            Matrix rescale = Matrix.Identity;
            if (this.Rotation.Rescale)
            {
                Vector3 rotFrom = Vector3.TransformCoordinate(_from,rotation);
                Vector3 rotTo = Vector3.TransformCoordinate(_to,rotation);

                float originalWidth = _to.X - _from.X;
                float rotWidth = rotTo.X - rotFrom.X;
                float scaleX = 1;
                if(rotWidth != 0 && originalWidth !=0)
                    scaleX = originalWidth / rotWidth;

                float originalHeight = _to.Y - _from.Y;
                float rotHeight = rotTo.Y - rotFrom.Y;
                float scaleY = 1;
                if(rotHeight != 0 && originalHeight !=0)
                    scaleY = originalHeight / rotHeight;

                float originalDepth = _to.Z - _from.Z;
                float rotDepth = rotTo.Z - rotFrom.Z;
                float scaleZ = 1;
                if (rotDepth != 0 && originalDepth != 0)
                    scaleZ = originalDepth / rotDepth;

                Matrix translateToOrigin = Matrix.Translation(-(this.Rotation.Origin));
                Matrix scaleMatrix = Matrix.Scaling(scaleX, scaleY, scaleZ);
                Matrix translateBack = Matrix.Translation((this.Rotation.Origin));
                rescale = translateToOrigin * scaleMatrix * translateBack;
            }
            return rescale;
        }
        public void ChangeTextureRef(string oldName, string newName)
        {
            foreach (CubeFace face in _faces)
            {
                if (face.TextureName == oldName)
                    face.TextureName = newName;
            }
        }
        public Matrix ObjectWorld
        {
            get
            {
                Matrix rotationMatrix = Rotation.GetOriginRotationMatrix();
                Matrix rescaleMatrix = GetRescaleMatrix(rotationMatrix);
                Matrix objectTransForm = rotationMatrix * rescaleMatrix;
                return objectTransForm;
            }
        }

        public void Move(Vector3 amount)
        {
            if (IsMoveInBounds(amount))
            {
                _tempMoveMatrix *= Matrix.Translation(amount);
                Rotation.Origin += amount;
            }
        }

        private void OnLooksChanged()
        {
            if(LooksChanged != null)
            {
                LooksChanged(this);
            }
        }

        public void ApplyEdit()
        {
            From = Vector3.TransformCoordinate(_from, _tempMoveMatrix);
            To = Vector3.TransformCoordinate(_to, _tempMoveMatrix);
            _tempMoveMatrix = Matrix.Identity;
            this._treeNode.UpdateValues();
        }

        private bool IsMoveInBounds(Vector3 amount)
        {
            Vector3 newFromUnbound = Vector3.TransformCoordinate(_from, _tempMoveMatrix) + amount;
            Vector3 newToUnbound = Vector3.TransformCoordinate(_to, _tempMoveMatrix) + amount;
            Vector3 newFromBound = SetToBounds(newFromUnbound);
            Vector3 newToBound = SetToBounds(newToUnbound);
            if (newFromBound != newFromUnbound || newToBound != newToUnbound)
                return false;
            else
                return true;
        }
        private Vector3 Round(Vector3 vector)
        {
            vector = new Vector3((float)Math.Round(vector.X, 2, MidpointRounding.AwayFromZero), (float)Math.Round(vector.Y, 2, MidpointRounding.AwayFromZero), (float)Math.Round(vector.Z, 2, MidpointRounding.AwayFromZero));
            return vector;
        }
        private Vector3 SetToBounds(Vector3 vector)
        {
            if (vector.X < -16)
                vector.X = -16;
            if (vector.Y < -16)
                vector.Y = -16;
            if (vector.Z < -16)
                vector.Z = -16;
            if (vector.X > 32)
                vector.X = 32;
            if (vector.Y > 32)
                vector.Y = 32;
            if (vector.Z > 32)
                vector.Z = 32;

            return vector;
        }
        public BoundingBox BoundingBox
        {
            get
            {
                BoundingBox bBox = new BoundingBox(Vector3.TransformCoordinate(_from, _tempMoveMatrix), Vector3.TransformCoordinate(_to, _tempMoveMatrix));
                return bBox;
            }
        }

        public void StartDrag(Vector3 dragPoint) { _isDragging = true; _dragPoint = dragPoint; }
        public void ResetDrag() { _isDragging = false; }

        public void Drag(Vector2 curMousePos)
        {
            if (_isDragging)
            {
                //X move
                Ray rayc = Renderer.camera.GetPickingRay(new Vector2(_lastHitMousePos.X, _lastHitMousePos.Y), Matrix.Identity);
                Ray raya = Renderer.camera.GetPickingRay(new Vector2(curMousePos.X, _lastHitMousePos.Y), Matrix.Identity);
                Vector3 right = Renderer.camera.Right;

                float A = (float)(Math.Abs(Global.FindAngle(-rayc.Direction, right)));
                float B = (float)(Math.Abs(Global.FindAngle(rayc.Direction, raya.Direction)));
                float C = (float)(Math.PI) - A - B;

                Vector3 disVector = rayc.Position - _dragPoint;
                float dist = disVector.Length();
                float c = dist;
                float b = (float)((c * Math.Sin(B)) / Math.Sin(C));

                int dir = 1;
                if (curMousePos.X < _lastHitMousePos.X)
                    dir = -1;
                Vector3 moveAmountX = right * b * dir;

                //Y move
                raya = Renderer.camera.GetPickingRay(new Vector2(_lastHitMousePos.X, curMousePos.Y), Matrix.Identity);
                Vector3 up = Renderer.camera.Up;

                A = (float)(Math.Abs(Global.FindAngle(-rayc.Direction, up)));
                B = (float)(Math.Abs(Global.FindAngle(rayc.Direction, raya.Direction)));
                C = (float)(Math.PI) - A - B;

                b = (float)((c * Math.Sin(B)) / Math.Sin(C));

                dir = 1;
                if (curMousePos.Y > _lastHitMousePos.Y)
                    dir = -1;
                Vector3 moveAmountY = up * b * dir;

                Vector3 moveAmount = moveAmountX + moveAmountY;
                if (curMousePos != _lastHitMousePos)
                {
                    Vector3 oldCenter = Center;
                    Move(moveAmount);
                    Vector3 movedAmount = Center - oldCenter;
                    _dragPoint = Vector3.TransformCoordinate(_dragPoint, Matrix.Translation(movedAmount));
                    _lastHitMousePos = curMousePos;
                }
            }
        }

        public bool Pick(float x, float y, out Vector3 hitPoint, out float dist)
        {
            bool hit = false;
            hitPoint = Vector3.Zero;

            Ray pickRay = Renderer.camera.GetPickingRay(new Vector2(x, y), ObjectWorld);

            float distance;
            if (Ray.Intersects(pickRay, BoundingBox, out distance))
            {
                if (distance > 0)
                {
                    hit = true;

                    hitPoint = pickRay.Position + (pickRay.Direction * distance);
                    Matrix invWorld = Matrix.Invert(ObjectWorld);
                    hitPoint = Vector3.TransformCoordinate(hitPoint, invWorld);
                    _lastHitMousePos = new Vector2(x, y);
                }
            }
            if (hit)
                dist = distance;
            else
                dist = -1;

            return hit;
        }
        public bool Pick(float x, float y, out Vector3 hitPoint)
        {
            float waste;
            return Pick(x, y, out hitPoint, out waste); ;
        }

        public bool Pick(float x, float y)
        {
            Vector3 waste;
            float wastefloat;
            return Pick(x, y, out waste, out  wastefloat); ;
        }
        public void Draw(EffectTechnique technique, int pass)
        {
            Matrix transform =  _tempMoveMatrix * ObjectWorld;
            foreach (CubeFace face in _faces)
            {
                face.Draw(technique, pass, transform);
            }

        }
        public void Dispose()
        {
            foreach (CubeFace face in _faces)
            {
                face.Dispose();
            }
        }

        private void UpdateFace(CubeFace face)
        {
            for(int i=0;i< _faces.Count;i++)
            {
                if (_faces[i] == face)
                    face.Resize(_from, _to);
            }
        }
        private void UpdateFaces()
        {
            for (int i = 0; i < _faces.Count; i++)
            {
                UpdateFace(_faces[i]);
            }
        }
    }
}
