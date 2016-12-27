using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.Wpf;

namespace CombiningOverlays
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Sets the correct map unit and the extent of the map.
            wpfMap1.MapUnit = GeographyUnit.DecimalDegree;
            wpfMap1.CurrentExtent = new RectangleShape(-131.22, 55.05, -54.03, 16.91);

            //Adds the WorldMapKit as a background.
            WorldMapKitWmsWpfOverlay worldMapKitOverlay = new WorldMapKitWmsWpfOverlay();
            wpfMap1.Overlays.Add(worldMapKitOverlay);

            //Adds the Shapefile MajorCities as a ShapeFileFeatureLayer between zoom levels 01 and 04.
            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(@"..\..\Data\MajorCities.shp");
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Turquoise, 8, GeoColor.StandardColors.Black);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level04;
            //Adds the ShapeFileFeatureLayer to an LayerOverlay.
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);
            wpfMap1.Overlays.Add(layerOverlay);

            //Uses FeatureSourceMakerOverlay between zoom levels 04 and 20.
            FeatureSourceMarkerOverlay markerOverlay = new FeatureSourceMarkerOverlay();
            //Takes the ShapeFileFeatureSource of the ShapeFileFeatureLayer as a FeatureSource of the MarkerOverlay.
            ShapeFileFeatureSource shapeFileFeatureSource = (ShapeFileFeatureSource)shapeFileFeatureLayer.FeatureSource;
            markerOverlay.FeatureSource = shapeFileFeatureSource;

            //Here we take advantage of the many properties of MarkerOverlay such as ImageSource and Tooltip.
            markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultPointMarkerStyle.ImageSource = new BitmapImage(new Uri("/Resources/AQUA.png", UriKind.RelativeOrAbsolute));
            markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultPointMarkerStyle.Width = 20;
            markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultPointMarkerStyle.Height = 34;
            markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultPointMarkerStyle.YOffset = -17;
            markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultPointMarkerStyle.ToolTip = "This is [#AREANAME#].";
            markerOverlay.ZoomLevelSet.ZoomLevel04.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            wpfMap1.Overlays.Add("MarkerOverlay", markerOverlay);   

            wpfMap1.Refresh();
        }

        private void wpfMap1_MouseMove(object sender, MouseEventArgs e)
        {
            //Gets the PointShape in world coordinates from screen coordinates.
            Point point = e.MouseDevice.GetPosition(null);
            
            ScreenPointF screenPointF = new ScreenPointF((float)point.X, (float)point.Y);
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(wpfMap1.CurrentExtent, screenPointF, (float)wpfMap1.Width, (float)wpfMap1.Height);

            textBox1.Text = "X: " + Math.Round(pointShape.X) + 
                          "  Y: " + Math.Round(pointShape.Y);

           }
        }
}
