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
using System.IO;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;
namespace MCModeler.Editors
{
    class Arrows 
    {
        public Arrow Up;
        public Arrow Down;
        public Arrow North;
        public Arrow South;
        public Arrow East;
        public Arrow West;
        public bool IsDragging { get { return Up.IsDragging | Down.IsDragging | North.IsDragging | South.IsDragging | East.IsDragging | West.IsDragging; } }

        public Arrows()
        {
            Up = new Arrow(Color.White, Vector3.Zero, new Vector3(0, 1, 0), 4, 0.5f);
            Down = new Arrow(Color.White, Vector3.Zero, new Vector3(0, -1, 0), 4, 0.5f);
            North = new Arrow(Color.White, Vector3.Zero, new Vector3(0, 0, -1), 4, 0.5f);
            South = new Arrow(Color.White, Vector3.Zero, new Vector3(0, 0, 1), 4, 0.5f);
            East = new Arrow(Color.White, Vector3.Zero, new Vector3(1, 0, 0), 4, 0.5f);
            West = new Arrow(Color.White, Vector3.Zero, new Vector3(-1, 0, 0), 4, 0.5f);
        }
        public bool Pick(Vector2 curMousePos, out float dist , out Vector3 hitPoint , out FaceOrientation orientation)
        {
            dist = float.MaxValue;
            float distArrow;
            orientation = FaceOrientation.up;

            bool hitUp = Up.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
            if (distArrow < dist && distArrow >= 0)
            {
                orientation = FaceOrientation.up;
                dist = distArrow;
            }

            bool hitDown = Down.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
            if (distArrow < dist && distArrow >= 0)
            {
                orientation = FaceOrientation.down;
                dist = distArrow;
            }

            bool hitNorth = North.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
            if (distArrow < dist && distArrow >= 0)
            {
                orientation = FaceOrientation.north;
                dist = distArrow;
            }

            bool hitSouth = South.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
            if (distArrow < dist && distArrow >= 0)
            {
                orientation = FaceOrientation.south;
               dist = distArrow;
            }
            bool hitEast = East.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
            if (distArrow < dist && distArrow >= 0)
            {
                orientation = FaceOrientation.east;
                dist = distArrow;
            }
             bool hitWest = West.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
             if (distArrow < dist && distArrow >= 0)
             {
                 orientation = FaceOrientation.west;
                 dist = distArrow;
             }

           return hitUp | hitDown | hitNorth | hitSouth | hitEast | hitWest;
        }
        public void StartDrag(Vector3 dragPoint,FaceOrientation orientation)
        {
            if (orientation == FaceOrientation.up)
                Up.StartDrag(dragPoint);
            if (orientation == FaceOrientation.down)
                Down.StartDrag(dragPoint);
            if (orientation == FaceOrientation.north)
                North.StartDrag(dragPoint);
            if (orientation == FaceOrientation.south)
                South.StartDrag(dragPoint);
            if (orientation == FaceOrientation.east)
                East.StartDrag(dragPoint);
            if (orientation == FaceOrientation.west)
                West.StartDrag(dragPoint);
        }
        public void Drag(Vector2 preMousePos, Vector2 curMousePos)
        {
            Up.Drag(preMousePos, curMousePos);
            Down.Drag(preMousePos, curMousePos);
            North.Drag(preMousePos, curMousePos);
            South.Drag(preMousePos, curMousePos);
            East.Drag(preMousePos, curMousePos);
            West.Drag(preMousePos, curMousePos);
        }
        public void ResetDrag()
        {
            Up.EndDrag();
            Down.EndDrag();
            North.EndDrag();
            South.EndDrag();
            East.EndDrag();
            West.EndDrag();
        }
        public void SetColors(Color4 color)
        {
            Up.Color = color;
            Down.Color = color;
            North.Color = color;
            South.Color = color;
            East.Color = color;
            West.Color = color;
        }
        public void SetColor(Color4 color , FaceOrientation orientation)
        {
            if (orientation == FaceOrientation.up)
                Up.Color = color;
            if (orientation == FaceOrientation.down)
                Down.Color = color;
            if (orientation == FaceOrientation.north)
                North.Color = color;
            if (orientation == FaceOrientation.south)
                South.Color = color;
            if (orientation == FaceOrientation.east)
                East.Color = color;
            if (orientation == FaceOrientation.west)
                West.Color = color;
        }
        public void SetLenghts(float lenght)
        {
            Up.SetSize(lenght, lenght * 0.05f);
            Down.SetSize(lenght, lenght * 0.05f);
            North.SetSize(lenght, lenght * 0.05f);
            South.SetSize(lenght, lenght * 0.05f);
            East.SetSize(lenght, lenght * 0.05f);
            West.SetSize(lenght, lenght * 0.05f);
        }
       
