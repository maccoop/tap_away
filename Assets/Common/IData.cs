using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IData
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public Sprite Thumbnail { get; }
}
