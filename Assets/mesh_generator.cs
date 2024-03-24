using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    Mesh mesh;

    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        
        CreateShape();
        UpdateMesh();

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


        for (int i = 0; i < segments; i++)
        {
            int a = i * 3;
            int b = (a + 1) % (segments * 3); //this mod must go on perface mode
            int c = (a + 2) % (segments * 3);
            int d = (a + 3) % (segments * 3);
            int e = (a + 4) % (segments * 3);
            int f = (a + 5) % (segments * 3);

            //funnel
            indices.Add(a); indices.Add(d); indices.Add(b);
            indices.Add(d); indices.Add(e); indices.Add(b);
            //tube
            indices.Add(b); indices.Add(e); indices.Add(c);
            indices.Add(e); indices.Add(f); indices.Add(c);
        }
    }
   void CreateShape()
    {
        vertices.Clear();
        indices.Clear();
        uvs.Clear();
 
        ///Funnel
        createFunnel();

    }

    /*void CreateShape()
    {
        vertices.Clear();
        indices.Clear();
        List<Vector3> topV = new List<Vector3>();
        List<Vector3> botV = new List<Vector3>();


        //BOT
        for (int i = 0; i < botSegments; i++)
        {
            float segment = (float)i / botSegments;
            float r = segment * PI * 2.0f;
            float x = Mathf.Cos(r) * botRadius;
            float z = Mathf.Sin(r) * botRadius;
            botV.Add(new Vector3(x, 0f, z));
        }

        //TOP
        for (int i = 0; i < topSegments; i++)
        {
            float segment = (float)i / topSegments;
            float r = segment * PI * 2.0f;
            float x = Mathf.Cos(r) * topRadius;
            float z = Mathf.Sin(r) * topRadius;
            topV.Add(new Vector3(x, height, z));
        }
      

        vertices.AddRange(botV);
        vertices.AddRange(topV);

        int maxSegment = Mathf.Max(botSegments, topSegments);
        int minSegment = Mathf.Min(botSegments, topSegments);

        float step = (float)minSegment / maxSegment;
        float counter = step;

        print("START HERE");
        for (int i = 0; i < maxSegment + 1; i++)
        {
            int scounter = 0;
            int offset = 0;
            //counter += step;
            if(counter >= 1)
            {
                print("adding f triangle");

                int b0 = i;
                int b1 = (b0 + 1) % (minSegment);
                int t0 = (botSegments + b0) % (minSegment + maxSegment);
                int t1 = botSegments + b1;

                //triangulate 051
                indices.Add(b0);
                indices.Add(t1);
                indices.Add(b1);
                print(b0);
                print(t1);
                print(b1);
                counter -= 1;
                offset++;


            }

            int it = 0;
            while(counter < 1)
            {
                it++;


                print("adding s triangle");
                int b0 = i + offset;
                int b1 = (b0 + 1) % (minSegment);
                int t0 = (botSegments + b0 + scounter) % (minSegment + maxSegment);
                int temp = (b0 + 1 + scounter) % (minSegment);
                int t1 = botSegments + temp;
                ///triangulate 450
                indices.Add(t0);
                indices.Add(t1);
                indices.Add(b0);
                print(t0);
                print(t1);
                print(b0);
                counter += step;
                scounter += 1;
                if(it > 100)
                {
                    print("broken whileLoop");
                    break;
                }
            }
                
            

        }


       //for (int i = 0; i < maxSegment; i++)
       //{
       //    int c = botSegments;
       //    int i0 = (int)((float)i / topSegments * i);
       //    int i1 = (i0 + 1) % (botSegments);
       //
       //    //int alt = (i + 1) % (botSegments + topSegments);
       //    int top0 = (c + i) % (botSegments + topSegments);
       //    int top1 = c + i1;
       //
       //
       //    indices.Add(i0);
       //    indices.Add(top0);
       //    indices.Add(i1);
       //    /////
       //    indices.Add(top0);
       //    indices.Add(top1);
       //    indices.Add(i1);
       //
       //
       //}

            //for (int i = 0; i < maxSegment; i++)
            //{
            //    int c = minSegment;
            //    int i0 = i;
            //    int i1 = (i0 + 1) % (minSegment);
            //    int top0 = (c + i0) % (minSegment + maxSegment);
            //    int top1 = c + i1;
            //    indices.Add(i0);
            //    indices.Add(top0);
            //    indices.Add(i1);
            //    /////
            //    indices.Add(top0);
            //    indices.Add(top1);
            //    indices.Add(i1);
            //
            //
            //}
    }
    */
    private void OnValidate()
    {
        UpdateMesh();
    }

    void updateFromSlider()
    {
        funnelHeight = fHeightSlider.value;
        funnelRadius = fRadiusSlider.value * 0.5f;
        tubeLength = tLengthSlider.value;
        tubeRadius = tRadiusSlider.value * 0.5f;

        UpdateMesh();
    }
    void UpdateMesh()
    {

        CreateShape();

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
