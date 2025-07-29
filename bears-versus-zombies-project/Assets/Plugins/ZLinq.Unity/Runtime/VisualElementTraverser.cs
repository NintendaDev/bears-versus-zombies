#if ZLINQ_UNITY_UIELEMENTS_SUPPORT

#nullable enable

using System.Runtime.InteropServices;
using UnityEngine.UIElements;
using ZLinq.Traversables;

namespace ZLinq
{
    public static class VisualTraverserExtensions
    {
        public static VisualElementTraverser AsTraverser(this VisualElement origin) => new(origin);

        // type inference helper

        public static ValueEnumerable<Children<VisualElementTraverser, VisualElement>, VisualElement> Children(this VisualElementTraverser traverser) => traverser.Children<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<Children<VisualElementTraverser, VisualElement>, VisualElement> ChildrenAndSelf(this VisualElementTraverser traverser) => traverser.ChildrenAndSelf<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<Descendants<VisualElementTraverser, VisualElement>, VisualElement> Descendants(this VisualElementTraverser traverser) => traverser.Descendants<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<Descendants<VisualElementTraverser, VisualElement>, VisualElement> DescendantsAndSelf(this VisualElementTraverser traverser) => traverser.DescendantsAndSelf<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<Ancestors<VisualElementTraverser, VisualElement>, VisualElement> Ancestors(this VisualElementTraverser traverser) => traverser.Ancestors<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<Ancestors<VisualElementTraverser, VisualElement>, VisualElement> AncestorsAndSelf(this VisualElementTraverser traverser) => traverser.AncestorsAndSelf<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<BeforeSelf<VisualElementTraverser, VisualElement>, VisualElement> BeforeSelf(this VisualElementTraverser traverser) => traverser.BeforeSelf<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<BeforeSelf<VisualElementTraverser, VisualElement>, VisualElement> BeforeSelfAndSelf(this VisualElementTraverser traverser) => traverser.BeforeSelfAndSelf<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<AfterSelf<VisualElementTraverser, VisualElement>, VisualElement> AfterSelf(this VisualElementTraverser traverser) => traverser.AfterSelf<VisualElementTraverser, VisualElement>();
        public static ValueEnumerable<AfterSelf<VisualElementTraverser, VisualElement>, VisualElement> AfterSelfAndSelf(this VisualElementTraverser traverser) => traverser.AfterSelfAndSelf<VisualElementTraverser, VisualElement>();

        // direct shortcut

        public static ValueEnumerable<Children<VisualElementTraverser, VisualElement>, VisualElement> Children(this VisualElement origin) => origin.AsTraverser().Children();
        public static ValueEnumerable<Children<VisualElementTraverser, VisualElement>, VisualElement> ChildrenAndSelf(this VisualElement origin) => origin.AsTraverser().ChildrenAndSelf();
        public static ValueEnumerable<Descendants<VisualElementTraverser, VisualElement>, VisualElement> Descendants(this VisualElement origin) => origin.AsTraverser().Descendants();
        public static ValueEnumerable<Descendants<VisualElementTraverser, VisualElement>, VisualElement> DescendantsAndSelf(this VisualElement origin) => origin.AsTraverser().DescendantsAndSelf();
        public static ValueEnumerable<Ancestors<VisualElementTraverser, VisualElement>, VisualElement> Ancestors(this VisualElement origin) => origin.AsTraverser().Ancestors();
        public static ValueEnumerable<Ancestors<VisualElementTraverser, VisualElement>, VisualElement> AncestorsAndSelf(this VisualElement origin) => origin.AsTraverser().AncestorsAndSelf();
        public static ValueEnumerable<BeforeSelf<VisualElementTraverser, VisualElement>, VisualElement> BeforeSelf(this VisualElement origin) => origin.AsTraverser().BeforeSelf();
        public static ValueEnumerable<BeforeSelf<VisualElementTraverser, VisualElement>, VisualElement> BeforeSelfAndSelf(this VisualElement origin) => origin.AsTraverser().BeforeSelfAndSelf();
        public static ValueEnumerable<AfterSelf<VisualElementTraverser, VisualElement>, VisualElement> AfterSelf(this VisualElement origin) => origin.AsTraverser().AfterSelf();
        public static ValueEnumerable<AfterSelf<VisualElementTraverser, VisualElement>, VisualElement> AfterSelfAndSelf(this VisualElement origin) => origin.AsTraverser().AfterSelfAndSelf();

    }

    [StructLayout(LayoutKind.Auto)]
    public struct VisualElementTraverser : ITraverser<VisualElementTraverser, VisualElement>
    {
        static readonly object CalledTryGetNextChild = new object();
        static readonly object ParentNotFound = new object();

        readonly VisualElement visualElement;
        object? initializedState; // CalledTryGetNext or Parent(for sibling operations)
        int childCount; // self childCount(TryGetNextChild) or parent childCount(TryGetSibling)
        int index;

        public VisualElementTraverser(VisualElement origin)
        {
            this.visualElement = origin;
            this.initializedState = null;
            this.childCount = 0;
            this.index = 0;
        }

        public VisualElement Origin => visualElement;
        public VisualElementTraverser ConvertToTraverser(VisualElement next) => new(next);

        public bool TryGetParent(out VisualElement parent)
        {
            var veParent = visualElement.parent;
            if (veParent != null)
            {
                parent = veParent;
                return true;
            }

            parent = default!;
            return false;
        }

        public bool TryGetChildCount(out int count)
        {
            count = visualElement.childCount;
            return true;
        }

        public bool TryGetHasChild(out bool hasChild)
        {
            hasChild = visualElement.childCount != 0;
            return true;
        }

        public bool TryGetNextChild(out VisualElement child)
        {
            if (initializedState == null)
            {
                initializedState = CalledTryGetNextChild;
                childCount = visualElement.childCount;
            }

            if (index < childCount)
            {
                child = visualElement[index++];
                return true;
            }

            child = default!;
            return false;
        }

        public bool TryGetNextSibling(out VisualElement next)
        {
            if (initializedState == null)
            {
                var veParent = visualElement.parent;
                if (veParent == null)
                {
                    initializedState = ParentNotFound;
                    next = default!;
                    return false;
                }

                // cache parent and childCount
                initializedState = veParent;
                childCount = veParent.childCount; // parent's childCount
                index = veParent.IndexOf(visualElement) + 1;
            }
            else if (initializedState == ParentNotFound)
            {
                next = default!;
                return false;
            }

            var parent = (VisualElement)initializedState;
            if (index < childCount)
            {
                next = parent[index++];
                return true;
            }

            next = default!;
            return false;
        }

        public bool TryGetPreviousSibling(out VisualElement previous)
        {
            if (initializedState == null)
            {
                var veParent = visualElement.parent;
                if (veParent == null)
                {
                    initializedState = ParentNotFound;
                    previous = default!;
                    return false;
                }

                initializedState = veParent;
                childCount = veParent.IndexOf(visualElement); // not childCount but means `to`
                index = 0; // 0 to siblingIndex
            }
            else if (initializedState == ParentNotFound)
            {
                previous = default!;
                return false;
            }

            var parent = (VisualElement)initializedState;
            if (index < childCount)
            {
                previous = parent[index++];
                return true;
            }

            previous = default!;
            return false;
        }

        public void Dispose()
        {
        }
    }
}

#endif
