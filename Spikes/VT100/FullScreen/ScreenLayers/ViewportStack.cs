using System;
using System.Collections.Generic;

namespace VT100.FullScreen.ScreenLayers
{
    internal class ViewportStack : IViewportStack
    {
        private readonly IFullScreenConsole _console;
        private readonly PlateStack _rootPlateStack;
        private readonly Viewport _rootViewport;
        private readonly List<Viewport> _viewports;
        private ViewportTree _tree;

        public ViewportStack(IFullScreenConsole rootConsole, PlateStack rootPlateStack)
        {
            _console = rootConsole;
            _rootViewport = new Viewport(rootConsole, rootPlateStack, 0, 0, _console.WindowWidth, _console.WindowHeight);
            _rootPlateStack = rootPlateStack;
            _viewports = new();
            AddViewports(_rootViewport);
        }

        public Viewport RootViewport => _rootViewport;

        public void AddViewports(params Viewport[] viewports)
        {
            AddViewports((IEnumerable<Viewport>)viewports);    
        }
        
        public void AddViewports(IEnumerable<Viewport> viewports)
        {
            _viewports.AddRange(viewports);
            _tree = new ViewportTree(_viewports);
        }
        
        public void RemoveViewport(Viewport viewport)
        {
            _viewports.Remove(viewport);
            _tree = new ViewportTree(_viewports);
        }
        
        public void Render()
        {
            var visibleRegion = new Rectangle((0, 0), (_console.WindowWidth, _console.WindowHeight), (0,0));
            Render(_tree.Root, 0, 0, visibleRegion);
        }

        private void Render(ViewportTree.Node node, int parentRootActualCol, int parentRootActualRow, Rectangle visibleRegion)
        {
            node.Viewport.Render(parentRootActualCol, parentRootActualRow, visibleRegion);

            parentRootActualCol += node.Viewport.ColWithinParent - node.Viewport.FirstContainedVisibleColumn;
            parentRootActualRow += node.Viewport.RowWithinParent - node.Viewport.FirstContainedVisibleRow;
            
            var child = node.FirstChild;
            while (child != null)
            {
                var rectangle = visibleRegion.Intersect(child.GetRectangle(parentRootActualCol, parentRootActualRow));
                if (!rectangle.IsDegenerate()) 
                    Render(child, parentRootActualCol, parentRootActualRow, rectangle);
                child = child.NextSibling;
            }
        }

        public void BringIntoView(Viewport viewport, int col, int row)
        {
            if (!_tree.NodeIndex.TryGetValue(viewport, out var node)) return;

            while (node != null)
            {
                var (newCol, newRow) = node.Viewport.BringIntoView(col, row);
                node = node.Parent;
                col = newCol;
                row = newRow;
            }
        }

        public void BringIntoView(Viewport viewport, Rectangle targetRect)
        {
            if (!_tree.NodeIndex.TryGetValue(viewport, out var node)) return;

            while (node != null)
            {
                var newRect = node.Viewport.BringIntoView(targetRect);
                node = node.Parent;
                targetRect = newRect;
            }
        }
    }
    
}