using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class AbstractData : IData
{
    [SerializeField, FoldoutGroup("Data")] private string id;
    [SerializeField, FoldoutGroup("Data")] private string name;
    [SerializeField, FoldoutGroup("Data")] private bool active;
    [SerializeField, FoldoutGroup("Data")] private string description;
    [SerializeField, FoldoutGroup("Data")] private Sprite thumbnail;

    public AbstractData()
    {

    }

    public AbstractData(IData data)
    {
        this.id = data.Id;
        this.name = data.Name;
        this.description = data.Description;
        this.thumbnail = data.Thumbnail;
    }

    public AbstractData(AbstractData data)
    {
        if (data == null)
            return;
        this.id = data.Id;
        this.name = data.Name;
        this.description = data.Description;
        this.thumbnail = data.Thumbnail;
        this.active = data.active;
    }

    protected AbstractData(string id, string name, bool active, string description, Sprite thumbnail)
    {
        this.id = id;
        this.name = name;
        this.active = active;
        this.description = description;
        this.thumbnail = thumbnail;
    }

    public virtual string Id => id;
    public virtual string Name => name;
    public virtual string Description => description;
    public virtual Sprite Thumbnail => thumbnail;
    public virtual bool Active => active;

    public void SetID(string id)
    {
        this.id = id;
    }
    public void SetName(string name)
    {
        this.name = name;
    }
    public void SetDescription(string des)
    {
        this.description = des;
    }
    public void SetThumbnail(Sprite thumb)
    {
        this.thumbnail = thumb;
    }
}
