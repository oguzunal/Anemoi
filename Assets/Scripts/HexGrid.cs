﻿using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

    public int chunkCountX = 4, chunkCountZ = 3;

    int cellCountX, cellCountZ;

    public HexGridChunk chunkPrefab;

    public HexCell cellPrefab;

    HexCell[] cells;

    HexGridChunk[] chunks;

    public Text cellLabelPrefab;

    //Canvas gridCanvas;

    //HexMesh hexMesh;

    public Color defaultColor = Color.white;

    public Texture2D noiseSource;

    void Awake() {
        HexMetrics.noiseSource = noiseSource;

        //hexMesh = GetComponentInChildren<HexMesh>();
        //gridCanvas = GetComponentInChildren<Canvas>();

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
    }

    void CreateChunks() {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++) {
            for (int x = 0; x < chunkCountX; x++) {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells() {
        cells = new HexCell[cellCountX * cellCountZ];

        for (int z = 0, i = 0; z < cellCountZ; z++) {
            for (int x = 0; x < cellCountX; x++) {
                CreateCell(x, z, i++);
            }
        }
    }

    //void Start() {
    //    hexMesh.Triangulate(cells);
    //}

    void OnEnable() {
        HexMetrics.noiseSource = noiseSource;
    }

    void CreateCell(int x, int z, int i) {
        Vector3 position;

        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        //cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;

        if(x > 0) {
            cell.SetNeighbour(HexDirection.W, cells[i - 1]);
        }

        if(z > 0) {
            if((z & 1) == 0) {
                cell.SetNeighbour(HexDirection.SE, cells[i - cellCountX]);
                if(x > 0) {
                    cell.SetNeighbour(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else {
                cell.SetNeighbour(HexDirection.SW, cells[i - cellCountX]);
                if(x < cellCountX - 1) {
                    cell.SetNeighbour(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        
        Text label = Instantiate<Text>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();

        cell.uiRect = label.rectTransform;

        // Elevation perlin noise stuff
        float scale = 0.003f;
        float height = Mathf.PerlinNoise(position.x * scale, position.z * scale);

        cell.Elevation = (int)(height * 10) - 1;

        if (cell.Elevation <= 2) {
            cell.Color = Color.blue * height * 3f;
        }
        else if(cell.Elevation <= 3) {
            cell.Color = Color.yellow * height * 2f;
        }
        else if(cell.Elevation <= 5) {
            cell.Color = Color.green * height * 1.2f;
        } else {
            cell.Color = Color.white * height * .95f;
        }

        //cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell) {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;

        HexGridChunk chunk = chunks[chunkX +  chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;

        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates) {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
            return null;

        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
            return null;

        return cells[x + z * cellCountX];
    }

    public void ShowUI(bool visible) {
        for(int i = 0; i < chunks.Length; i++) {
            chunks[i].ShowUI(visible);
        }
    }
}