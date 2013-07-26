﻿using Core.DataStructures;
using Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTests
{
  class Program
  {
    static void Main(string[] args)
    {

      Rectangle rootRectangle = new Rectangle(1, 1, 100, 100);






      QuadTreeNode root = new QuadTreeNode(rootRectangle);
      QuadTree tree = new QuadTree(root);

      QuadTreeNode second = new QuadTreeNode("second");
      QuadTreeNode third = new QuadTreeNode("third");
      QuadTreeNode forth = new QuadTreeNode("forth");
      QuadTreeNode fifth = new QuadTreeNode("fifth");
      QuadTreeNode sixth = new QuadTreeNode("sixth");
      QuadTreeNode seventh = new QuadTreeNode("seventh");
      QuadTreeNode eigth = new QuadTreeNode("eigth");
      QuadTreeNode ninth = new QuadTreeNode("ninth");

      tree.Insert(second);
      tree.Insert(third);
      tree.Insert(forth);
      tree.Insert(fifth);
      tree.Insert(sixth);
      tree.Insert(seventh);
      tree.Insert(eigth);
      tree.Insert(ninth);


      tree.Visit();
      Console.ReadLine();

    }
  }
}
