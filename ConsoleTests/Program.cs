using Core.DataStructures;
using Core.Geometry;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTests
{
  using GlbCnst = Core.Enums.GlobalConstants;

  class Program
  {
    static void Main(string[] args)
    {
        Action<QuadTreeNode> outputToConsole = t => 
          {
            if (t != null && t.Surface != null)
            {
              Console.WriteLine(t.Surface.Area);
            }
          };

        var rootRectangle = RectangleBuilder.ARectangle()
                                     .WithMaxWidth(400)
                                     .WithMaxHeight(200)
                                     .WithCanvasMaxHeight(600)
                                     .WithCanvasMaxWidth(900)
                                     .BuildMaximized();

        QuadTreeNode root = NodeBuilder.ANode()
                                       .WithSurface(rootRectangle)
                                       .Build();

        QuadTree tree = new QuadTree(root);
      
        for (int i = 0; i < 10; i++)
        {
          var rb = RectangleBuilder.ARectangle()
                                   .WithMaxWidth(400)
                                   .WithMaxHeight(200)
                                   .WithCanvasMaxHeight(600)
                                   .WithCanvasMaxWidth(900)
                                   .BuildRandom(); ;
          
            var rbChildren = RectangleBuilder.ARectangle()
                                     .WithMaxWidth(400)
                                     .WithMaxHeight(200)
                                     .WithCanvasMaxHeight(600)
                                     .WithCanvasMaxWidth(900)
                                     .BuildRandomList(GlbCnst.TREE_CHILDREN_COUNT);

            var node = NodeBuilder.ANode()
                                  .WithSurface(rb)
                                  .WithSubSurfaces(rbChildren.ToArray())
                                  .Build();
            tree.Insert(node);
        }


        tree.Visit(outputToConsole);
        Console.ReadKey();

    }
  }
}
