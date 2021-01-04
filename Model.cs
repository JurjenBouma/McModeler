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
    public  delegate void SelectedBoxChangedHandler(Model sender,Box activeBox);
    public class Model
    {
        List<Box> _boxes;
        List<ModelTexture> _textures;
        string _name;
        public bool AmbientOcclusion;
        public string FilePath;
        TreeNodeModel _treeNode;

        public List<Box> Boxes { get { return _boxes; } }
        public List<ModelTexture> Textures { get { return _textures; } }
        public TreeNodeModel TreeNode { get { return _treeNode; } set { _treeNode = value; _treeNode.Link(this); } }
        public string Name { get { return _name; } set { _name = value; _treeNode.UpdateName(); } }

        public SelectedBoxChangedHandler SelectedBoxChanged;

        public Model(string name)
        {
            _boxes = new List<Box>();
            _textures = new List<ModelTexture>();
            FilePath = "";
            AddTexture("MissingT", "Tool\\MissingTexture");
            _name = name;
            AmbientOcclusion = true;
            _treeNode = new TreeNodeModel(this);
        }
        public Model(JsonStructure jsonModel)
        {
            _boxes = new List<Box>();
            _textures = new List<ModelTexture>();
            FilePath = jsonModel.FilePath;
            AddTexture("MissingT", "Tool\\MissingTexture");
            _name = jsonModel.Name;
            AmbientOcclusion = jsonModel.AmbientOcclusion;

            foreach (JsonTexture tex in jsonModel.Textures)
            {
                AddTexture(tex.Name, tex.Path);
            }
            foreach(JsonElement element in jsonModel.Elements)
            {
                AddBox(new Box(element,this));
            }

            _treeNode = new TreeNodeModel(this);
        }
        public ModelTexture GetTexture(int index)
        {
            if (index >= 0 && index < _textures.Count)
                return _textures[index];
            else
                return _textures[0];
        }
        public ModelTexture GetTexture(string name)
        {
            if (name.Length > 0)
                foreach (ModelTexture tex in _textures)
                {
                    if (tex.Name == name.Substring(1))
                    {
                        return tex;
                    }
                }
            return _textures[0];
        }
        
        public Box GetBox(int index)
        {
            if (index >= 0 && index < _boxes.Count)
                return _boxes[index];
            else
                return null;
        }

        public void AddBox(Box box) 
        {
            _boxes.Add(box);
            if (_treeNode != null)
            {
                _treeNode.AddTreeNodeElement(box.TreeNode);
            }
        }
        public void RemoveBox(Box box) 
        {
            if (_treeNode != null)
                _treeNode.RemoveTreeNodeElement(box.TreeNode);
            _boxes.Remove(box);
            box.Dispose();   
        }
        public Box GetSelectedBox()
        {
            foreach (Box box in _boxes)
            {
                if (box.IsSelected)
                {
                    return box;
                }
            }
            return null;
        }
        public void SetSelectedBox(Box selectedBox)
        {
            for (int i = 0; i < _boxes.Count; i++)
            {
                Box box = _boxes[i];
                if (box == selectedBox)
                    SetSelectedBox(i);
            }
        }
        public void SetSelectedBox(int index)
        {
            Box selected = GetSelectedBox();
            for (int i = 0; i < _boxes.Count; i++)
            {
                Box box = _boxes[i];
                if (i != index)
                    box.IsSelected = false;
                else
                    box.IsSelected = true;
            }
            if (selected != GetSelectedBox())
                if (SelectedBoxChanged != null)
                    SelectedBoxChanged(this,GetSelectedBox());
        }

        public void AddTexture(string name, string texPath) 
        {
            ModelTexture tex = new ModelTexture(name, texPath, this);
            tex.NameChanged += new NameChangedEventHandler(On_Tex_Name_Changed);
            _textures.Add(tex);
            if (_treeNode != null)
            {
                _treeNode.AddTreeNodeTexture(tex.TreeNode);
                _treeNode.UpdateTextureOptions();
            }
        }
        public void RemoveTexture(string name)
        {
            for (int i = 0; i < _textures.Count; i++)
            {
                if (_textures[i].Name == name)
                {
                    if (_treeNode != null)
                    {
                        _treeNode.RemoveTreeNodeTexture(_textures[i].TreeNode);
                        _treeNode.UpdateTextureOptions();
                    }
                    _textures[i].Dispose();
                    _textures.RemoveAt(i);
                }
            }
        }
        public bool ContainsTexture(string textureName)
        {
            bool result = false;
            foreach (ModelTexture tex in _textures)
            {
                if (tex.Name == textureName.Substring(1))
                {
                    result = true;
                }
            }
            return result;
        }

        public void SetActiveTexture(string textureName)
        {
            ModelTexture tex = GetTexture(textureName);
            if (!tex.IsReference)
                tex.SetActive();
            else
                SetActiveTexture(tex.Reference);
            return;

        }
        public string[] GetTextureNames()
        {
            List<string> names = new List<string>();
            foreach (ModelTexture tex in _textures)
            {
                if (tex.Name != "MissingT")
                {
                    names.Add(tex.Name);
                }
            }
            return names.ToArray();
        }
        public void MoveSelectedBox(Vector3 amount)
        {
            foreach (Box box in _boxes)
            {
                if (box.IsSelected)
                    box.Move(amount);
            }
        }

        public void ApplyMove()
        {
            foreach (Box box in _boxes)
            {
                if (box.IsSelected)
                {
                    box.ApplyEdit();
                }
            }
        }

        public int Pick(float x, float y,out Vector3 hitPoint,out float dist)
        {
            bool hit = false;
            int pickedBoxIndex = -1;
            float closest = float.MaxValue;
            hitPoint = Vector3.Zero;
            for (int i = 0; i < _boxes.Count; i++)
            {
                Box box = _boxes[i];
                float boxDist;
                Vector3 boxDragPoint;
                if (box.Pick(x, y, out boxDragPoint, out boxDist))
                {
                    if (boxDist < 0) continue;
                    if (boxDist < closest)
                    {
                        closest = boxDist;
                        pickedBoxIndex = i;
                        hit = true;
                        hitPoint = boxDragPoint;
                    }
                }
            }
            if (hit)
                dist = closest;
            else 
                dist = - 1;

            return pickedBoxIndex;
        }
        public int Pick(float x, float y,out Vector3 hitPoint)
        {
            float waste;
            return Pick(x, y, out hitPoint, out waste); ;
        }

        public int Pick(float x, float y)
        {
            Vector3 waste;
            float wastefloat;
            return Pick(x, y, out waste, out  wastefloat); ;
        }

        public void Draw()
        {
            for (int p = 0; p < Renderer.TexturedTechnique.Description.PassCount; p++)
            {
                foreach (Box box in _boxes)
                {
                    if (!box.IsSelected)
                        box.Draw(Renderer.TexturedTechnique, p);
                }
            }
            for (int p = 0; p < Renderer.SelectedTechnique.Description.PassCount; p++)
            {
                foreach (Box box in _boxes)
                {
                    if (box.IsSelected)
                        box.Draw(Renderer.SelectedTechnique, p);
                }
            }
        }
        public void Dispose()
        {
            foreach (Box box in _boxes)
            {
                box.Dispose();
            }
            foreach (ModelTexture tex in _textures)
            {
                tex.Dispose();
            }
        }
        private void On_Tex_Name_Changed(ModelTexture texture,string oldName)
        {
            for (int i = 0; i < _boxes.Count; i++)
            {
                _boxes[i].ChangeTextureRef("#" + oldName, "#" + texture.Name);
            }
            _treeNode.UpdateTextureOptions();
        }
    }
}
