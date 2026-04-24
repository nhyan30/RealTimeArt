using UnityEngine;

public class BoardClickHandler : MonoBehaviour
{
    [Header("References")]
    public BoardBorder border;
    public GameObject patternPrefab;

    [Header("Settings")]
    public float boardSize = 6f;
    public float zOffset = -0.05f;          // pattern sits slightly behind the border

    private Camera cam;
    private TessellationPattern currentPattern;

    void Start() => cam = Camera.main;

    void Update()
    {
        // Left-click  → create or evolve
        if (Input.GetMouseButtonDown(0)) OnLeftClick();

        // Right-click → reset & remove
        if (Input.GetMouseButtonDown(1)) OnRightClick();
    }

    // ────────────────────────────────────────────
    void OnLeftClick()
    {
        // ignore clicks outside the board
        Vector2 mp = cam.ScreenToWorldPoint(Input.mousePosition);
        float half = boardSize / 2f;
        if (Mathf.Abs(mp.x) > half || Mathf.Abs(mp.y) > half) return;

        // first click → instantiate ONE pattern, centred on the board
        if (currentPattern == null)
        {
            Vector3 pos = new Vector3(0f, 0f, zOffset);
            GameObject obj = Instantiate(patternPrefab, pos, Quaternion.identity);
            obj.transform.SetParent(transform, false);
            currentPattern = obj.GetComponent<TessellationPattern>();
        }

        // every click (including the first) → evolve one stage
        currentPattern.Evolve();
    }

    void OnRightClick()
    {
        if (currentPattern == null) return;
        currentPattern.Clear();
        Destroy(currentPattern.gameObject);
        currentPattern = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(boardSize, boardSize, 0.1f));
    }
}