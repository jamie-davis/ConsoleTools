using System.Collections.Generic;
using System.Linq;

namespace VT100.FullScreen.ScreenLayers
{
    /// <summary>
    /// Immutable viewport tree.
    /// </summary>
    internal class ViewportTree
    {
        public ViewportTree(IEnumerable<Viewport> viewports)
        {
            NodeIndex = viewports.ToDictionary(v => v, v => new Node(v));
            foreach (var node in NodeIndex.Values)
            {
                node.Link(NodeIndex);
                if (node.Parent == null)
                    Root = node;
            }
        }

        internal class Node
        {
            public Node(Viewport viewport)
            {
                Viewport = viewport;
            }

            public Viewport Viewport { get; }
            public Node Parent { get; private set; }
            public Node FirstChild { get; private set; }
            public Node NextSibling { get; private set; }

            public void Link(Dictionary<Viewport, Node> nodeIndex)
            {
                if (Parent != null || Viewport.Container == null || !nodeIndex.TryGetValue(Viewport.Container, out var parent)) return;

                Parent = parent;
                if (parent.FirstChild == null)
                {
                    Parent.FirstChild = this;
                    return;
                }

                var child = Parent.FirstChild;
                while (child.NextSibling != null)
                    child = child.NextSibling;

                child.NextSibling = this;
            }

            public Rectangle GetRectangle(int containerOriginCol, int containerOriginRow)
            {
                return new Rectangle((containerOriginCol + Viewport.ColWithinParent, containerOriginRow + Viewport.RowWithinParent),
                    (Viewport.WidthWithinParent, Viewport.HeightWithinParent), (-1, -1));
            }
        }

        public Node Root { get; }

        public Dictionary<Viewport, Node> NodeIndex { get; }
    }
}