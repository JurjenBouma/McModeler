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
    public class Mover: Editor
    {
        Arrow _xArrow;
        Arrow _yArrow;
        Arrow _zArrow;
        bool _mouseHovers = false;

        public bool AllowFreeMove = false;
        public bool SnapToUnits = false;

        public Mover()
        {
            _xArrow = new Arrow(Color.Red, Vector3.Zero, new Vector3(1, 0, 0), 1, 0.1f);
            _yArrow = new Arrow(Color.Green, Vector3.Zero, new Vector3(0, 1, 0), 1, 0.1f);
            _zArrow = new Arrow(Color.Blue, Vector3.Zero, new Vector3(0, 0, 1), 1, 0.1f);
        }
        public override void SetBox(Box box)
        {
            if (box != null)
            {
                _editBox = box;
                _xArrow.Position = box.Center;
                _yArrow.Position = box.Center;
                _zArrow.Position = box.Center;

                float xLenght = _editBox.Width;
                float yLenght = _editBox.Height;
                float zLenght = _editBox.Depth;

                float arrowLenght = (float)Math.Max(xLenght, Math.Max(yLenght, zLenght));
                _xArrow.SetSize(arrowLenght, arrowLenght * 0.05f);
                _yArrow.SetSize(arrowLenght, arrowLenght * 0.05f);
                _zArrow.SetSize(arrowLenght, arrowLenght * 0.05f);

                Vector3 min = new Vector3(-16, -16, -16);
                Vector3 max = new Vector3(32, 32, 32);

                min.X += _editBox.Width * 0.5f;
                min.Y += _editBox.Height * 0.5f;
                min.Z += _editBox.Depth * 0.5f;

                max.X -= _editBox.Width * 0.5f;
                max.Y -= _editBox.Height * 0.5f;
                max.Z -= _editBox.Depth * 0.5f;

                BoundingBox bounds = new BoundingBox(min, max);

                _xArrow.Bounds = bounds;
                _yArrow.Bounds = bounds;
                _zArrow.Bounds = bounds;
            }
            else
            {
                this.Enabled = false;
            }
        }
        public override void Reset()
        {
            if (Enabled)
            {
                if (_editBox != null && _xArrow.IsDragging | _yArrow.IsDragging| _zArrow.IsDragging) 
                    _editBox.ApplyEdit();
                _xArrow.EndDrag(); _yArrow.EndDrag(); _zArrow.EndDrag(); 
                SetArrowColors();
                _editBox.ResetDrag();
            }
        }
        public override void DoMouseEffect(Vector2 curMousePos)
        {
            if (!(_xArrow.IsDragging | _yArrow.IsDragging | _zArrow.IsDragging))
            {
                float waste;
                Vector3 point;
                Axis axis;
                bool result = Pick(curMousePos, out waste, out point, out axis);

                if (result != _mouseHovers)
                {
                    SetArrowColors();
                    if (result)
                    {
                        if (axis == Axis.x)
                            _xArrow.Color = new Color4(0.8f, 0, 0);
                        else if (axis == Axis.y)
                            _yArrow.Color = new Color4(0, 0.8f, 0);
                        else if (axis == Axis.z)
                            _zArrow.Color = new Color4(0, 0, 0.8f);
                    }
                }
                _mouseHovers = result;
            }
        }
        private bool Pick(Vector2 curMousePos,out float dist,out Vector3 hitPoint,out Axis axis)
        {
            dist = float.MaxValue;
            hitPoint = Vector3.Zero;
            axis = Axis.x;
            if (Enabled)
            {
                if (!AllowFreeMove)
                {
                    float distArrow;
                    bool hitX = _xArrow.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
                    if (distArrow < dist && distArrow >= 0)
                    {
                        axis = Axis.x;
                        dist = distArrow;
                    }

                    bool hitY = _yArrow.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
                    if (distArrow < dist && distArrow >= 0)
                    {
                        axis = Axis.y;
                        dist = distArrow;
                    }

                    bool hitZ = _zArrow.Pick(curMousePos.X, curMousePos.Y, out hitPoint, out distArrow);
                    if (distArrow < dist && distArrow >= 0)
                    {
                        axis = Axis.z;
                        dist = distArrow;
                    }

                    return hitX | hitY | hitZ;
                }
                else
                {
                    bool hit = _editBox.Pick(curMousePos.X, curMousePos.Y,out hitPoint);
                    return hit;
                }
            }
            else
                return false;
        }
        public override bool Pick(Vector2 curMousePos,out float dist)
        {
            Vector3 hitPoint;
            Axis axis;
            bool result = Pick(curMousePos, out dist, out hitPoint,out axis);

            return result;
        }
        public override bool TryDrag(Vector2 curMousePos, out float dist)
        {
            Vector3 hitPoint;
            Axis axis;
            dist = -1;
            if (Enabled)
            {
                if (!AllowFreeMove)
                {
                    bool hit = Pick(curMousePos, out dist, out hitPoint,out axis);
                    if (hit)
                    {
                        if (axis == Axis.x)
                            _xArrow.StartDrag(hitPoint);
                        if (axis == Axis.y)
                            _yArrow.StartDrag(hitPoint);
                        if (axis == Axis.z)
                            _zArrow.StartDrag(hitPoint);

                        SetArrowColors();
                    }
                    return hit;
                }
                else
                {
                    bool hit = Pick(curMousePos, out dist, out hitPoint, out axis);
                    if (hit)
                        _editBox.StartDrag(hitPoint);
                    return hit;
                }
            }
            else
                return false;
        }

        void SetArrowColors()
        {
            if (_xArrow.IsDragging || _yArrow.IsDragging || _zArrow.IsDragging)
            {
                if (_xArrow.IsDragging)
                    _xArrow.Color = new Color4(1,0,0);
                else
                    _xArrow.Color = new Color4(0.25f, 1, 0, 0);

                if (_yArrow.IsDragging)
                    _yArrow.Color = new Color4(0,1,0);
                else
                    _yArrow.Color = new Color4(0.25f, 0, 1, 0);

                if (_zArrow.IsDragging)
                    _zArrow.Color = new Color4(0,0,1);
                else
                    _zArrow.Color = new Color4(0.25f, 0, 0, 1);
            }
            else
            {
                _xArrow.Color = new Color4(0.5f,0,0);
                _yArrow.Color = new Color4(0, 0.5f, 0);
                _zArrow.Color = new Color4(0, 0, 0.5f);
            }
        }
        void UpdateArrowsPositions()
        {
            if (SnapToUnits)
            {
                if (!_xArrow.IsDragging)
                    _xArrow.Position = _editBox.Center;
                if (!_yArrow.IsDragging)
                    _yArrow.Position = _editBox.Center;
                if (!_zArrow.IsDragging)
                    _zArrow.Position = _editBox.Center;
            }
            else
            {
                _xArrow.Position = _editBox.Center;
                _yArrow.Position = _editBox.Center;
                _zArrow.Position = _editBox.Center;
            }
        }

        public override void Edit(Vector2 preMousePos, Vector2 curMousePos)
        {
            if (Enabled)
            {
                if (!AllowFreeMove)
                    Drag(preMousePos,curMousePos);
                else
                    _editBox.Drag(curMousePos);
            }
        }
        private void Drag(Vector2 preMousePos, Vector2 curMousePos)
        {
            if (_xArrow.IsDragging || _yArrow.IsDragging || _zArrow.IsDragging)
            {
                _xArrow.Drag(preMousePos,curMousePos);
                _yArrow.Drag(preMousePos,curMousePos);
                _zArrow.Drag(preMousePos,curMousePos);
                MoveBoxByArrow();
                UpdateArrowsPositions();
            }
        }
        private void MoveBoxByArrow()
        {
            Vector3 newCenter = _editBox.Center;
            Vector3 moveAmount = Vector3.Zero;

            if (_xArrow.IsDragging)
            {
                newCenter = _xArrow.Position;
                moveAmount = newCenter - _editBox.Center;
                if (SnapToUnits)
                {
                    if (moveAmount.X > 0)
                    {
                        if (newCenter.X < (int)(_editBox.Center.X + 1))
                            moveAmount = Vector3.Zero;
                        else
                        {
                            moveAmount = new Vector3(1, 0, 0);
                        }
                    }
                    else if (moveAmount.X < 0)
                    {
                        if (newCenter.X > (int)(_editBox.Center.X - 1))
                            moveAmount = Vector3.Zero;
                        else
                        {
                            moveAmount = new Vector3(-1, 0, 0);
                        }
                    }
                }
            }
            if (_yArrow.IsDragging)
            {
                newCenter = _yArrow.Position;
                moveAmount = newCenter - _editBox.Center;
                if (SnapToUnits)
                {
                    if (moveAmount.Y > 0)
                    {
                        if (newCenter.Y < (int)(_editBox.Center.Y + 1))
                            moveAmount = Vector3.Zero;
                        else
                        {
                            moveAmount = new Vector3(0, 1, 0);
                        }
                    }
                    else if (moveAmount.Y < 0)
                    {
                        if (newCenter.Y > (int)(_editBox.Center.Y - 1))
                            moveAmount = Vector3.Zero;
                        else
                        {
                            moveAmount = new Vector3(0, -1, 0);
                        }
                    }
                }
            }
            if (_zArrow.IsDragging)
            {
                newCenter = _zArrow.Position;
                moveAmount = newCenter - _editBox.Center;
                if (SnapToUnits)
                {
                    if (moveAmount.Z > 0)
                    {
                        if (newCenter.Z < (int)(_editBox.Center.Z + 1))
                            moveAmount = Vector3.Zero;
                        else
                        {
                            moveAmount = new Vector3(0, 0, 1);
                        }
                    }
                    else if (moveAmount.Z < 0)
                    {
                        if (newCenter.Z > (int)(_editBox.Center.Z - 1))
                            moveAmount = Vector3.Zero;
                        else
                        {
                            moveAmount = new Vector3(0, 0, -1);
                        }
                    }
                }
            }
            _editBox.Move(moveAmount);
        }

        public override void DrawUI()
        {
            if(Enabled && !AllowFreeMove)
            {
                _xArrow.Draw();
                _yArrow.Draw();
                _zArrow.Draw();
            }
        }

        public override void Dispose()
        {
            if (_xArrow != null)
                _xArrow.Dispose();
            if (_yArrow != null)
                _yArrow.Dispose();
            if (_zArrow != null)
                _zArrow.Dispose();
        }
    }
}
