using UnityEngine;

public class GenevauxTerrainTester : MonoBehaviour
{
    public Texture2D riverSlopeMap;
	public bool showContour = true;
	public bool showGraph = true;
	public bool showVoronoi = true;

	Polygon contour;
	Texture2D tex;
	GenevauxTerrain terrain;

    public void Start() {

		tex = new Texture2D(riverSlopeMap.width, riverSlopeMap.height);
		float[,] slopeMap = new float[riverSlopeMap.width, riverSlopeMap.height];

		for (int y = 0; y < tex.height; y++)
			for (int x = 0; x < tex.height; x++)
			{
				slopeMap[x, y] = riverSlopeMap.GetPixel(x, y).r;
				tex.SetPixel(x, y, riverSlopeMap.GetPixel(x, y));
			}
		tex.Apply();

		contour = new Polygon(GenevauxTerrain.ExtractContour(slopeMap, 0.025f));

		//for (int i = 0; i < contour.Length; i++)
		//	Debug.Log(contour[i].x + ", " + contour[i].y);

		terrain = new GenevauxTerrain();
		terrain.Generate(slopeMap, slopeMap);

		tex.Apply();

		if (img != null)
		{
			img.texture = tex;
		}
    }
}