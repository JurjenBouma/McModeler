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
using System.IO;
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
    public delegate void NameChangedEventHandler(ModelTexture texture,string oldName);
    public class ModelTexture
    {
        public string GamePath;
        string _filePath;
        string _name;
        ShaderResourceView _texture;
        bool _isReference;
        string _referenceName;
        TreeNodeTexture _treeNode;
        Model _parentModel;

        public Model ParentModel { get { return _parentModel; } }
        public NameChangedEventHandler NameChanged;

        public string FilePath { get { return _filePath; } set { _filePath = value; Dispose(); LoadTexture(); } }
        public string Name { get { return _name; } set { string oldName = _name; _name = value; On_Name_Changed(oldName); } }
        public string Reference { get { return _referenceName; } set { _referenceName = value; } }
        public bool IsReference { get { return _isReference; } }
        public TreeNodeTexture TreeNode { get { return _treeNode; } set { _treeNode = value; _treeNode.Link(this); } }

        public ModelTexture(string name, string texturePath,Model parentModel)
        {
            _name = name;
            _parentModel = parentModel;
            string validPath = texturePath.Replace("/", "\\");
            if (Global.IsPath(validPath))
            {
                _isReference = false;
                if (Global.IsRelativePath(validPath))
                {
                    validPath += ".png";
                    GamePath = texturePath;
                    if (_parentModel.FilePath.Length > 0)
                    {
                        if (_parentModel.FilePath.LastIndexOf("\\assets\\minecraft") > 0)
                            _filePath = _parentModel.FilePath.Substring(0, _parentModel.FilePath.LastIndexOf("\\assets\\minecraft"));
                        _filePath += "\\assets\\minecraft\\textures\\" + validPath;
                    }
                    if (!File.Exists(_filePath))
                        _filePath = "textures\\" + validPath;
                }
                else
                {
                    int startIndex = texturePath.LastIndexOf("textures\\");
                    startIndex += "textures\\".Length;
                    GamePath = texturePath.Substring(startIndex).Replace("\\", "/");
                    GamePath = GamePath.Substring(0, GamePath.Length - 4);
                    _filePath = texturePath;
                }
                LoadTexture();
            }
            else
            {
                _isReference = true;
                _referenceName = texturePath;
            }
            _treeNode = new TreeNodeTexture(this);
        }
        private void LoadTexture()
        {
            try {_texture = ShaderResourceView.FromFile(Renderer.device, _filePath); }
            catch { }
        }
        public void SetActive()
        {
            if (!_isReference)
                if (_texture != null)
                    Renderer.SetTexture(_texture);
        }
        public void Dispose()
        {
            if(_texture != null)
                _texture.Dispose();
        }

        private void On_Name_Changed(string oldName)
        {
            if(NameChanged != null)
            {
                NameChanged(this, oldName);
            }
        }
    }
}
