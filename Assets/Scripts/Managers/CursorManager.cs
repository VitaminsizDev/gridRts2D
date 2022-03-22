using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _cursor;
    // Start is called before the first frame update
    void Start()
    {
        // Set cursor texture to _cursor.texture
        Cursor.SetCursor(_cursor, Vector2.zero, CursorMode.ForceSoftware);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
