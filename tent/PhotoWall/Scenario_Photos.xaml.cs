using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows​.UI​.Xaml​.Controls​.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Tent
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario_Photos
    {
        public Scenario_Photos()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var propertyDesc = e.Parameter as PropertyDescriptor;

            if (propertyDesc != null)
            {
                DataContext = propertyDesc.Expando;
            }

            AdaptiveGridViewControl.ItemsSource = await new PhotosDataSource().GetItemsAsync();
        }

        private void Image_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Image senderImg = sender as Image;
            senderImg.Blur(value: 0, duration: 100, delay: 0).StartAsync();
            //senderImg.Scale(centerX: 50.0f, centerY: 50.0f, scaleX: 3.0f, scaleY: 3.0f, duration: 100, delay: 0).StartAsync();
            
            Grid itemGrid = VisualTreeHelper.GetParent(senderImg) as Grid;
            ListViewItemPresenter lvip = VisualTreeHelper.GetParent(itemGrid) as ListViewItemPresenter;
            GridViewItem gvi = VisualTreeHelper.GetParent(lvip) as GridViewItem;
            ItemsWrapGrid grid = VisualTreeHelper.GetParent(gvi) as ItemsWrapGrid;

            string senderAutomationId = AutomationProperties.GetAutomationId(senderImg);
        
            foreach (GridViewItem item in grid.Children)
            {
                ListViewItemPresenter lviPresenter = VisualTreeHelper.GetChild(item, 0) as ListViewItemPresenter;
                Grid imageGrid = VisualTreeHelper.GetChild(lviPresenter, 0) as Grid;
                Image image = VisualTreeHelper.GetChild(imageGrid, 0) as Image;

                string imageAutomationId = AutomationProperties.GetAutomationId(image);

                if (imageAutomationId != senderAutomationId)
                {
                    image.Blur(value: 2.5, duration: 300, delay: 0).StartAsync();
                    //image.Scale(centerX: 0.0f, centerY: 0.0f, scaleX: 1.0f, scaleY: 1.0f, duration: 100, delay: 0).StartAsync();
                }
            }
        }

        private void Image_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //Image img = sender as Image;
            //img.Blur(value: 2.5, duration: 100, delay: 0).Start();
        }

        private void Image_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Image img = sender as Image;
            img.Blur(value: 2.5, duration: 0, delay: 0).StartAsync();
        }
    }
}
