using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleSpriteToFillOrthographicCamera : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private float worldScreenHeight;
    private float worldScreenWidth;
    private Vector2 spriteSize;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        ResizeSpriteToCameraAspectRatio(spriteRenderer);
    }

    // this will resize the scale of the sprite to camera aspect ratio
    private void ResizeSpriteToCameraAspectRatio(SpriteRenderer sprite)
    {

        // ortho is half the size of screen height
        worldScreenHeight = Camera.main.orthographicSize * 2;
        worldScreenWidth = Camera.main.aspect * worldScreenHeight;
        spriteSize = sprite.sprite.bounds.size;

        // current scale of background is updated to the scale from screen width/height
        transform.localScale = new Vector3(worldScreenWidth / spriteSize.x, worldScreenHeight / spriteSize.y, 10);

    }
}
