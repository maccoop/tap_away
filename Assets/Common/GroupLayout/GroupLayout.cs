using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif
public class GroupLayout : UIBehaviour
{
    [System.Serializable]
    public class LayoutPadding
    {
        public float top, left, right, down;
    }

    [System.Serializable]
    public enum ContentAlignment
    {
        Left, Center, Right
    }

    [System.Serializable]
    public class ItemLayoutFormat
    {
        public int amountObject;
        public float height;

        public ItemLayoutFormat(float height, int length)
        {
            this.height = height;
            this.amountObject = length;
        }
    }

    public List<ItemLayoutFormat> itemFormat;
    public LayoutPadding padding;
    public Vector2 margin;
    public ContentAlignment alignment;
    private RectTransform[] transformChilds
    {
        get
        {
            var result = new List<RectTransform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                result.Add(transform.GetChild(i).GetComponent<RectTransform>());
            }
            return result.ToArray();
        }
    }

    public RectTransform RectTransform => GetComponent<RectTransform>();

    private void Update()
    {
        OnValueChanged();
    }

    public virtual void OnValueChanged()
    {
        var pivot = GetPivot(alignment);
        //var pivotParent = new Vector2(0, 1);
        //RectTransform.anchorMin = pivotParent;
        //RectTransform.anchorMax = pivotParent;
        //RectTransform.pivot = pivotParent;

        int index = 0;
        int indexFormat = 0;
        float currentPositionY = -padding.top;
        float height = padding.top;
        if (itemFormat.Count == 0)
        {
            return;
        }
        ItemLayoutFormat currentFormat = itemFormat[indexFormat];
        if (currentFormat.amountObject == 0 || currentFormat.height == 0)
            return;
        var transformChilds = this.transformChilds;
        for (int i = 0; i < transformChilds.Length; i++)
        {
            var child = transformChilds[i];
            var element = child.GetComponent<ElementLayout>();
            var hasElement = element != null;
            if (hasElement && element.Ignore || !child.gameObject.activeSelf)
            {
                continue;
            }
            child.anchorMin = pivot;
            child.anchorMax = pivot;
            child.pivot = new Vector2(0.5f, 0.5f);
            var size = GetSize(currentFormat.amountObject, currentFormat.height, element?.Padding);
            child.anchoredPosition =
                GetPosition(currentPositionY, index, size.x, element?.Padding, size.y, currentFormat.amountObject);
            if (!child.gameObject.activeSelf)
            {
                child.sizeDelta = Vector2.zero;
            }
            else if (hasElement && element.CustomSize)
            {
                child.sizeDelta = element.Size;
            }
            else
            {
                child.sizeDelta = size;
            }
            var cHeight = Mathf.Min(child.sizeDelta.y, currentFormat.height);
            height = Mathf.Abs(currentPositionY) + cHeight;
            index++;
            if (index >= currentFormat.amountObject)
            {
                indexFormat++;
                index = 0;
                currentPositionY -= cHeight + margin.y;
                if (indexFormat < itemFormat.Count)
                {
                    currentFormat = itemFormat[indexFormat];
                }
            }
        }
        height += padding.down;
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, height);
    }

    internal void RemoveLayout(int index, int count)
    {
        itemFormat.RemoveRange(index, count);
    }

    private Vector2 GetPosition(float currentPositionY, int index, float width, LayoutPadding addPadding, float height, int amountElement = 1)
    {
        var totalWidth = RectTransform.sizeDelta.x;
        var cx = padding.left + index * width + index * margin.x + width / 2;
        currentPositionY -= height / 2;
        if (addPadding != null)
        {
            cx += addPadding.left;
            currentPositionY -= addPadding.top;
        }
        return new Vector2(cx, currentPositionY);
    }

    private Vector2 GetSize(int amountItem, float height, LayoutPadding addPadding = null)
    {
        //float parrentSize = Mathf.Min(RectTransform.sizeDelta.x, Screen.width);
        float parrentSize = RectTransform.rect.size.x;
        var width = (parrentSize - padding.left - padding.right) / amountItem;
        width -= ((amountItem - 1) * margin.x) / amountItem;
        if (addPadding != null)
        {
            width -= addPadding.left + addPadding.right;
            height -= addPadding.top + addPadding.down;
        }
        return new Vector2(width, height);
    }

    public void AddLayoutFormat(int amount, float height)
    {
        if (height == 0 || amount == 0)
            return;
        AddLayoutFormat(new ItemLayoutFormat(height, amount), null);
    }

    public void AddLayoutFormat(int amount, float height, int index)
    {
        if (height == 0 || amount == 0)
            return;
        AddLayoutFormat(new ItemLayoutFormat(height, amount), index);
    }

    public void AddLayoutFormat(ItemLayoutFormat layout, int? index)
    {
        if (layout == null)
            return;
        if (index.HasValue)
            itemFormat.Insert(index.Value, layout);
        else itemFormat.Add(layout);
    }

    public Vector2 GetPivot(ContentAlignment layout)
    {
        return new Vector2(0, 1);
        switch (layout)
        {
            case ContentAlignment.Center:
                return new Vector2(0.5f, 1);
            case ContentAlignment.Left:
                return new Vector2(0, 1);
            case ContentAlignment.Right:
                return new Vector2(1, 1);
        }
    }
}