        public void Draw()
        {
            Up.Draw();
            Down.Draw();
            North.Draw();
            South.Draw();
            East.Draw();
            West.Draw();
        }
        public void Dispose()
        {
            if (Up != null)
                Up.Dispose();
            if (Down != null)
                Down.Dispose();
            if (North != null)
                North.Dispose();
            if (South != null)
                South.Dispose();
            if (East != null)
                East.Dispose();
            if (West != null)
                West.Dispose();
        }
    }
    public class Shaper : Editor
    {
        Arrows _arrows;
        bool _mouseHovers;

        public Shaper()
        {
            _arrows = new Arrows();
            _mouseHovers = false;
        }

        public override void SetBox(Box box)
        {
            if(_editBox != null)
                _editBox.LooksChanged -= new BoxLooksChangedEventHandler(Box_Looks_Changed);
            if (box != null)
            {
                _editBox = box;
                _editBox.LooksChanged += new BoxLooksChangedEventHandler(Box_Looks_Changed);
                SetArrows();
            }
            else
                this.Enabled = false;
        }
        private void Box_Looks_Changed(Box sender)
        {
            SetArrows();
        }
        private void SetArrows()
        {
            SetArrowColors();
            SetArrowVisibilety();
            SetArrowDirections();
            SetArrowPositions();
        }
        private void SetArrowVisibilety()
        {
            _arrows.Up.Visible = _editBox.GetFace(FaceOrientation.up).Visible;
            _arrows.Down.Visible = _editBox.GetFace(FaceOrientation.down).Visible;
            _arrows.North.Visible = _editBox.GetFace(FaceOrientation.north).Visible;
            _arrows.South.Visible = _editBox.GetFace(FaceOrientation.south).Visible;
            _arrows.East.Visible = _editBox.GetFace(FaceOrientation.east).Visible;
            _arrows.West.Visible = _editBox.GetFace(FaceOrientation.west).Visible;
        }
        private void SetArrowPositions()
        {
            Vector3[] positions = GetArrowPositions();
            _arrows.Up.Position = positions[0];
            _arrows.Down.Position = positions[1];
            _arrows.North.Position = positions[2];
            _arrows.South.Position = positions[3];
            _arrows.East.Position = positions[4];
            _arrows.West.Position = positions[5];
        }
        
        Vector3[] GetArrowPositions()
        {
            Vector3[] positions = new Vector3[]
            {
            Vector3.TransformCoordinate(new Vector3(_editBox.From.X + _editBox.Width * 0.5f,
                _editBox.To.Y, _editBox.From.Z + _editBox.Depth * 0.5f),_editBox.ObjectWorld),

            Vector3.TransformCoordinate(new Vector3(_editBox.From.X + _editBox.Width * 0.5f,
                _editBox.From.Y, _editBox.From.Z + _editBox.Depth * 0.5f),_editBox.ObjectWorld),

           Vector3.TransformCoordinate(new Vector3(_editBox.From.X + _editBox.Width * 0.5f,
                _editBox.From.Y + _editBox.Height *0.5f, _editBox.From.Z),_editBox.ObjectWorld),

            Vector3.TransformCoordinate(new Vector3(_editBox.From.X + _editBox.Width * 0.5f,
                _editBox.From.Y + _editBox.Height * 0.5f, _editBox.To.Z),_editBox.ObjectWorld),

            Vector3.TransformCoordinate(new Vector3(_editBox.To.X, _editBox.From.Y + _editBox.Height * 0.5f,
                _editBox.From.Z + _editBox.Depth * 0.5f),_editBox.ObjectWorld),

            Vector3.TransformCoordinate(new Vector3(_editBox.From.X, _editBox.From.Y + _editBox.Height * 0.5f,
                _editBox.From.Z + _editBox.Depth * 0.5f), _editBox.ObjectWorld),
            };
            return positions;
        }
        private void SetArrowDirections()
        {
            _arrows.Up.Direction = Vector3.TransformCoordinate(new Vector3(0, 1, 0),_editBox.Rotation.GetRotationMatrix());
            _arrows.Down.Direction = Vector3.TransformCoordinate(new Vector3(0, -1, 0), _editBox.Rotation.GetRotationMatrix());
            _arrows.North.Direction = Vector3.TransformCoordinate(new Vector3(0, 0, -1), _editBox.Rotation.GetRotationMatrix());
            _arrows.South.Direction = Vector3.TransformCoordinate(new Vector3(0, 0, 1), _editBox.Rotation.GetRotationMatrix());
            _arrows.East.Direction = Vector3.TransformCoordinate(new Vector3(1, 0, 0), _editBox.Rotation.GetRotationMatrix());
            _arrows.West.Direction = Vector3.TransformCoordinate(new Vector3(-1, 0, 0), _editBox.Rotation.GetRotationMatrix());
        }

