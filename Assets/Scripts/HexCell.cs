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

            if(hasOutgoingRiver && elevation < GetNeighbour(outgoingRiver).elevation) {
                RemoveOutgoingRiver();
            }

            if(hasIncomingRiver && elevation > GetNeighbour(incomingRiver).elevation) {
                RemoveOutgoingRiver();
            }

            for (int i = 0; i < roads.Length; i++) {
                if(roads[i] && GetElevationDifference((HexDirection)i) > 1) {
                    SetRoad(i, false);
                }
            }

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

    bool hasIncomingRiver, hasOutgoingRiver;
    HexDirection incomingRiver, outgoingRiver;

    public bool HasIncomingRiver {
        get {
            return hasIncomingRiver;
        }
    }

    public bool HasOutgoingRiver {
        get {
            return hasOutgoingRiver;
        }
    }

    public HexDirection IncomingRiver {
        get {
            return incomingRiver;
        }
    }

    public HexDirection OutgoingRiver {
        get {
            return outgoingRiver;
        }
    }

    public bool HasRiver {
        get {
            return hasIncomingRiver || hasOutgoingRiver;
        }
    }

    public bool HasRiverBeginOrEnd {
        get {
            return hasIncomingRiver != hasOutgoingRiver;
        }
    }

    public float StreamBedY {
        get {
            return (elevation + HexMetrics.streamBedElevationOffset) * HexMetrics.elevationStep;
        }
    }

    public float RiverSurfaceY {
        get {
            return (elevation + HexMetrics.riverSurfaceElevationOffset) * HexMetrics.elevationStep;
        }
    }

    [SerializeField]
    bool[] roads;

    public bool HasRoad {
        get {
            for (int i = 0; i < roads.Length; i++) {
                if (roads[i])
                    return true;
            }

            return false;
        }
    }

    public bool HasRoadThroughEdge(HexDirection direction) {
        return roads[(int)direction];
    }

    public void AddRoad(HexDirection direction) {
        if (!roads[(int)direction] && !HasRiverThroughEdge(direction) && GetElevationDifference(direction) <= 1) {
            SetRoad((int)direction, true);
        }
    }

    public void RemoveRoads() {
        for (int i = 0; i < neighbours.Length; i++) {
            if (roads[i]) {
                SetRoad(i, false);
            }
        }
    }

    private void SetRoad(int index, bool state) {
        roads[index] = state;
        neighbours[index].roads[(int)((HexDirection)index).Opposite()] = state;
        neighbours[index].Refresh(true);
        Refresh(true);
    }

    public int GetElevationDifference(HexDirection direction) {
        int difference = elevation - GetNeighbour(direction).elevation;
        return difference >= 0 ? difference : -difference;
    }

    public bool HasRiverThroughEdge(HexDirection direction) {
        return hasIncomingRiver && incomingRiver == direction || hasOutgoingRiver && outgoingRiver == direction;
    }

    public void RemoveOutgoingRiver() {
        if (!hasOutgoingRiver)
            return;

        hasOutgoingRiver = false;
        Refresh(true);

        HexCell neighbour = GetNeighbour(outgoingRiver);
        neighbour.hasIncomingRiver = false;
        neighbour.Refresh(true);
    }

    public void RemoveIncomingRiver() {
        if (!hasIncomingRiver)
            return;

        hasIncomingRiver = false;
        Refresh(true);

        HexCell neighbour = GetNeighbour(incomingRiver);
        neighbour.hasOutgoingRiver = false;
        neighbour.Refresh(true);
    }

    public void RemoveRiver() {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction) {
        if(hasOutgoingRiver && outgoingRiver == direction) {
            return;
        }

        HexCell neighbour = GetNeighbour(direction);
        if(!neighbour || elevation < neighbour.elevation) {
            return;
        }

        RemoveOutgoingRiver();
        if(hasIncomingRiver && incomingRiver == direction) {
            RemoveIncomingRiver();
        }

        hasOutgoingRiver = true;
        outgoingRiver = direction;

        neighbour.RemoveIncomingRiver();
        neighbour.hasIncomingRiver = true;
        neighbour.incomingRiver = direction.Opposite();

        SetRoad((int)direction, false);
    }

    public RectTransform uiRect;

    public HexCell GetNeighbour(HexDirection direction) {
        return neighbours[(int)direction];
    }

    public void SetNeighbour(HexDirection direction, HexCell cell) {
        neighbours[(int)direction] = cell;
        cell.neighbours[(int)direction.Opposite()] = this;
    }

    void Refresh(bool onlySelf = false) {
        if(chunk) {
            chunk.Refresh();

            if (onlySelf)
                return;

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