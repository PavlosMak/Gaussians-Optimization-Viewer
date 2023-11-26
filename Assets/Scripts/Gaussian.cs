using UnityEngine;

public class Gaussian
{
    public Vector3 Position { get; }
    public Vector3 Scale { get; }
    public Quaternion Rotation { get; }
    public float Opacity { get; }

    public Gaussian(Vector3 position, Vector3 scale, Quaternion rotation, float opacity)
    {
        Position = position;
        Scale = scale;
        Rotation = rotation;
        Opacity = opacity;
    }

    public GameObject ToEllipsoid()
    {
        GameObject ellipsoid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // Set the ellipsoid's position
        ellipsoid.transform.position = this.Position;

        Debug.Log(this.Scale);
        // Set the ellipsoid's scale
        ellipsoid.transform.localScale = this.Scale;
        
        
        // Set the ellipsoid's rotation
        ellipsoid.transform.rotation = this.Rotation;

        // Set opacity (change material color or transparency based on opacity value)
        Renderer renderer = ellipsoid.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color currentColor = renderer.material.color;
            renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, this.Opacity);
        }

        return ellipsoid;
    }
}