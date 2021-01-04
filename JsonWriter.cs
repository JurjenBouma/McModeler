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
using System.Globalization;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MCModeler
{
    class JsonWriter
    {
        string _filePath;
        JsonStructure _jsonStructure;
        string _fileText;
        public JsonWriter(JsonStructure jsonStruct)
        {
            _jsonStructure = jsonStruct;
            _filePath = jsonStruct.FilePath;
            Write();
        }

        public void SaveJson()
        {
            Stream stream = new FileStream(_filePath, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(_fileText);
            writer.Flush();
            stream.Close();
        }

        private void Write()
        {
            _fileText = "";
            _fileText += "{\n       ";
            AddBool("ambientocclusion", _jsonStructure.AmbientOcclusion);
            _fileText += ",\n";
            AddTexturesText();
            AddElementsText();
            _fileText += "}";
        }
        private void AddTexturesText()
        {
            if (_jsonStructure.Textures.Count > 0)
            {
                _fileText += "      \"textures\": {\n";
                for (int i = 0; i < _jsonStructure.Textures.Count; i++)
                {
                    _fileText += "          \"" + _jsonStructure.Textures[i].Name + "\": ";

                    string texRef = _jsonStructure.Textures[i].Path;
                    if (texRef.Contains("#"))
                        foreach (JsonTexture tex in _jsonStructure.Textures)
                        {
                            if (tex.Name == texRef.Substring(1))
                                texRef = tex.Path;
                        }
                    _fileText += "\"" + texRef + "\"";

                    if ((i + 1) < _jsonStructure.Textures.Count)
                        _fileText += ",";
                    _fileText += "\n";
                }
                _fileText += "          },\n";
            }
        }
        private void AddElementsText()
        {
            _fileText += "      \"elements\": [\n";
            for (int i = 0; i < _jsonStructure.Elements.Count; i++)
            {
                _fileText += "          {\n";
                JsonElement element = _jsonStructure.Elements[i];

                AddTabs(4);
                _fileText += "\"__comment\": ";
                _fileText += "\"" + element.Name + "\"";
                _fileText += ",\n";

                AddTabs(4);
                AddVector("from",element.From);
                _fileText += ",\n";

                AddTabs(4);
                AddVector("to", element.To);
                _fileText += ",\n";

                if (!element.Shade)
                {
                    AddTabs(4);
                    AddBool("shade", element.Shade);
                    _fileText += ",\n";
                }

                if (element.Rotation.Angle != 0)
                {
                    AddTabs(4);
                    _fileText += "\"rotation\": { ";
                    AddVector("origin", element.Rotation.Origin);
                    _fileText += ", \"axis\": \"" + element.Rotation.Axis.ToString() + "\", ";
                    AddFloat("angle", element.Rotation.Angle);
                    _fileText += ", ";
                    AddBool("rescale", element.Rotation.Rescale);
                    _fileText += " },\n";
                }

                AddFaces(element);
                _fileText += "          }";
                if ((i + 1) < _jsonStructure.Elements.Count)
                    _fileText += ",";
                _fileText += "\n";;
            }
            _fileText += "      ]\n";
        }
        private void AddFaces(JsonElement element)
        {
            AddTabs(4);
            _fileText += "\"faces\": {\n";
            bool addComma = false;
            for (int i = 0; i < element.Faces.Count; i++)
            {
                JsonFace face = element.Faces[i];
                if (face.Visible)
                {
                    if (addComma)
                    {
                        _fileText += ", ";
                        _fileText += "\n";
                        addComma = false;
                    }
                    AddTabs(5);
                    _fileText += "\"" + face.Orientation.ToString().ToLower() + "\": { ";
                    AddVector("uv", face.UV);
                    _fileText += ", \"texture\": " + "\"" + face.TextureName + "\"";
                    if (face.Rotation != 0)
                        _fileText += ", \"rotation\": " + face.Rotation.ToString();
                    if (face.TintIndex >= 0)
                        _fileText += ", \"tintindex\": " + face.TintIndex.ToString();
                    if (face.CullFace != CullFace.none)
                        _fileText += ", \"cullface\": " + "\"" + face.CullFace.ToString().ToLower() + "\"";
                    _fileText += " }";
                    addComma = true;
                }
            }
            _fileText += "\n              }\n";
        }
        private void AddTabs(int count)
        {
            for (int i = 0; i < count; i++)
                _fileText += "   ";
        }
        private void AddFloat(string var,float value)
        {
            _fileText += "\"" + var + "\": " + Math.Round(value,2).ToString("G",CultureInfo.CreateSpecificCulture("en-US"));
        }
        private void AddBool(string var,bool value)
        {
            _fileText += "\"" + var + "\": " + value.ToString().ToLower();
        }
        private void AddVector(string var, Vector3 value)
        {
            _fileText += "\"" + var + "\": [" + Math.Round(value.X,2).ToString("G",CultureInfo.CreateSpecificCulture("en-US")) + ", " +  Math.Round(value.Y,2).ToString("G",CultureInfo.CreateSpecificCulture("en-US")) + ", " +  Math.Round(value.Z,2).ToString("G",CultureInfo.CreateSpecificCulture("en-US")) + " ]";
        }
        private void AddVector(string var, Vector4 value)
        {
            _fileText += "\"" + var + "\": [" + Math.Round(value.X,2).ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + ", " + Math.Round(value.Y,2).ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + ", " + Math.Round(value.Z,2).ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + ", " + Math.Round(value.W,2).ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + " ]";
        }
    }
}
