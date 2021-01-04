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
    public class Camera
    {
        Vector3 _position;
        Vector3 _target;
        Vector3 _look;
        Vector3 _up;
        Vector3 _right;

        public Vector3 Position { get { return _position; } }
        public Vector3 Look { get { return _look; } }
        public Vector3 Up {get { return _up; }  }
        public Vector3 Right { get { return _right; } }

        public Camera(Vector3 pos, Vector3 target,Vector3 up)
        {
            _position = pos;
            _target = target;
            _look = Vector3.Normalize(_target - _position);
            _right = Vector3.Normalize(Vector3.Cross(_look, up));
            _up = Vector3.Normalize(Vector3.Cross(_right,_look));
        }

        public void Pitch(float angle)
        {
            Matrix rotation = Matrix.RotationAxis(_right, angle);
            _up = Vector3.TransformCoordinate(_up, rotation);
            _position = Vector3.TransformCoordinate(_position, rotation);
            _look = Vector3.Normalize(_target - _position);
            _right = Vector3.Normalize(Vector3.Cross(_look, _up));
        }

        public void Yaw(float angle)
        {
            Matrix rotation = Matrix.RotationAxis(_up, angle);
            _position = Vector3.TransformCoordinate(_position, rotation);
            _look = Vector3.Normalize(_target - _position);
            _right = Vector3.Normalize(Vector3.Cross(_look, _up));
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.LookAtRH(_position, _target, _up);
        }

        public Ray GetRay( Vector3 direction,Matrix objectWorld)
        {
            Matrix proj = Renderer.ProjectionMatrix;
            Matrix view = Renderer.ViewMatrix;
            Matrix world = Renderer.WorldMatrix;

            Viewport viewport = Renderer.ViewPort;

            direction = Vector3.Unproject(direction, viewport.X, viewport.Y, viewport.Width, viewport.Height, viewport.MinZ, viewport.MaxZ, objectWorld * world * view * proj);

            Vector3 origin = Position;
            Matrix invWorld = Matrix.Invert(objectWorld * world);
            origin = Vector3.TransformCoordinate(origin, invWorld);

            Ray pickRay = new Ray(origin, direction);
            pickRay.Direction.Normalize();

            return pickRay;
        }
        public Ray GetPickingRay(Vector2 pixel, Matrix objectMatrix)
        {
            return GetRay(new Vector3(pixel, 1.0f), objectMatrix);
        }

        public void ZoomCamera(int amount)
        {
            _position += _look * amount;
            _look = Vector3.Normalize(_target - _position);
            _right = Vector3.Normalize(Vector3.Cross(_look, _up));
        }
    }
}
