using System;
using UnityEngine;
using UnityEngine.UI;
public static class ScrollRectExtensions
{
    public static void ScrollToTop(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    public static void ScrollToBottom(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(-1, 0);
        Debug.Log("");
    }

    internal static void ScrollToBottom()
    {
        throw new NotImplementedException();
    }
}