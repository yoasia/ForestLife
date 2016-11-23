#pragma strict

function Start () {
    // Get mesh info from attached mesh
    var mesh = GetComponent(MeshFilter).mesh;
    var vertices = mesh.vertices;
    var uv = mesh.uv;
    var triangles = mesh.triangles;
       
    // Set up new arrays to use with rebuilt mesh
    var newVertices = new Vector3[triangles.Length];
    var newUV = new Vector2[triangles.Length];
     
    // Rebuild mesh so that every triangle has unique vertices
    for (var i = 0; i < triangles.Length; i++) {
        newVertices[i] = vertices[triangles[i]];
        newUV[i] = uv[triangles[i]];
        triangles[i] = i;
    }
       
    // Assign new mesh and rebuild normals
    mesh.vertices = newVertices;
    mesh.uv = newUV;
    mesh.triangles = triangles;
    mesh.RecalculateNormals();
}


function Update () {

}