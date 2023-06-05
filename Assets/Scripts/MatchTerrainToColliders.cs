using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MatchTerrainToColliders : MonoBehaviour
{
	[Tooltip(
		"Assign Terrain here if you like, otherwise we search for one.")]
	public Terrain terrain;

	[Tooltip(
		"Default is to cast from below. This will cast from above and bring the terrain to match the TOP of our collider.")]
	public bool CastFromAbove;

	[Header( "Related to smoothing around the edges.")]

	[Tooltip(
		"Size of gaussian filter applied to change array. Set to zero for none")]
	public int PerimeterRampDistance;

	[Tooltip(
		"Use Perimeter Ramp Curve in lieu of direct gaussian smooth.")]
	public bool ApplyPerimeterRampCurve;

	[Tooltip(
		"Optional shaped ramp around perimeter.")]
	public AnimationCurve PerimeterRampCurve;

	[Header("Misc/Editor")]

	[Tooltip(
		"Enable this if you want undo. It is SUPER-dog slow though, so I would leave it OFF.")]
	public bool EnableEditorUndo;

    private void Start()
    {
		terrain = GameObject.FindObjectOfType<Terrain>();

		BringTerrainToUndersideOfCollider();
	}

    void GeneratePerimeterHeightRampAndFlange(float[,] heightMap, float[,] blendStencil, int distance)
	{
		int w = blendStencil.GetLength(0);
		int h = blendStencil.GetLength(1);

		float[][,] stencilPile = new float[distance + 1][,];

		float[,] extendedHeightmap = new float[w, h];

		int[] neighborXYPairs = new int[] {
			0, 1,
			1, 0,
			0, -1,
			-1, 0,
			1,1,
			-1,1,
			1,-1,
			-1,-1,
		};

		int neighborCount = 4;

		float[,] source = blendStencil;

		for (int n = 0; n <= distance; n++)
		{
			stencilPile[n] = source;

			float[,] expanded = new float[w, h];
			for (int j = 0; j < h; j++)
			{
				for (int i = 0; i < w; i++)
				{
					expanded[i, j] = source[i, j];
				}
			}

			if (n == distance)
			{
				break;
			}

			for (int j = 0; j < h; j++)
			{
				for (int i = 0; i < w; i++)
				{
					if (source[i, j] == 0)
					{
						int count = 0;

						float height = 0.0f;

						for (int neighbor = 0; neighbor < neighborCount; neighbor++)
						{
							int x = i + neighborXYPairs[neighbor * 2 + 0];
							int y = j + neighborXYPairs[neighbor * 2 + 1];
							if ((x >= 0) && (x < w) && (y >= 0) && (y < h))
							{
								if (source[x, y] != 0)
								{
									height += heightMap[x, y];
									count++;
								}
							}
						}

						if (count > 0)
						{
							expanded[i, j] = 1.0f;

							extendedHeightmap[i, j] = height / count;
						}
					}
				}
			}

			for (int j = 0; j < h; j++)
			{
				for (int i = 0; i < w; i++)
				{
					var height = extendedHeightmap[i, j];

					if (height > 0)
					{
						heightMap[i,j] = height;
					}

					extendedHeightmap[i, j] = 0;
				}
			}

			source = expanded;
		}
		for (int j = 0; j < h; j++)
		{
			for (int i = 0; i < w; i++)
			{
				float total = 0;
				for (int n = 0; n <= distance; n++)
				{
					total += stencilPile[n][i, j];
				}

				total /= (distance + 1);

				blendStencil[i, j] = total;
			}
		}

	}

	public void BringTerrainToUndersideOfCollider()
	{
		var Colliders = GetComponentsInChildren<Collider>();

		if (Colliders == null || Colliders.Length == 0)
		{
			Debug.LogError("We must have at least one collider on ourselves or below us in the hierarchy. " +
				"We will cast to it and match terrain to that contour.");
			return;
		}

		if (!terrain)
		{
			terrain = FindObjectOfType<Terrain>();
			if (!terrain)
			{
				Debug.LogError("couldn't find a terrain");
				return;
			}
			Debug.LogWarning(
				"Terrain not supplied; finding it myself. I found and assigned " + terrain.name +
				", but I didn't do anything yet... click again to actually DO the modification.");
			return;
		}

		TerrainData terData = terrain.terrainData;
		int Tw = terData.heightmapResolution;
		int Th = terData.heightmapResolution;
		var heightMapOriginal = terData.GetHeights(0, 0, Tw, Th);

		var heightMapCreated = new float[heightMapOriginal.GetLength(0), heightMapOriginal.GetLength(1)];

		var heightAlpha = new float[heightMapOriginal.GetLength(0), heightMapOriginal.GetLength(1)];

#if UNITY_EDITOR
		if (EnableEditorUndo)
		{
			Undo.RecordObject(terData, "ModifyTerrain");
		}
#endif

		for (int Tz = 0; Tz < Th; Tz++)
		{
			for (int Tx = 0; Tx < Tw; Tx++)
			{
				var pos = terrain.transform.position +
					new Vector3((Tx * terData.size.x) / (Tw - 1),
					-10,
					(Tz * terData.size.z) / (Th - 1));

				Ray ray = new Ray(pos, Vector3.up);

				if (CastFromAbove)
				{
					pos.y = transform.position.y + terData.size.y + 10;
					ray = new Ray(pos, Vector3.down);
				}

				bool didHit = false;
				float yHit = 0;

				foreach (var ourCollider in Colliders)
				{
					RaycastHit hit;
					if (ourCollider.Raycast(ray, out hit, 1000))
					{
						if (!didHit)
						{
							yHit = hit.point.y;
						}

						didHit = true;

						if (CastFromAbove)
						{
							if (hit.point.y > yHit)
							{
								yHit = hit.point.y;
							}
						}
						else
						{
							if (hit.point.y < yHit)
							{
								yHit = hit.point.y;
							}
						}

					}

					if (didHit)
					{
						var height = yHit / terData.size.y;

						heightMapCreated[Tz, Tx] = height;
						heightAlpha[Tz, Tx] = 1.0f;
					}
				}
			}
		}

		if (PerimeterRampDistance > 0)
		{
			GeneratePerimeterHeightRampAndFlange(
				heightMap: heightMapCreated,
				blendStencil: heightAlpha,
				distance: PerimeterRampDistance);
		}
		for (int Tz = 0; Tz < Th; Tz++)
		{
			for (int Tx = 0; Tx < Tw; Tx++)
			{
				float fraction = heightAlpha[Tz, Tx];

				if (ApplyPerimeterRampCurve)
				{
					fraction = PerimeterRampCurve.Evaluate( fraction);
				}

				heightMapOriginal[Tz, Tx] = Mathf.Lerp(
					heightMapOriginal[Tz, Tx],
					heightMapCreated[Tz, Tx],
					fraction);
			}
		}

		terData.SetHeights(0, 0, heightMapOriginal);
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(MatchTerrainToColliders))]
	public class MatchTerrainToCollidersEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			MatchTerrainToColliders item = (MatchTerrainToColliders)target;

			DrawDefaultInspector();

			EditorGUILayout.BeginVertical();

			var buttonLabel = "Bring Terrain To Underside Of Collider";
			if (item.CastFromAbove)
			{
				buttonLabel = "Bring Terrain To Topside Of Collider";
			}

			if (GUILayout.Button(buttonLabel))
			{
				item.BringTerrainToUndersideOfCollider();
			}

			EditorGUILayout.EndVertical();
		}
