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
    public static class Global
    {
        public static bool IsRelativePath(string path)
        {
            return !path.Contains(":\\");
        }
        public static bool IsPath(string path)
        {
            bool result = path.Contains("\\");
            return path.Contains("\\");
        }
        public static float DegreesToRedians(float angle)
        {
            return (float)Math.PI/180 * angle;
        }
        public static float FindAngle(Vector2 a,Vector2 b)
        {
            a.Normalize();
            b.Normalize();
            float dotProduct = Vector2.Dot(a,b);
            float angle = (float)Math.Acos((double)(dotProduct));
            
            return angle;
        }
        public static float FindAngle(Vector3 a, Vector3 b)
        {
            a.Normalize();
            b.Normalize();
            if (a == b)
                return 0;
            float dotProduct = Vector3.Dot(a, b);
            float angle = (float)Math.Acos((double)(dotProduct));

            return angle;
        }
        public static Vector3 FindRotationAxis(Vector3 a,Vector3 b)
        {
            Vector3 axis = Vector3.Cross(a, b);
            axis.Normalize();
            return axis;
        }
        public static Vector3 PlaceInObjectSpace(Vector3 vector, Matrix objectMatrix)
        {
            Vector3 result = vector;
            Matrix world = Renderer.WorldMatrix;
            Matrix invWorld = Matrix.Invert(objectMatrix * world);
            result = Vector3.TransformCoordinate(result, invWorld);

            return result;
        }
    }
}
