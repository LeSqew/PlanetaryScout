using UnityEngine;

public static class SpectrumTextureGenerator
{
    public static Texture2D GenerateSpectrumTexture(double[] measured, double[] target, int width = 256, int height = 64)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color(0, 0, 0, 1);

        int n = measured.Length;

        for (int x = 0; x < width; x++)
        {
            int idx = Mathf.Clamp(Mathf.RoundToInt((float)x / (width - 1) * (n - 1)), 0, n - 1);
            int yM = Mathf.Clamp(Mathf.RoundToInt((float)measured[idx] * (height - 1)), 0, height - 1);
            int yT = Mathf.Clamp(Mathf.RoundToInt((float)target[idx] * (height - 1)), 0, height - 1);

            pixels[yM * width + x] = Color.cyan;
            pixels[yT * width + x] = Color.red;
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }  
}
