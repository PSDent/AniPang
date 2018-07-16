using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTile : MonoBehaviour {

    const float ALPHA_DECRESE = 0.15f;
    const float TIME = 0.03f; // RefillTiming > TIMING > 7 * TIME

    SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

    IEnumerator FlareDestroy()
    {
        spriteRenderer.enabled = true;

        Color color = spriteRenderer.color;
        while (color.a > 0)
        {
            color.a -= ALPHA_DECRESE;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(TIME);
        }
        Destroy(transform.parent.gameObject);
    }
}
