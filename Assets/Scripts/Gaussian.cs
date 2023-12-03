using Unity.VisualScripting;
using UnityEngine;

public class Gaussian
{
    public Vector3 Position { get; }
    public Vector3 Scale { get; }
    public Quaternion Rotation { get; }
    public float Opacity { get; }

    public Vector3 Features { get; }

    private bool featuresAreRGB = false;

    public Gaussian(Vector3 position, Vector3 scale, Quaternion rotation, float opacity, Vector3 colorFeatures,
        bool featuresAreRGB = false)
    {
        Position = position;
        Scale = scale;
        Rotation = rotation;
        Opacity = opacity;
        Features = colorFeatures;
        this.featuresAreRGB = featuresAreRGB;
    }

    public GameObject ToEllipsoid()
    {
        GameObject ellipsoid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // Set the ellipsoid's position
        ellipsoid.transform.position = this.Position;

        // Set the ellipsoid's scale
        ellipsoid.transform.localScale = this.Scale;

        // Set the ellipsoid's rotation
        ellipsoid.transform.rotation = this.Rotation;


        return ellipsoid;
    }

    public Color SH2RGB()
    {
        if (this.featuresAreRGB)
        {
            return new Color(this.Features.x / 255.0f, this.Features.y / 255.0f, this.Features.z / 255.0f);
        }

        float C0 = 0.28209479177387814f;

        float red = Mathf.Max(0.0f, 0.5f + C0 * this.Features.x);
        float green = Mathf.Max(0.0f, 0.5f + C0 * this.Features.y);
        float blue = Mathf.Max(0.0f, 0.5f + C0 * this.Features.z);

        Color color = new Color(red, green, blue);
        return color;
    }
}