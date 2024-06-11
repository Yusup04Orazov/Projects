using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BooleanMesh {
    public static Mesh Union(GameObject gameObject1, GameObject gameObject2) {
        Mesh gameObject1Mesh = gameObject1.GetComponent<MeshFilter>().sharedMesh;        
        Net3dBool.Solid gameObject1Solid = new Net3dBool.Solid(gameObject1Mesh.vertices, gameObject1Mesh.triangles);
        gameObject1Solid.Rotate(gameObject1.transform.rotation);
        gameObject1Solid.Scale(gameObject1.transform.localScale);
        gameObject1Solid.Translate(gameObject1.transform.position);

        Mesh gameObject2Mesh = gameObject2.GetComponent<MeshFilter>().sharedMesh;
        Net3dBool.Solid gameObject2Solid = new Net3dBool.Solid(gameObject2Mesh.vertices, gameObject2Mesh.triangles);
        gameObject2Solid.Rotate(gameObject2.transform.rotation);
        gameObject2Solid.Scale(gameObject2.transform.localScale);
        gameObject2Solid.Translate(gameObject2.transform.position);

        Net3dBool.Solid outputSolid = (new Net3dBool.BooleanModeller(gameObject1Solid, gameObject2Solid)).GetUnion();
        Mesh outputMesh = new Mesh();
        outputMesh.vertices = outputSolid.GetFVertices();
        outputMesh.triangles = outputSolid.GetIndices();
        outputMesh.RecalculateNormals();  
        outputMesh.RecalculateTangents();

        return outputMesh;
    }

    public static Mesh Difference(GameObject gameObject1, GameObject gameObject2) {
        Mesh gameObject1Mesh = gameObject1.GetComponent<MeshFilter>().sharedMesh;        
        Net3dBool.Solid gameObject1Solid = new Net3dBool.Solid(gameObject1Mesh.vertices, gameObject1Mesh.triangles);
        gameObject1Solid.Rotate(gameObject1.transform.rotation);
        gameObject1Solid.Scale(gameObject1.transform.localScale);
        gameObject1Solid.Translate(gameObject1.transform.position);

        Mesh gameObject2Mesh = gameObject2.GetComponent<MeshFilter>().sharedMesh;
        Net3dBool.Solid gameObject2Solid = new Net3dBool.Solid(gameObject2Mesh.vertices, gameObject2Mesh.triangles);
        gameObject2Solid.Rotate(gameObject2.transform.rotation);
        gameObject2Solid.Scale(gameObject2.transform.localScale);
        gameObject2Solid.Translate(gameObject2.transform.position);

        Net3dBool.Solid outputSolid = (new Net3dBool.BooleanModeller(gameObject1Solid, gameObject2Solid)).GetDifference();
        Mesh outputMesh = new Mesh();
        outputMesh.vertices = outputSolid.GetFVertices();
        outputMesh.triangles = outputSolid.GetIndices();
        outputMesh.RecalculateNormals();  
        outputMesh.RecalculateTangents();

        return outputMesh;
    }

    public static Mesh Intersection(GameObject gameObject1, GameObject gameObject2) {
        Mesh gameObject1Mesh = gameObject1.GetComponent<MeshFilter>().sharedMesh;        
        Net3dBool.Solid gameObject1Solid = new Net3dBool.Solid(gameObject1Mesh.vertices, gameObject1Mesh.triangles);
        gameObject1Solid.Rotate(gameObject1.transform.rotation);
        gameObject1Solid.Scale(gameObject1.transform.localScale);
        gameObject1Solid.Translate(gameObject1.transform.position);

        Mesh gameObject2Mesh = gameObject2.GetComponent<MeshFilter>().sharedMesh;
        Net3dBool.Solid gameObject2Solid = new Net3dBool.Solid(gameObject2Mesh.vertices, gameObject2Mesh.triangles);
        gameObject2Solid.Rotate(gameObject2.transform.rotation);
        gameObject2Solid.Scale(gameObject2.transform.localScale);
        gameObject2Solid.Translate(gameObject2.transform.position);

        Net3dBool.Solid outputSolid = (new Net3dBool.BooleanModeller(gameObject1Solid, gameObject2Solid)).GetIntersection();
        Mesh outputMesh = new Mesh();
        outputMesh.vertices = outputSolid.GetFVertices();
        outputMesh.triangles = outputSolid.GetIndices();
        outputMesh.RecalculateNormals();  
        outputMesh.RecalculateTangents();

        return outputMesh;
    }
}