        public override void Reset()
        {
            if (_editBox != null && _arrows.IsDragging)
                _editBox.ApplyEdit();
            _arrows.ResetDrag();
            SetArrowColors();
        }
        public override void DoMouseEffect(Vector2 curMousePos)
        {
            if (!_arrows.IsDragging)
            {
                float waste;
                Vector3 point;
                FaceOrientation orientation;
                bool result = Pick(curMousePos, out waste, out point, out orientation);
                if (result != _mouseHovers)
                {
                    SetArrowColors();
                    if (result)
                        _arrows.SetColor(new Color4(0.9f, 1, 1, 1), orientation);
                }
                _mouseHovers = result;
            }
        }
        private bool Pick(Vector2 curMousePos, out float dist,out Vector3 hitPoint,out FaceOrientation orientation)
        {
            hitPoint = Vector3.Zero;
            orientation = FaceOrientation.up;
            dist = float.MaxValue;
            if (Enabled)
            {
                return _arrows.Pick(curMousePos, out dist, out hitPoint, out orientation);
            }
            else
                return false;
        }
        public override bool Pick(Vector2 curMousePos, out float dist)
        {
            Vector3 hitPoint;
            FaceOrientation orientation;
            bool result = Pick(curMousePos, out dist, out hitPoint, out orientation);
            return result;
        }
        public override bool TryDrag(Vector2 curMousePos, out float dist)
        {
            Vector3 hitPoint;
            FaceOrientation orientation;
            bool hit = Pick(curMousePos, out dist, out hitPoint, out orientation);
            if (hit)
            {
                _arrows.StartDrag(hitPoint, orientation);
                SetArrowColors();
            }
            return hit;
        }
        public override void Edit(Vector2 preMousePos, Vector2 curMousePos)
        {
            if (Enabled)
            {
                _arrows.Drag(preMousePos, curMousePos);
                EditBox();
                SetArrowPositions();
            }
        }

        void EditBox()
        {
            Vector3 from = _editBox.From;
            Vector3 to = _editBox.To;

            Vector3[] originalPositions = GetArrowPositions();

            Vector3 moveUp = _arrows.Up.Position - originalPositions[0];
            moveUp = Vector3.TransformCoordinate(moveUp, Matrix.Invert(_editBox.Rotation.GetRotationMatrix()));
            to += moveUp;

            Vector3 moveDown = _arrows.Down.Position - originalPositions[1];
            moveDown = Vector3.TransformCoordinate(moveDown, Matrix.Invert(_editBox.Rotation.GetRotationMatrix()));
            from += moveDown;

            Vector3 moveNorth = _arrows.North.Position - originalPositions[2];
            moveNorth = Vector3.TransformCoordinate(moveNorth, Matrix.Invert(_editBox.Rotation.GetRotationMatrix()));
            from += moveNorth;

            Vector3 moveSouth = _arrows.South.Position - originalPositions[3];
            moveSouth = Vector3.TransformCoordinate(moveSouth, Matrix.Invert(_editBox.Rotation.GetRotationMatrix()));
            to += moveSouth;

            Vector3 moveEast = _arrows.East.Position - originalPositions[4];
            moveEast = Vector3.TransformCoordinate(moveEast, Matrix.Invert(_editBox.Rotation.GetRotationMatrix()));
            to += moveEast;

            Vector3 moveWest = _arrows.West.Position - originalPositions[5];
            moveWest = Vector3.TransformCoordinate(moveWest, Matrix.Invert(_editBox.Rotation.GetRotationMatrix()));
            from += moveWest;

            _editBox.To = to;
            _editBox.From = from;
        }

        void SetArrowColors()
        {
            if (_arrows.IsDragging)
            {
                _arrows.SetColors(new Color4(0.1f, 1, 1, 1));
                if (_arrows.Up.IsDragging)
                    _arrows.Up.Color = Color.White;
                if (_arrows.Down.IsDragging)
                    _arrows.Down.Color = Color.White;
                if (_arrows.North.IsDragging)
                    _arrows.North.Color = Color.White;
                if (_arrows.South.IsDragging)
                    _arrows.South.Color = Color.White;
                if (_arrows.East.IsDragging)
                    _arrows.East.Color = Color.White;
                if (_arrows.West.IsDragging)
                    _arrows.West.Color = Color.White;
            }
            else
                _arrows.SetColors(new Color4(0.5f, 1, 1, 1));
        }

        public override void DrawUI()
        {
            if (Enabled)
            {
                _arrows.Draw();
            }
        }

        public override void Dispose()
        {
            _arrows.Dispose();
        }
    }
}
