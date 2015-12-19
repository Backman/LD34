using UnityEngine;

namespace UnityEditor
{
    public class PixelPerUnityPostprocess : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            var importer = (TextureImporter)assetImporter;
            importer.spritePixelsPerUnit = 32;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        }
    }
}