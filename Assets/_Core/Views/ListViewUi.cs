using System;
using AOT.Views;
using UnityEngine;
using UnityEngine.UI;

namespace AOT.BaseUis
{
    public class ListViewUi<T> : PrefabsViewUi<T>
    {
        private readonly ScrollRect _scrollRect;

        public ScrollRect ScrollRect
        {
            get
            {
                if (_scrollRect == null)
                    throw new InvalidOperationException("如果要调用ScrollRect,请在构造的时候传入scrollrect控件");
                return _scrollRect;
            }
        }

        public ListViewUi(View prefab, ScrollRect scrollRect,IView v ,bool hideChildrenViews = true,bool display = true) : base(prefab, scrollRect.content, hideChildrenViews, display)
        {
        }

        public ListViewUi(IView v, string prefabName, string scrollRectName, bool hideChildrenViews = true,
            bool display = true) : this(
            v.Get<View>(prefabName),
            v.Get<ScrollRect>(scrollRectName), v, hideChildrenViews, display)
        {
        }

        public void SetVerticalScrollPosition(float value)
        {
            ScrollRect.verticalNormalizedPosition = value;
        }

        public void SetHorizontalScrollPosition(float value)
        {
            ScrollRect.horizontalNormalizedPosition = value;
        }

        public void ScrollRectSetSize(Vector2 size) => ((RectTransform)_scrollRect.transform).sizeDelta = size;

        public void ScrollRectSetSizeX(float x)
        {
            var rect = ((RectTransform)_scrollRect.transform);
            rect.sizeDelta = new Vector2(x, rect.sizeDelta.y);
        }

        public void ScrollRectSetSizeY(float y)
        {
            var rect = ((RectTransform)_scrollRect.transform);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, y);
        }
    }
}