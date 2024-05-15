using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ElementLayout: MonoBehaviour
{
    [SerializeField] private bool ignore;
    [SerializeField] [HideIf(nameof(Ignore))] private bool customSize;
    [SerializeField] [HideIf(nameof(Ignore)), ShowIf(nameof(CustomSize))] private Vector2 size;
    [SerializeField] [HideIf(nameof(Ignore))] private GroupLayout.LayoutPadding padding;

    public bool Ignore { get => ignore; set => ignore = value; }
    public Vector2 Size { get => size; }
    public GroupLayout.LayoutPadding Padding { get => padding; }
    public bool CustomSize { get => customSize; }
}
