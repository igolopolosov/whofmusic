using WarehouseOfMusic.ViewModels;

namespace WarehouseOfMusic.Views
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using Microsoft.Phone.Controls;

    public partial class PianoRollPage : UserControl
    {
        public PianoRollPage()
        {
            InitializeComponent();
        }

        #region Control for firs visible element in list
        /// <summary>
        /// All items of LLS
        /// </summary>
        private Dictionary<object, ContentPresenter> items = new Dictionary<object, ContentPresenter>();

        /// <summary>
        /// Get first visible element of long list selector
        /// </summary>
        /// <returns></returns>
        private object GetFirstVisibleItem()
        {
            var offset = FindViewport(PianoKeys).Viewport.Top;
            return items.Where(x => Canvas.GetTop(x.Value) + x.Value.ActualHeight > offset)
                .OrderBy(x => Canvas.GetTop(x.Value)).First().Key;
        }

        private void LLS_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                items.Add(o, e.Container);
            }
        }

        private void LLS_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                items.Remove(o);
            }
        }

        private static ViewportControl FindViewport(DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; i++)
            {
                var elt = VisualTreeHelper.GetChild(parent, i);
                if (elt is ViewportControl) return (ViewportControl)elt;
                var result = FindViewport(elt);
                if (result != null) return result;
            }
            return null;
        } 
        #endregion

        #region Control for offset
        /// <summary>
        /// Initialize offset for this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PianoKeys_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in
                    from object item in PianoKeys.ItemsSource
                    let key = item as KeyContext
                    where key.Value == TactContext.PianoRollContext.TopKey
                    select item)
            {
                PianoKeys.ScrollTo(item);
            }
        }

        /// <summary>
        /// Set offset for this page
        /// </summary>
        public void Scroll()
        {
            foreach (var item in
                    from object item in PianoKeys.ItemsSource
                    let key = item as KeyContext
                    where key.Value == TactContext.PianoRollContext.TopKey
                    select item)
            {
                PianoKeys.ScrollTo(item);
            }
        }

        /// <summary>
        /// Save offset for all pages
        /// </summary>
        public void SaveOffset()
        {
            var key = GetFirstVisibleItem() as KeyContext;
            if (key != null) TactContext.PianoRollContext.TopKey = key.Value;
        } 
        #endregion
    }
}
