using UnityEngine;

public class HexCell : MonoBehaviour {
    public HexCoordinates coordinates;

    Color color;
    public Color Color {
        get {
            return color;
        }
        set {
            if (color == value)
                return;

            color = value;
            Refresh();
        }
    }

    public HexGridChunk chunk;

    private int elevation = int.MinValue;
    public int Elevation {
        get {
            return elevation;
        }
        set {
            if(elevation == value)
                return;

            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;

            Refresh();
        }
    }

    public Vector3 Position {
        get {
            return transform.localPosition;
        }
    }

    [SerializeField]
    HexCell[] neighbours;

    public RectTransform uiRect;

    public HexCell GetNeighbour(HexDirection direction) {
        return neighbours[(int)direction];
    }

    public void SetNeighbour(HexDirection direction, HexCell cell) {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;
    }

    void Refresh() {
        if(chunk) {
            chunk.Refresh();

            for (int i = 0; i < neighbours.Length; i++) {
                HexCell neighbour = neighbours[i];
                if(neighbour != null && neighbour.chunk != chunk) {
                    neighbour.chunk.Refresh();
                }
            }
        }
    }

    public HexEdgeType GetEdgeType(HexDirection direction) {
        return HexMetrics.GetEdgeType(elevation, neighbours[(int)direction].elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell) {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }
}