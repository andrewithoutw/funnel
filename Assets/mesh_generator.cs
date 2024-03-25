using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MeshFilter))]
public class mesh_generator : MonoBehaviour
{
    const float PI = 3.1415f;
    public int segments = 64;
    float funnelHeight = 1.0f;
    float funnelRadius = 1.0f;
    float tubeRadius = 1.0f;
    float tubeLength = 1.0f;
    float txtScale = 1.0f;
    public bool perFace = false;

    public Slider fHeightSlider;
    public Slider fRadiusSlider;
    public Slider tLengthSlider;
    public Slider tRadiusSlider;

    public DataManager dataManager;

    Mesh mesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        fRadiusSlider.onValueChanged.AddListener(delegate { updateFromSlider(); });
        fHeightSlider.onValueChanged.AddListener(delegate { updateFromSlider(); });
        tLengthSlider.onValueChanged.AddListener(delegate { updateFromSlider(); });
        tRadiusSlider.onValueChanged.AddListener(delegate { updateFromSlider(); });
    }

   void createFunnel()
    {
        float step = (PI * 2.0f) / segments;
        for (int i = 0; i < segments; i++)
        {
            float r = i * step;
            float x = Mathf.Cos(r);
            float z = Mathf.Sin(r);
            vertices.Add(new Vector3(x * funnelRadius, funnelHeight, z * funnelRadius));
            vertices.Add(new Vector3(x * tubeRadius, 0f , z * tubeRadius));
            vertices.Add(new Vector3(x * tubeRadius, -tubeLength, z * tubeRadius));

            if(perFace)
            {
                if ((float)i % 2 == 0)
                {
                    uvs.Add(new Vector2(0.0f, 1.0f));
                    uvs.Add(new Vector2(0.0f, 0.5f));
                    uvs.Add(new Vector2(0.0f, 0.0f));
                }
                else
                {
                    uvs.Add(new Vector2(1.0f, 1.0f));
                    uvs.Add(new Vector2(1.0f, 0.5f));
                    uvs.Add(new Vector2(1.0f, 0.0f));
                }
            }
            else
            {
                float w = (float)i / segments;
                
                uvs.Add(new Vector2(w, 1f));
                uvs.Add(new Vector2(w, 0.5f));
                uvs.Add(new Vector2(w, 0f));
            }

        }
        /////handle seam
        if (!perFace)
        {
            vertices.Add(new Vector3(funnelRadius, funnelHeight, 0f));
            vertices.Add(new Vector3(tubeRadius, 0f, 0f));
            vertices.Add(new Vector3(tubeRadius, -tubeLength, 0f));

            uvs.Add(new Vector2(0.999f, 1f));
            uvs.Add(new Vector2(0.999f, 0.5f));
            uvs.Add(new Vector2(0.999f, 0f));
        }

        ///Indices
        for (int i = 0; i < segments; i++)
        {
            int a = i * 3;
            int b = a + 1;
            int c = a + 2;
            int d = a + 3;
            int e = a + 4;
            int f = a + 5;

            if (perFace)
            {
                modulateIndices(ref b, ref c, ref d, ref e, ref f);
            }
            //funnel
            indices.Add(a); indices.Add(d); indices.Add(b);
            indices.Add(d); indices.Add(e); indices.Add(b);
            //tube
            indices.Add(b); indices.Add(e); indices.Add(c);
            indices.Add(e); indices.Add(f); indices.Add(c);
        }
   }

    void modulateIndices(ref int b, ref int c, ref int d, ref int e, ref int f)
    {
        b = b % (segments * 3);
        c = c % (segments * 3);
        d = d % (segments * 3);
        e = e % (segments * 3);
        f = f % (segments * 3);
    }

    void CreateShape()
    {
         vertices.Clear();
         indices.Clear();
         uvs.Clear();
         ///Funnel
         createFunnel();
    }

    private void OnValidate()
    {
        CreateShape();
        UpdateMesh();
    }

    public void GenerateClick()
    {
        CreateShape();
        UpdateMesh();
    }
    public void SaveClick()
    {
        if (dataManager.data == null)
        {
            dataManager.data = new Data();
        }
        dataManager.data.vertices = vertices;
        dataManager.data.indices = indices;
        dataManager.data.uvs = uvs;
        dataManager.Save();
    }
    public void LoadClick()
    {
       dataManager.Load();
       vertices = dataManager.data.vertices;
       indices = dataManager.data.indices;
       uvs = dataManager.data.uvs;
       UpdateMesh();
    }

    void updateFromSlider()
    {
        funnelHeight = fHeightSlider.value;
        funnelRadius = fRadiusSlider.value * 0.5f;
        tubeLength = tLengthSlider.value;
        tubeRadius = tRadiusSlider.value * 0.5f;
        CreateShape();
        UpdateMesh();
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
    }

    private void OnGUI()
    {
        txtScale += Input.mouseScrollDelta.y * 0.05f;
        gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_TxtScale", txtScale);
    }
}
