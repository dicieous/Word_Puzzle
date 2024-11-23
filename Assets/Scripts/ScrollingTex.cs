using UnityEngine;

public class ScrollingTex : MonoBehaviour
{
    [SerializeField] private Material mat;
    public float scrollSpeed;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        mat.mainTextureOffset += new Vector2(0f, scrollSpeed * Time.deltaTime);
    }
}