﻿using UnityEngine;

public class Box : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetInfo(BoxInfo info)
    {
        _meshRenderer.material.color = info.Color;
    }
}