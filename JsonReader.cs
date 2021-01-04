using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
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
    class JsonReader
    {
        string _filePath;
        JsonStructure _jsonModel;
        string _fileText;
        int _readIndex = 0;

        public JsonReader(string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            if (info.Extension == ".json")
            {
                _filePath = filePath;
                Read();
            }
        }

        public JsonStructure GetjsonModel()
        {
            return _jsonModel;
        }

        private void Read()
        {
            _jsonModel = new JsonStructure();
            _fileText = File.ReadAllText(_filePath);
            _readIndex = 0;

            bool done = false;
            while (!done)
            {
                int varNameIndex = _fileText.IndexOf("\"", _readIndex) + 1;
                int varNameLenght = _fileText.IndexOf("\"", varNameIndex) - varNameIndex;
                string varName = _fileText.Substring(varNameIndex, varNameLenght);
                _readIndex = _fileText.IndexOf(":",_readIndex)+1;
                int result = ParseVariable(varName);

                if (result == 0)
                    done = true;
            }
        }
        private int ParseVariable(string varName)
        {
            if (varName == "parent")
            {
                return ParseParent();
            }
            if (varName == "ambientocclusion")
            {
                return ParseAmbientOcclusion();
            }
            if (varName == "textures")
            {
                return ParseTextures();
            }
            if (varName == "elements")
            {
                return ParseElements();
            }
            if (varName == "__comment")
            {
                int dataEnd = _fileText.IndexOf(",", _readIndex);
                if (dataEnd < 0)
                    dataEnd = _fileText.IndexOf("}", _readIndex);
                _readIndex = dataEnd + 1;
                return 1;
            }
            return 0;
        }

        private int ParseParent()
        {
            int dataEnd = _fileText.IndexOf(",", _readIndex);
            if (dataEnd < 0)
                dataEnd = _fileText.IndexOf("}", _readIndex);
            string value = _fileText.Substring(_readIndex);

            int pathIndex = value.IndexOf("\"") + 1;
            int pathLenght = value.IndexOf("\"", pathIndex) - pathIndex;
            string relativePath = value.Substring(pathIndex, pathLenght);

            relativePath = relativePath.Replace("/", "\\");
            string modelFolder = relativePath.Substring(0,relativePath.IndexOf("\\"));
            string fullPath = "";
            if(_filePath.Contains(modelFolder))
                   fullPath = _filePath.Substring(0, _filePath.IndexOf(modelFolder)) + relativePath +".json";
            
            string partentText = "";
            if(File.Exists(fullPath))
                partentText = File.ReadAllText(fullPath);
            else
                partentText = File.ReadAllText("models\\" + relativePath + ".json");

             _readIndex = dataEnd + 1;
             _fileText = partentText + _fileText.Substring(_readIndex);

             _readIndex = 0;
            return 1;
        }
        private int ParseAmbientOcclusion()
        {
            int dataEnd = _fileText.IndexOf(",", _readIndex);
            if (dataEnd < 0)
                dataEnd = _fileText.IndexOf("}", _readIndex);
            string value = _fileText.Substring(_readIndex, dataEnd - _readIndex);
            if (value.Contains("false"))
                _jsonModel.AmbientOcclusion = false;
            else if (value.Contains("true"))
                _jsonModel.AmbientOcclusion = true;

            _readIndex = dataEnd + 1;
            return  1;
        }
        private int ParseTextures()
        {
            int dataEnd = _fileText.IndexOf("}", _readIndex);
            string value = _fileText.Substring(_readIndex);

            bool done = false;
            while(!done)
            {
                int nameIndex = value.IndexOf("\"") + 1;
                int nameLenght = value.IndexOf("\"", nameIndex) - nameIndex;
                string name = value.Substring(nameIndex, nameLenght);
                value = value.Substring(value.IndexOf(":")+1);

                int pathIndex = value.IndexOf("\"") + 1;
                int pathLenght = value.IndexOf("\"", pathIndex) - pathIndex;
                string path = value.Substring(pathIndex, pathLenght);

                JsonTexture tex = new JsonTexture();
                tex.Name = name;
                tex.Path = path;
                _jsonModel.Textures.Add(tex);

                int nextComma = value.IndexOf(",");
                int nextCloseBrack = value.IndexOf("}");
                if (nextComma < 0 || nextComma > nextCloseBrack)
                    done = true;
                else
                    value = value.Substring(nextComma + 1);

            }
            _readIndex = dataEnd + 1;
            return 1;
        }

        private int ParseElements()
        {
            int dataEnd = _fileText.LastIndexOf("]");
            string value = _fileText.Substring(_readIndex, dataEnd - _readIndex);

            JsonElement element = new JsonElement();
            bool done = false;
            while(!done)
            {
                int nameIndex = value.IndexOf("\"") + 1;
                int nameLenght = value.IndexOf("\"", nameIndex) - nameIndex;
                string name = value.Substring(nameIndex, nameLenght);
                value = value.Substring(value.IndexOf(":") + 1);

                if (name == "__comment")
                {
                    int commentIndex = value.IndexOf("\"") + 1;
                    int commentLenght = value.IndexOf("\"", commentIndex) - commentIndex;
                    string comment = value.Substring(commentIndex, commentLenght);

                    element.Name = comment;
                    value = value.Substring(commentIndex + commentLenght + 1);
                }
                if (name == "from")
                {
                    value = value.Substring(value.IndexOf("[") + 1);
                    double x = System.Xml.XmlConvert.ToDouble(value.Substring(0, value.IndexOf(",")));

                    value = value.Substring(value.IndexOf(",") + 1);
                    double y = System.Xml.XmlConvert.ToDouble(value.Substring(0, value.IndexOf(",")));

                    value = value.Substring(value.IndexOf(",") + 1);
                    double z = System.Xml.XmlConvert.ToDouble(value.Substring(0, value.IndexOf("]")));

                    element.From = new Vector3((float)x, (float)y, (float)z);

                    value = value.Substring(value.IndexOf("]") + 1);
                }
                if (name == "to")
                {
                    value = value.Substring(value.IndexOf("[") + 1);
                    double x = System.Xml.XmlConvert.ToDouble(value.Substring(0, value.IndexOf(",")));

                    value = value.Substring(value.IndexOf(",") + 1);
                    double y = System.Xml.XmlConvert.ToDouble(value.Substring(0, value.IndexOf(",")));

                    value = value.Substring(value.IndexOf(",") + 1);
                    double z = System.Xml.XmlConvert.ToDouble(value.Substring(0, value.IndexOf("]")));

                    element.To = new Vector3((float)x, (float)y, (float)z);

                    value = value.Substring(value.IndexOf("]") + 1);
                }
              
                if (name == "shade")
                {
                    int boolEnd;
                    int nextComma = value.IndexOf(",");
                    int nextCloseBrack = value.IndexOf("}");
                    if (nextComma < 0 || nextComma > nextCloseBrack)
                        boolEnd = nextCloseBrack;
                    else
                        boolEnd = nextComma;

                    string boolString = value.Substring(0,boolEnd);

                    if (value.Contains("false"))
                        element.Shade = false;
                    else if (value.Contains("true"))
                        element.Shade = true;

                    value = value.Substring(boolEnd + 1);
                }
                #region Faces

                if (name == "faces")
                {
                    /////todo
                    bool facesDone = false;
                    while (!facesDone)
                    {
                        JsonFace face = new JsonFace();
                        face.Visible = true;
                        int faceNameIndex = value.IndexOf("\"") + 1;
                        int faceNameLenght = value.IndexOf("\"", faceNameIndex) - faceNameIndex;
                        string faceName = value.Substring(faceNameIndex, faceNameLenght);
                        value = value.Substring(value.IndexOf(":") + 1);

                        if (faceName == "north")
                            face.Orientation = FaceOrientation.north;
                        if (faceName == "south")
                            face.Orientation = FaceOrientation.south;
                        if (faceName == "up")
                            face.Orientation = FaceOrientation.up;
                        if (faceName == "down")
                            face.Orientation = FaceOrientation.down;
                        if (faceName == "east")
                            face.Orientation = FaceOrientation.east;
                        if (faceName == "west")
                            face.Orientation = FaceOrientation.west;


                        bool parasDone = false;
                        string faceData = value.Substring(0, value.IndexOf("}"));
                        while (!parasDone)
                        {
                            int parNameIndex = faceData.IndexOf("\"") + 1;
                            int parNameLenght = faceData.IndexOf("\"", parNameIndex) - parNameIndex;
                            string parName = faceData.Substring(parNameIndex, parNameLenght);
                            faceData = faceData.Substring(faceData.IndexOf(":") + 1);
                            int nextComma = faceData.IndexOf(",");
                            if (parName == "uv")
                            {
                                faceData = faceData.Substring(faceData.IndexOf("[") + 1);
                                double x1 = System.Xml.XmlConvert.ToDouble(faceData.Substring(0, faceData.IndexOf(",")));

                                faceData = faceData.Substring(faceData.IndexOf(",") + 1);
                                double y1 = System.Xml.XmlConvert.ToDouble(faceData.Substring(0, faceData.IndexOf(",")));

                                faceData = faceData.Substring(faceData.IndexOf(",") + 1);
                                double x2 = System.Xml.XmlConvert.ToDouble(faceData.Substring(0, faceData.IndexOf(",")));

                                faceData = faceData.Substring(faceData.IndexOf(",") + 1);
                                double y2 = System.Xml.XmlConvert.ToDouble(faceData.Substring(0, faceData.IndexOf("]")));

                                face.UV = new Vector4((float)x1, (float)y1, (float)x2, (float)y2);
                            }
                            if (parName == "texture")
                            {
                                int texNameIndex = faceData.IndexOf("\"") + 1;
                                int texNameLenght = faceData.IndexOf("\"", texNameIndex) - texNameIndex;
                                string texName = faceData.Substring(texNameIndex, texNameLenght);
                                face.TextureName = texName;
                            }
                            if (parName == "rotation")
                            {
                                if (nextComma < 0)
                                    face.Rotation = Convert.ToInt32(faceData);
                                else
                                    face.Rotation = Convert.ToInt32(faceData.Substring(0, nextComma));
                            }
                            if (parName == "tintindex")
                            {
                                if (nextComma < 0)
                                    face.TintIndex = Convert.ToInt32(faceData);
                                else
                                    face.TintIndex = Convert.ToInt32(faceData.Substring(0, nextComma));
                            }
                            if (parName == "cullface")
                            {
                                int cullFaceIndex = faceData.IndexOf("\"") + 1;
                                int cullFaceLenght = faceData.IndexOf("\"", cullFaceIndex) - cullFaceIndex;
                                string cullFaceName = faceData.Substring(cullFaceIndex, cullFaceLenght);

                                if (cullFaceName == "down")
                                    face.CullFace = CullFace.down;
                                if (cullFaceName == "up")
                                    face.CullFace = CullFace.up;
                                if (cullFaceName == "north")
                                    face.CullFace = CullFace.north;
                                if (cullFaceName == "south")
                                    face.CullFace = CullFace.south;
                                if (cullFaceName == "west")
                                    face.CullFace = CullFace.west;
                                if (cullFaceName == "east")
                                    face.CullFace = CullFace.east;
                            }

                            nextComma = faceData.IndexOf(",");
                            if (nextComma < 0)
                                parasDone = true;
                            else
                                faceData = faceData.Substring(nextComma + 1);

                        }
                        element.Faces.Add(face);
                        value = value.Substring(value.IndexOf("}") + 1);

                        int commaindex = value.IndexOf(",");
                        int closeBrackindex = value.IndexOf("}");
                        if (commaindex < 0 || commaindex > closeBrackindex)
                        {
                            value = value.Substring(value.IndexOf("}") + 1);
                            facesDone = true;
                        }
                    }

                }
#endregion Faces
                #region Rotation
                if (name == "rotation")
                {
                    ElementRotation rotation = new ElementRotation();
                    string rotData = value.Substring(0, value.IndexOf("}"));

                    bool parasDone = false;
                    while (!parasDone)
                    {
                        int parameterNameIndex = rotData.IndexOf("\"") + 1;
                        int parameterNameLenght = rotData.IndexOf("\"", parameterNameIndex) - parameterNameIndex;
                        string parameterName = rotData.Substring(parameterNameIndex, parameterNameLenght);
                        rotData = rotData.Substring(rotData.IndexOf(":") + 1);
                        int nextComma = rotData.IndexOf(",");

                        if (parameterName == "origin")
                        {
                            rotData = rotData.Substring(rotData.IndexOf("[") + 1);
                            double x = System.Xml.XmlConvert.ToDouble(rotData.Substring(0, rotData.IndexOf(",")));

                            rotData = rotData.Substring(rotData.IndexOf(",") + 1);
                            double y = System.Xml.XmlConvert.ToDouble(rotData.Substring(0, rotData.IndexOf(",")));

                            rotData = rotData.Substring(rotData.IndexOf(",") + 1);
                            double z = System.Xml.XmlConvert.ToDouble(rotData.Substring(0, rotData.IndexOf("]")));

                            rotation.Origin = new Vector3((float)x, (float)y, (float)z);
                        }
                        if (parameterName == "axis")
                        {
                            int axIndex = rotData.IndexOf("\"") + 1;
                            int axLenght = rotData.IndexOf("\"", axIndex) - axIndex;
                            string axName = rotData.Substring(axIndex, axLenght);

                            if (axName == "x")
                                rotation.Axis = Axis.x;
                            if (axName == "y")
                                rotation.Axis = Axis.y;
                            if (axName == "z")
                                rotation.Axis = Axis.z;
                        }
                        if (parameterName == "angle")
                        {
                            if (nextComma < 0)
                                rotation.Angle = (float)System.Xml.XmlConvert.ToDouble(rotData);
                            else
                                rotation.Angle = (float)System.Xml.XmlConvert.ToDouble(rotData.Substring(0, nextComma));
                        }
                        if (parameterName == "rescale")
                        {
                            string boolScale;
                            if (nextComma >= 0)
                                boolScale = rotData.Substring(0, nextComma);
                            else
                                boolScale = rotData;

                            if (boolScale.Contains("true"))
                                rotation.Rescale = true;
                            if (boolScale.Contains("false"))
                                rotation.Rescale = false;
                        }
                        nextComma = rotData.IndexOf(",");
                        if (nextComma < 0)
                            parasDone = true;
                        else
                            rotData = rotData.Substring(nextComma+1);
                    }
                    element.Rotation = rotation;
                    value = value.Substring( value.IndexOf("}") + 1);
                }
                #endregion Rotation

                int commaIndex = value.IndexOf(",");
                int closeBrackIndex = value.IndexOf("}");
                if (commaIndex > closeBrackIndex)
                {
                    _jsonModel.Elements.Add(element);
                    element = new JsonElement();
                }
                if (commaIndex < 0)
                {
                    _jsonModel.Elements.Add(element);
                    done = true;
                }
            }
            _readIndex = dataEnd + 1;
            return 1;
        }
             
    }
}
