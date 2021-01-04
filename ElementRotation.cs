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
    public delegate void OnRotationChangedHandler();
    public class ElementRotation
    {
        Axis _axis;
        float _angle;
        Vector3 _origin;
        bool _rescale;
        TreeNodeRotation _treeNode;

        public Axis Axis { get { return _axis; } set { _axis = value; OnChanged(); } }
        public float Angle { get { return _angle; } set { _angle = value; OnChanged(); } }
        public Vector3 Origin { get { return _origin; } set { _origin = value; OnChanged(); } }
        public bool Rescale { get { return _rescale; } set { _rescale = value; OnChanged(); } }
        public TreeNodeRotation TreeNode { get { return _treeNode; } set { _treeNode = value; _treeNode.Link(this); } }

        public OnRotationChangedHandler Changed;

        public ElementRotation()
        { _axis = Axis.x; _angle = 0; _origin = new Vector3(8, 8, 8); _rescale = false; _treeNode = new TreeNodeRotation(this); }
        public ElementRotation(Vector3 origin, Axis axis, float angle, bool rescale)
        {
            _axis = axis;
            _angle = angle;
            _origin = origin;
            _rescale = rescale;
            _treeNode = new TreeNodeRotation(this);
        }
        public ElementRotation(ElementRotation rotation)
        {
            _axis = rotation._axis;
            _angle = rotation._angle;
            _origin = new Vector3(rotation._origin.X, rotation._origin.Y, rotation._origin.Z);
            _rescale = rotation._rescale;
            _treeNode = new TreeNodeRotation(this);
        }
        private Matrix GetOrginRotationMatrix(Vector3 origin, Vector3 axis, float angle)
        {
            Matrix translateToOrigin = Matrix.Translation(-(origin));
            Matrix rotate = Matrix.RotationAxis(axis, angle);
            Matrix translateBack = Matrix.Translation(origin);

            return translateToOrigin * rotate * translateBack;
        }
        public Matrix GetRotationMatrix()
        {
            Vector3 rotAxis = new Vector3(0, 0, 0);
            if (_axis == Axis.x)
                rotAxis.X = 1;
            if (_axis == Axis.y)
                rotAxis.Y = 1;
            if (_axis == Axis.z)
                rotAxis.Z = 1;
            Matrix rotate = Matrix.RotationAxis(rotAxis, Global.DegreesToRedians(_angle));

            return rotate;
        }
        public Matrix GetOriginRotationMatrix()
        {
            Vector3 rotAxis = new Vector3(0, 0, 0);
            if (_axis == Axis.x)
                rotAxis.X = 1;
            if (_axis == Axis.y)
                rotAxis.Y = 1;
            if (_axis == Axis.z)
                rotAxis.Z = 1;

            return GetOrginRotationMatrix(_origin, rotAxis, Global.DegreesToRedians(_angle));
        }

        private void OnChanged()
        {
            if (Changed != null)
                Changed();
        }
    }
}
