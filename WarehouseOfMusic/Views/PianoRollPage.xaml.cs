namespace WarehouseOfMusic.Views
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Microsoft.Phone.Controls;
    using System.Threading.Tasks;
    using Models;
    using UIElementContexts;

    public partial class PianoRollPage : UserControl
    {
        /// <summary>
        /// All items of LLS
        /// </summary>
        private readonly Dictionary<object, ContentPresenter> _items = new Dictionary<object, ContentPresenter>();

        /// <summary>
        /// Use to detect tap doubletap on note
        /// </summary>
        private bool _singleTapOnNote;

        private double _cellWidth;

        private PianoRollContext _pianoRollContext;
        
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
            return _items.Where(x => Canvas.GetTop(x.Value) + x.Value.ActualHeight > offset)
                .OrderBy(x => Canvas.GetTop(x.Value)).First().Key;
        }

        private void LLS_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                _items.Add(o, e.Container);
            }
        }

        private void LLS_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                object o = e.Container.DataContext;
                _items.Remove(o);
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
            _pianoRollContext = this.DataContext as PianoRollContext;
            foreach (var item in
                    from object item in PianoKeys.ItemsSource
                    let key = item as KeyContext
                    where key.Value == PianoRollContext.TopKey
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
                    where key.Value == PianoRollContext.TopKey
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
            if (key != null) PianoRollContext.TopKey = key.Value;
        } 
        #endregion

        #region Adding, Selecting, Deleting notes
        /// <summary>
        /// Add new note to view and raise AddedNote event
        /// </summary>
        private void KeyCanvas_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;
            var canvas = sender as Canvas;
            if (canvas == null) return;
            var key = canvas.DataContext as KeyContext;
            if (key == null) return;

            var tapPoint = e.GetPosition(canvas);
            _cellWidth = canvas.ActualWidth / 16;
            var tactPostition = (tapPoint.X - (tapPoint.X % _cellWidth)) / _cellWidth;
            _pianoRollContext.AddNote((byte) key.Value,(byte)tactPostition);
        }

        /// <summary>
        /// Select note
        /// </summary>
        private async void noteRectangle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;
            this._singleTapOnNote = true;
            await Task.Delay(200);
            if (this._singleTapOnNote)
            {
            }
        }

        /// <summary>
        /// Delete note from view and raise DeletedNote event
        /// </summary>
        private void noteRectangle_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this._singleTapOnNote = false;
            var noteRectangle = sender as Rectangle;
            if (noteRectangle == null) return;
            var toDoNote = noteRectangle.DataContext as ToDoNote;
            if (toDoNote == null) return;
            _pianoRollContext.DeleteNote(toDoNote);
        }

        private void Note_OnLoaded(object sender, RoutedEventArgs e)
        {
            var noteRectangle = sender as Rectangle;
            if (noteRectangle == null) return;
            var toDoNote = noteRectangle.DataContext as ToDoNote;
            if (toDoNote == null) return;

            var blockWidth = toDoNote.Duration * _cellWidth;
            noteRectangle.Width = blockWidth;
            Canvas.SetTop(noteRectangle, 0);
            Canvas.SetLeft(noteRectangle, toDoNote.TactPosition * _cellWidth);
            noteRectangle.Tap += noteRectangle_Tap;
            noteRectangle.DoubleTap += noteRectangle_DoubleTap;
        }

        #endregion
        
    }
}
