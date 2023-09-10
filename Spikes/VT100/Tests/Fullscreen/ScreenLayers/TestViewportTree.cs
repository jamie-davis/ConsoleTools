using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.FullScreen.ScreenLayers;
using VT100.Tests.Fakes;
using Xunit;

namespace VT100.Tests.Fullscreen.ScreenLayers
{
    public class TestViewportTree
    {
        private readonly FakeFullScreenConsole _console = new(50, 50);

        [Fact]
        public void RootIsSetToUnparentedViewport()
        {
            //Arrange

            var root = new Viewport(_console, MakePlates(), 0, 0, 50, 50);
            var children = Enumerable.Range(0, 4)
                .Select(n =>
                {
                    var viewport = new Viewport(_console, MakePlates(), 0, n * 3, 50, 2);
                    viewport.Container = root;
                    return viewport;
                })
                .ToList();
            
            //Act
            var result = new ViewportTree(children.Concat(new[] { root }));
            
            //Assert
            result.Root.Viewport.Should().BeSameAs(root);
        }

        [Fact]
        public void RootChildrenAreLinked()
        {
            //Arrange
            var root = new Viewport(_console, MakePlates(), 0, 0, 50, 50);
            var children = MakeChildList(root);
            
            //Act
            var result = new ViewportTree(children.Concat(new[] { root }));
            
            //Assert
            var output = new Output();
            output.WrapLine("Tree expected to consist of a root and four children:");
            RenderTree(result, output);
            output.Report.Verify();
        }

        [Fact]
        public void ChildrenAreLinked()
        {
            //Arrange
            var root = new Viewport(_console, MakePlates(), 0, 0, 50, 50);
            var children = MakeChildList(root);
            foreach (var child in children.ToList())
            {
                var grandchildren = MakeChildList(child);
                children.AddRange(grandchildren);
            }
            
            //Act
            var result = new ViewportTree(children.Concat(new[] { root }));
            
            //Assert
            var output = new Output();
            output.WrapLine("Tree expected to consist of a root and four children, with four children each:");
            RenderTree(result, output);
            output.Report.Verify();
        }

        private List<Viewport> MakeChildList(Viewport root)
        {
            return Enumerable.Range(0, 4)
                .Select(n =>
                {
                    var viewport = new Viewport(_console, MakePlates(), 0, n * 3, 50, 2);
                    viewport.Container = root;
                    return viewport;
                })
                .ToList();
        }

        private void RenderTree(ViewportTree tree, Output output)
        {
            string DescribeNode(int indent, ViewportTree.Node node)
            {
                return $"{new string(' ', indent)}@({node.Viewport.ColWithinParent},{node.Viewport.RowWithinParent}) " +
                       $"(W:{node.Viewport.WidthWithinParent} H:{node.Viewport.HeightWithinParent}) ";
            }

            string MakeNodeString(int indent, ViewportTree.Node node)
            {
                return $"{DescribeNode(indent, node)} Parent: {(node.Parent == null ? "null" : DescribeNode(indent, node.Parent))}";
            }

            RenderNode(tree.Root, 0);

            void RenderNode(ViewportTree.Node node, int indent)
            {
                var nodeString = MakeNodeString(indent, node);
                output.WriteLine(nodeString);
                var contained = node.FirstChild;
                while (contained != null)
                {
                    RenderNode(contained, indent + 4);
                    contained = contained.NextSibling;
                }
            }
        }

        private PlateStack MakePlates()
        {
            return new PlateStack(new Plate(10, 10));
        }
    }
}