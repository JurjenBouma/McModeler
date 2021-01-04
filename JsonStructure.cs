using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MCModeler
{
    public struct JsonTexture
    {
        public string Name;
        public string Path;
    }
    public class JsonStructure
    {
        public bool AmbientOcclusion;
        public List<JsonTexture> Textures;
        public List<JsonElement> Elements;
        public string FilePath;
        public string Name;

        public JsonStructure()
        {
            AmbientOcclusion = true;
            Textures = new List<JsonTexture>();
            Elements = new List<JsonElement>();
            FilePath = "";
            Name = "";
        }
        public JsonStructure(string path)
        {
            JsonReader reader = new JsonReader(path);
            JsonStructure jsonStruc = reader.GetjsonModel();
            if (jsonStruc != null)
            {
                jsonStruc.FilePath = path;
                FileInfo info = new FileInfo(path);
                jsonStruc.Name = info.Name;
                Copy(jsonStruc);
            }
            else
            {
                jsonStruc = new JsonStructure();
                Copy(jsonStruc);
            }
        }
        public JsonStructure(Model model)
        {
            AmbientOcclusion = model.AmbientOcclusion;
            Textures = new List<JsonTexture>();
            Elements = new List<JsonElement>();
            FilePath = model.FilePath;
            Name = model.Name;

            FillTexture(model);
            FillElements(model);
        }
        public void Save()
        {
            JsonWriter writer = new JsonWriter(this);
            writer.SaveJson();
        }
        public void Copy(JsonStructure model)
        {
            AmbientOcclusion = model.AmbientOcclusion;
            Textures = new List<JsonTexture>(model.Textures);
            Elements = new List<JsonElement>(model.Elements);
            FilePath = model.FilePath;
            Name = model.Name;
        }

        private void FillTexture(Model model)
        {
            foreach( ModelTexture tex in model.Textures)
            {
                if (tex.Name != "MissingT")
                {
                    JsonTexture jTex = new JsonTexture();
                    jTex.Name = tex.Name;
                    jTex.Path = tex.GamePath;
                    if (tex.IsReference)
                        jTex.Path = tex.Reference;
                    Textures.Add(jTex);
                }
            }
        }
        private void FillElements(Model model)
        {
            int i = 0;
            while (true)
            {
                Box box = model.GetBox(i);
                if (box == null)
                    break;
                Elements.Add(box.GetElement());
                i++;
            }
        }
    }
}
