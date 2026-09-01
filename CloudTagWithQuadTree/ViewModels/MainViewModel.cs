using CloudTagWithQuadTree.Rasterizers;
using Core.Cloud;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CloudTagWithQuadTree.ViewModels
{
  using GlbCnst = Core.Enums.GlobalConstants;

  public class MainViewModel : BaseViewModel
  {
    public ObservableCollection<PlacedWord> Words { get; private set; }
    public string Summary { get; private set; }

    public int CanvasWidth
    {
      get
      {
        return GlbCnst.MAX_WIDTH;
      }
    }

    public int CanvasHeight
    {
      get
      {
        return GlbCnst.MAX_HEIGHT;
      }
    }

    public MainViewModel()
    {
      Words = new ObservableCollection<PlacedWord>();
      Summary = string.Empty;
      LoadViewModelData();
    }


    private void LoadViewModelData()
    {
      // the typeface here and the FontFamily in MainWindow.xaml must be the same face,
      // or the masks the placer tested are not the glyphs the window draws
      var rasterizer = new FormattedTextRasterizer(new Typeface("Segoe UI"));
      var placer = new WordCloudPlacer(rasterizer, GlbCnst.MAX_WIDTH, GlbCnst.MAX_HEIGHT, GlbCnst.DEFAULT_GUTTER, 10, 64, 0.25, 2013);
      var result = placer.Place(SampleTags.Build());

      foreach (var word in result.Placed)
      {
        Words.Add(word);
      }

      Summary = string.Format("{0} placed, {1} refused, {2} ms, quadtree {3} nodes, {4} accepted and {5} refused on the tree alone, {6} bitwise tests",
        result.Placed.Count, result.Unplaced.Count, result.ElapsedMilliseconds,
        result.Board.Index.CountNodes(), result.Board.FastPathHits, result.Board.FastPathRejects, result.Board.BitwiseTests);
      RaisePropertyChanged("Summary");
    }
  }
}
