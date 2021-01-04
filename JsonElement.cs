using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.Direct2D;
using SlimDX.Windows;
using SlimDX.DXGI;
using SlimDX.D3DCompiler;

namespace MCModeler
{
    public enum Axis { x,y,z}

    public enum FaceOrientation { down,up,north,south,east,west}
    public enum CullFace { none,down, up, north, south, east, west }
    public class JsonFace
    {
        public FaceOrientation Orientation;
        public Vector4 UV = new Vector4(0,0,16,16);
        public string TextureName = "";
        public CullFace CullFace = CullFace.none;
        public int Rotation = 0;
        public int TintIndex =-1;
        public bool Visible = true;
    }

    public class JsonElement
    {
        public Vector3 From;
        public Vector3 To;
        public ElementRotation Rotation;
        public bool Shade;
        public string Name;
        public List<JsonFace> Faces;

        public JsonElement()
        {
            From = Vector3.Zero;
            To = Vector3.Zero;
            Shade = true;
            Faces = new List<JsonFace>();
            Rotation = new ElementRotation();
            Name = "Element";
        }
    }
}
