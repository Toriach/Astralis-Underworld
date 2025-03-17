using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public float scale = 20f;
    public int Width = 256;
    public int Height = 256;
    public Renderer Renderer;

    private float oldScale;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        Renderer.material.mainTexture = GenerateTexture();
        oldScale = scale;
    }

    private Texture GenerateTexture()
    {
        Texture2D texture = new Texture2D(Width, Height);

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Color color = CalculateColor(i,j);
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply();
        return texture;
    }

    private Color CalculateColor(int i, int j)
    {
        float xCored = (float)i / Width * scale;
        float yCored = (float)j / Height * scale;

        float sample = Mathf.PerlinNoise(xCored, yCored);
        return new Color(sample, sample, sample);
    }

    private void Update()
    {
        if(oldScale ==  scale) return;
        Renderer.material.mainTexture = GenerateTexture();
        oldScale = scale;
    }
}