#endif
	}

	void WritePNG( float[,] array, string filename, bool normalize = false)
	{
		int w = array.GetLength(0);
		int h = array.GetLength(1);

		Texture2D texture = new Texture2D( w, h);

		Color[] colors = new Color[ w * h];

		{
			float min = 0;
			float max = 1;

			if (normalize)
			{
				min = 1;
				max = 0;
				for (int j = 0; j < h; j++)
				{
					for (int i = 0; i < w; i++)
					{
						float x = array[i,j];
						if (x < min) min = x;
						if (x > max) max = x;
					}
				}

				if (max <= min)
				{
					min = 0;
					max = 1;
				}
			}

			int n = 0;
			for (int j = 0; j < h; j++)
			{
				for (int i = 0; i < w; i++)
				{
					float x = array[i,j];
					x = x - min;
					x /= (max - min);
					colors[n] = new Color( x,x,x);
					n++;
				}
			}
		}

		texture.SetPixels( colors);
		texture.Apply();

		var bytes = texture.EncodeToPNG();

		DestroyImmediate(texture);

		filename = filename + ".png";

		System.IO.File.WriteAllBytes( filename, bytes);
	}

	void Debug_Microtest()
	{
		float[,] heights = new float[3,3] {
			{ 0.0f, 0.0f, 0.0f, },
			{ 0.0f, 0.5f, 0.0f, },
			{ 0.0f, 0.0f, 0.0f, }
		};
		float[,] stencil = new float[3,3] {
			{ 0.0f, 0.0f, 0.0f, },
			{ 0.0f, 1.0f, 0.0f, },
			{ 0.0f, 0.0f, 0.0f, }
		};

		{
			WritePNG( heights, "height-0", true);
			WritePNG( stencil, "alpha-0", true);

			GeneratePerimeterHeightRampAndFlange(
				heightMap: heights,
				blendStencil: stencil,
				distance: PerimeterRampDistance);

			WritePNG( heights, "height-1", true);
			WritePNG( stencil, "alpha-1", true);
		}
	}
}
