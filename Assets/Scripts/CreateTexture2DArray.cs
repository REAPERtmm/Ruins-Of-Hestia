using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class CreateTexture2DArray : MonoBehaviour
{
    public bool UPDATE = false;

    public Texture2D[] Textures;
    public Texture2DArray Array;
    public string FileName;

    void Update()
    {
        if(UPDATE)
        {
            if (Textures == null || Textures.Length == 0) return;
          
            int W = Textures[0].width;
            int H = Textures[0].height;
            TextureFormat FORMAT = Textures[0].format;
            for (int i = 1; i < Textures.Length; i++)
            {
                if (Textures[i].width != W || Textures[i].height != H || Textures[i].format != FORMAT)
                {
                    Debug.LogWarning("Does not support different sized textures to create a texture array !");
                    return;
                }
            }

            Debug.Log("Texture Width  : " + W);
            Debug.Log("Texture Height : " + H);
            Debug.Log("Texture Format : " + FORMAT);

            Array = new Texture2DArray(W, H, Textures.Length, FORMAT, false);
            for (int i = 0; i < Textures.Length; i++)
            {
                Graphics.CopyTexture(Textures[i], 0, 0, Array, i, 0);
            }
            Array.Apply(false);

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(Array, "Assets/TextureArrays/" + FileName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Texture2DArray saved at: " + FileName);
#else
            Debug.LogWarning("Saving Texture2DArray assets is only supported in the Editor.");
#endif

            UPDATE = false;
        }
    }
}
