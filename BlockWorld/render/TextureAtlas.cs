using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OTKUtilities;

namespace BlockWorld.render
{
    class TextureAtlas
    {
        public Dictionary<string, int> TextureMap;
        public TextureArray Array;

        public TextureAtlas(int width, int height, int layers)
        {
            Array = new TextureArray(width, height, layers);
            TextureMap = new Dictionary<string, int>();
        }

        public void SetImageAt(int layer, string filename)
        {
            Array.SetImageAt(layer, filename);
        }

        public void AddImage(string resource_id, string filename)
        {
            if(TextureMap.Count < Array.MaxSize)
            {
                SetImageAt(TextureMap.Count, filename);
                TextureMap.Add(resource_id, TextureMap.Count);
            }
        }

        public int GetImage(string resource_id)
        {
            if (TextureMap.TryGetValue(resource_id, out int v))
                return v;
            return 0;
        }

        public void Bind()
        {
            Array.Bind();
        }
    }
}
