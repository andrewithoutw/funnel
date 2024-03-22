using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class mesh_generator : MonoBehaviour
{
    const float PI = 3.1415f;
    public int segments = 4;
    public int topSegments = 12;
    public int botSegments = 16;
    public int height = 1;
    public int topRadius = 1;
    public int botRadius = 3;

    Mesh mesh;

    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
        
    }

   /* void CreateShapeDep()
    {
        vertices.Clear();
        indices.Clear();  
        List<Vector3> topV = new List<Vector3>();
        List<Vector3> botV = new List<Vector3>();

        Debug.Log("1Updating mesh with height: " + height);
        for (int i = 0; i < segments; i++)
        {
            float segment = (float)i / segments;
            float r = segment * PI * 2.0f;
            float x = Mathf.Cos(r) * radius;
            float z = Mathf.Sin(r) * radius;
            vertices.Add(new Vector3(x, 0f, z));
            vertices.Add(new Vector3(x, height, z));
        }

        for (int i = 0;i < segments; i++)
        {
             int ii = i * 2;
             int jj = (ii + 2) % (segments * 2);
             int kk = (ii + 3) % (segments * 2);
             int ll = ii + 1;
             indices.Add(ii);
             indices.Add(jj);
            // indices.Add(kk);
             indices.Add(ll);
            /////
            indices.Add(jj);
            indices.Add(kk);
            indices.Add(ll);


        }


        // vertices = new Vector3[]
        // {
        //     new Vector3 (0.0f, 0.0f, 0.0f),
        //     new Vector3 (0.0f, 0.0f, 1.0f),
        //     new Vector3 (1.0f, 0.0f, 0.0f)
        // };

       // triangles = new int[]
       // {
       //     0, 1, 2
       //
       // };
    }
   */
    void CreateShape()
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
        
        for (int i = 0; i < maxSegment + 1; i++)
        {
            int scounter = 0;
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

                counter -= 1;
  
            }

            int it = 0;
            while(counter < 1)
            {
                it++;


                print("adding s triangle");
                int b0 = i;
                int b1 = (b0 + 1) % (minSegment);
                int t0 = (botSegments + b0 + scounter) % (minSegment + maxSegment);
                int temp = (b0 + 1 + scounter) % (minSegment);
                int t1 = botSegments + temp;
                ///triangulate 450
                indices.Add(t0);
                indices.Add(t1);
                indices.Add(b0);
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
    private void OnValidate()
    {
        Debug.Log("0Updating mesh with height: " + height);
        UpdateMesh();
    }

    void UpdateMesh()
    {
        CreateShape();

       // foreach (var x in indices)
       // {
       //     print(x);
       // }
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();

        //GetComponent<MeshFilter>().mesh = mesh;


        print(vertices[1]);
       
       //foreach (var x in mesh.triangles)
       //{
       //    print(x);
       //}
    }
    // Update is called once per frame
    void Update()
    {
       //print(height);
       //print(radius);
       
    }
}
