using UnityEngine;
using System.Collections;

public class EdgeBlocker : MonoBehaviour
{
    float mapSize;
    // Use this for initialization
    void Start()
    {
        mapSize = GameEngine.instance.mapSize - 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 fixedPos = transform.position;
        bool changed = false;
        if(fixedPos.x < 0f)
        {
            fixedPos.x = 0f; changed = true;
        } else if(fixedPos.x > mapSize)
        {
            fixedPos.x = mapSize; changed = true;
        }
        if(fixedPos.y < 0f)
        {
            fixedPos.y = 0f; changed = true;
        } else if(fixedPos.y > mapSize)
        {
            fixedPos.y = mapSize; changed = true;
        }

        if (changed)
            transform.position = fixedPos;
    }
}
