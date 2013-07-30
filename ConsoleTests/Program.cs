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
       var rootRectangle = RectangleBuilder.GetRectangleWithRandomProperties();


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
                                     .Build();
          
            var rbChildren = RectangleBuilder.ARectangle()
                                     .WithMaxWidth(400)
                                     .WithMaxHeight(200)
                                     .WithCanvasMaxHeight(600)
                                     .WithCanvasMaxWidth(900)
                                     .BuildList(GlbCnst.TREE_CHILDREN_COUNT);
           


            var node = NodeBuilder.ANode()
                                  .WithSurface(rb)
                                  .WithSubSurfaces(rbChildren.ToArray())
                                  .Build();
            tree.Insert(node);
        }


        tree.Visit();
        Console.ReadKey();

    }
  }
}
