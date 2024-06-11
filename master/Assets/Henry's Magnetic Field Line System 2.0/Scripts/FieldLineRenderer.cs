using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Analytics;

public class FieldLineRenderer : MonoBehaviour
{
    [Header("Visualization Quality Settings (*Do not change in run mode!*)")]
    [Range(0, 250)] public int fieldSourceCount = 60;
    [Range(0, 500)] public int maxLineSegments = 500;
    [Range(0, 1)] public float sphereRadius = 0.015f;

    [Header("Compute Shader Settings")]
    [Range(0, 0.1f)] public float stepSize = 0.025f;
    [Range(100, 10000)] public float maxFieldThreshold = 4575;
    [Range(0, 10)] public float minFieldThreshold = 0.01f;
    [Range(0, 20)] public float maxDistance = 10;

    [Header("Render Shader Settings")]
    [Range(0.001f, 0.01f)] public float lineThickness = 0.005f;
    [Range(0, 2)] public int fieldLerpType = 0;
    [Range(0, 10)] public float fieldLerpScale = 4.17f;
    public Color lerpColor1 = Color.blue;
    public Color lerpColor2 = Color.red;

    [Header("Magnet List")]
    public List<GameObject> magnets;

    [Header("References")]
    public Mesh lineSegmentMesh;
    public Material lineSegmentMaterial;
    public ComputeShader fieldLineComputeShader;

    private int kernelID;

    private ComputeBuffer chargeBuffer;
    private ComputeBuffer fieldSourceBuffer;
    private ComputeBuffer lineSegmentBuffer;
    private ComputeBuffer lineDirectionBuffer;

    private Vector4[] chargeArray;
    private Vector4[] fieldSourceArray;

    private Bounds bounds;

    private const int threadCount = 1024;
    private float goldenRatio = (1 + Mathf.Sqrt(5)) / 2f;

    private void Update()
    {
        if (magnets.Count == 0) { return; }

        //Extracting magnetic "charges" from magnet gameObjects -- feel free to replace this to fit your own data structures, all the program requires is an array of magnetic "charges"
        chargeArray = new Vector4[magnets.Count * 2];
        int chargeIndex = 0;

        foreach (GameObject magnet in magnets)
        {
            foreach (Transform pole in magnet.transform)
            {
                if (pole.gameObject.tag == "NegativePole")
                {
                    //xyz components -> charge position; w component -> charge
                    chargeArray[chargeIndex] = new Vector4(pole.position.x, pole.position.y, pole.position.z, -1);
                    ++chargeIndex;
                }
                else if (pole.gameObject.tag == "PositivePole")
                {
                    chargeArray[chargeIndex] = new Vector4(pole.position.x, pole.position.y, pole.position.z, 1);
                    ++chargeIndex;
                }
            }
        }
        chargeIndex = 0;

        //Creating a point array for field line sources around magentic "charges" using the Fibonacci lattice
        fieldSourceArray = new Vector4[chargeArray.Length * fieldSourceCount];

        foreach (Vector4 charge in chargeArray)
        {
            if (charge.w == 0) { continue; }

            for (int i = 0; i < fieldSourceCount; i++)
            {
                float theta = 2 * Mathf.PI * i / goldenRatio;
                float phi = Mathf.Acos(1 - 2 * (i + 0.5f) / fieldSourceCount);

                Vector3 fieldSourcePosition = new Vector3(charge.x, charge.y, charge.z) - sphereRadius * new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(phi));
                fieldSourceArray[i + fieldSourceCount * chargeIndex] = new Vector4(fieldSourcePosition.x, fieldSourcePosition.y, fieldSourcePosition.z, charge.w);
            }

            ++chargeIndex;
        }

        //Writing charge and field source data to their respective compute buffers
        chargeBuffer.SetData(chargeArray);
        fieldSourceBuffer.SetData(fieldSourceArray);

        //Linking the compute buffers to the compute shader
        fieldLineComputeShader.SetBuffer(kernelID, "_Charge", chargeBuffer);
        fieldLineComputeShader.SetBuffer(kernelID, "_FieldSource", fieldSourceBuffer);
        fieldLineComputeShader.SetBuffer(kernelID, "_LineSegment", lineSegmentBuffer);
        fieldLineComputeShader.SetBuffer(kernelID, "_LineDirection", lineDirectionBuffer);

        //Setting compute shader parameters
        fieldLineComputeShader.SetInt("_NumberOfCharges", chargeArray.Length);
        fieldLineComputeShader.SetInt("_MaxLineSegments", maxLineSegments);

        fieldLineComputeShader.SetFloat("_StepSize", stepSize);
        fieldLineComputeShader.SetFloat("_MaxFieldThreshold", maxFieldThreshold);
        fieldLineComputeShader.SetFloat("_MinFieldThreshold", minFieldThreshold);
        fieldLineComputeShader.SetFloat("_MaxDistance", maxDistance);

        //Dispatching the compute shader to generate line segment positions and directions
        fieldLineComputeShader.Dispatch(kernelID, (int)Mathf.Ceil(fieldSourceArray.Length / (float)threadCount), 1, 1);

        //Linking the compute buffers to the render shader
        lineSegmentMaterial.SetBuffer("_LineSegments", lineSegmentBuffer);
        lineSegmentMaterial.SetBuffer("_LineDirections", lineDirectionBuffer);

        //Setting render shader parameters 
        lineSegmentMaterial.SetFloat("_LineLength", stepSize);
        lineSegmentMaterial.SetFloat("_LineThickness", lineThickness);
        lineSegmentMaterial.SetFloat("_FieldLerpScale", fieldLerpScale);
        lineSegmentMaterial.SetInt("_FieldLerpType", fieldLerpType);
        lineSegmentMaterial.SetColor("_LerpColor1", lerpColor1);
        lineSegmentMaterial.SetColor("_LerpColor2", lerpColor2);

        //Render the line segments using instanced indirect rendering
        Graphics.DrawMeshInstancedProcedural(lineSegmentMesh, 0, lineSegmentMaterial, bounds, lineSegmentBuffer.count);
    }

    private void OnEnable()
    {
        //Extract the compute shader kernel id and set the culling bounds for the instanced indirect rendering
        kernelID = fieldLineComputeShader.FindKernel("CSMain");
        bounds = new Bounds(Vector3.zero, Vector3.one * 10);

        //Generate the compute buffers.  Note that these are fixed size at run-time
        chargeBuffer = new ComputeBuffer(magnets.Count * 2, sizeof(float) * 4);
        fieldSourceBuffer = new ComputeBuffer(chargeBuffer.count * fieldSourceCount, sizeof(float) * 4);
        lineSegmentBuffer = new ComputeBuffer(fieldSourceBuffer.count * maxLineSegments, sizeof(float) * 4);
        lineDirectionBuffer = new ComputeBuffer(lineSegmentBuffer.count, sizeof(float) * 4);
    }

    private void OnDisable()
    {
        //Release the allocated compute buffers
        if (chargeBuffer != null) { chargeBuffer.Release(); chargeBuffer = null; }
        if (fieldSourceBuffer != null) { fieldSourceBuffer.Release(); fieldSourceBuffer = null; }
        if (lineSegmentBuffer != null) { lineSegmentBuffer.Release(); lineSegmentBuffer = null; }
        if (lineDirectionBuffer != null) { lineDirectionBuffer.Release(); lineDirectionBuffer = null; }
    }
}