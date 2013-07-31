using Core.DataStructures;
using Core.Geometry;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTagWithQuadTree.ViewModels
{
  public class MainViewModel
  {
    public ObservableCollection<Rectangle> Rectangles { get; set; }
    
    public MainViewModel()
    {
      Rectangles = new ObservableCollection<Rectangle>();
      LoadViewModelData();
    }


    private void LoadViewModelData()
    {
      Action<QuadTreeNode> addToViewModel = t =>
      {
        if (t != null && t.Surface != null)
        {

          Rectangles.Add(t.Surface);
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

      for (int i = 0; i < 2; i++)
      {
        var rb = RectangleBuilder.ARectangle()
                                     .WithMaxWidth(400)
                                     .WithMaxHeight(200)
                                     .WithCanvasMaxHeight(600)
                                     .WithCanvasMaxWidth(900)
                                     .BuildRandom();

        var rbChildren = RectangleBuilder.ARectangle()
                                 .WithMaxWidth(400)
                                 .WithMaxHeight(200)
                                 .WithCanvasMaxHeight(500)
                                 .WithCanvasMaxWidth(800)
                                 .BuildRandomList(Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT);

        var node = NodeBuilder.ANode()
                              .WithSurface(rb)
                              .WithSubSurfaces(rbChildren.ToArray())
                              .Build();
        tree.Insert(node);
      }



      tree.Visit(addToViewModel);
    }
  }
}
