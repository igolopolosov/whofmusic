using System;
using System.Windows.Input;
using System.Windows.Shapes;
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
    using System.Threading.Tasks;

    public partial class PianoRollPage : UserControl
    {
        /// <summary>
        /// All items of LLS
        /// </summary>
        private Dictionary<object, ContentPresenter> items = new Dictionary<object, ContentPresenter>();

        /// <summary>
        /// Use to detect tap doubletap on note
        /// </summary>
        private bool singleTapOnNote;
        
        public PianoRollPage()
        {
            InitializeComponent();
        }

        #region Control for firs visible element in list

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

        #region Adding, Selecting, Deleting notes
        /// <summary>
        /// Detect tap on pianoroll to adding note
        /// </summary>
        private void KeyCanvas_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var canvas = sender as Canvas;
            if (canvas == null) return;
            var tapPoint = e.GetPosition(canvas);
            AddNote(canvas, tapPoint);
        }

        /// <summary>
        /// Add new note to view and raise AddedNote event
        /// </summary>
        private void AddNote(Panel panel, Point tapPoint)
        {
            var cellWidth = panel.ActualWidth / 16;
            var blockWidth = TactContext.PianoRollContext.NoteDuration * cellWidth;
            var noteRectangle = new Rectangle
            {
                Width = blockWidth,
                Height = panel.ActualHeight,
                Fill = new SolidColorBrush((Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color)
            };
            noteRectangle.Tap += noteRectangle_Tap;
            noteRectangle.DoubleTap += noteRectangle_DoubleTap;
            Canvas.SetTop(noteRectangle, 0);
            Canvas.SetLeft(noteRectangle, tapPoint.X - (tapPoint.X % cellWidth));
            Canvas.SetZIndex(noteRectangle, 1);
            panel.Children.Add(noteRectangle);

            var key = panel.DataContext as KeyContext;
            var tactPostition = (tapPoint.X - (tapPoint.X % cellWidth)) / cellWidth + 1;
            if (key == null) return;
            if (AddedNote != null)
                AddedNote(this, new NoteEventArgs
                {
                    Key = key.Value,
                    TactPosition = (byte)tactPostition
                });
        }

        /// <summary>
        /// Select note
        /// </summary>
        private async void noteRectangle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;
            this.singleTapOnNote = true;
            await Task.Delay(200);
            if (this.singleTapOnNote)
            {
            }
        }

        /// <summary>
        /// Delete note from view and raise DeletedNote event
        /// </summary>
        private void noteRectangle_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.singleTapOnNote = false;
            var noteRectangle = sender as Rectangle;
            if (noteRectangle == null) return;
            var keyCanvas = noteRectangle.Parent as Canvas;
            if (keyCanvas == null) return;
            var key = keyCanvas.DataContext as KeyContext;
            if (key == null) return;
            keyCanvas.Children.Remove(noteRectangle);

            var cellWidth = keyCanvas.ActualWidth / 16;
            var tactPostition = Canvas.GetLeft(noteRectangle) / cellWidth + 1;
            if (DeletedNote != null)
                DeletedNote(this, new NoteEventArgs
                {
                    Key = key.Value,
                    TactPosition = (byte)tactPostition
                });

        }

        public delegate void NoteChangedHandler(object sender, NoteEventArgs e);

        /// <summary>
        /// Happend, when user add new note on piano roll
        /// </summary>
        public event NoteChangedHandler AddedNote;
        /// <summary>
        /// Happend, when user delete note from piano roll
        /// </summary>
        public event NoteChangedHandler DeletedNote; 
        #endregion
    }
}
