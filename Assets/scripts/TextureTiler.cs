using UnityEngine;

[ExecuteInEditMode]
public class TextureTiler : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
		// Would be nice to get this working in the editor without running the game. 

        Renderer renderer = GetComponent<Renderer>();

        float xScale = transform.localScale.x;
		float yScale = transform.localScale.y;
        renderer.sharedMaterial.mainTextureScale = new Vector2(xScale, yScale);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void UpdateTextureScale()
    {

    }
}
