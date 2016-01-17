// ----------------------------------------------------
// Modified from FrayxRulez/SwipeListView (MIT License)
// see https://github.com/FrayxRulez/SwipeListView
// ----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace UWPToolkit.Controls
{
    public enum SwipeListDirection
    {
        Left, Right, Top, Buttom, None
    }

    public enum SwipeListBehavior
    {
        Expand, Collapse, Disabled
    }

    public class ItemSwipeEventArgs : EventArgs
    {
        public object SwipedItem { get; private set; }

        public SwipeListDirection Direction { get; private set; }

        public ItemSwipeEventArgs(object item, SwipeListDirection direction)
        {
            SwipedItem = item;
            Direction = direction;
        }
    }

    public delegate void ItemSwipeEventHandler(object sender, ItemSwipeEventArgs e);

    public class SwipeableItem : ListViewItem
    {
        private TranslateTransform ContentDragTransform;
        private TranslateTransform LeftOrTopTransform;
        private TranslateTransform RightOrButtomTransform;

        private Border LeftOrTopContainer;
        private Border RightOrButtomContainer;

        private Grid DragBackground;
        private RectangleGeometry DragClip;
        private TranslateTransform DragClipTransform;
        private Border DragContainer;

        public SwipeableItem()
        {
            DefaultStyleKey = typeof(SwipeableItem);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ContentDragTransform = (TranslateTransform)GetTemplateChild("ContentDragTransform");
            LeftOrTopTransform = (TranslateTransform)GetTemplateChild("LeftOrTopTransform");
            RightOrButtomTransform = (TranslateTransform)GetTemplateChild("RightOrButtomTransform");

            LeftOrTopContainer = (Border)GetTemplateChild("LeftOrTopContainer");
            RightOrButtomContainer = (Border)GetTemplateChild("RightOrButtomContainer");

            DragBackground = (Grid)GetTemplateChild("DragBackground");
            DragClip = (RectangleGeometry)GetTemplateChild("DragClip");
            DragClipTransform1 = (TranslateTransform)GetTemplateChild("DragClipTransform");
            DragContainer = (Border)GetTemplateChild("DragContainer");

            if(HorizontalMode)
            {
                DragContainer.ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateX;
            }
            else
            {
                DragContainer.ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateY;
            }
        }

        /// <summary>
        /// Resets the <see cref="SwipeListViewItem"/> swipe state.
        /// </summary>
        public void ResetSwipe()
        {
            if (DragBackground != null)
            {
                DragBackground.Background = null;
                DragClip.Rect = new Rect(0, 0, 0, 0);

                if(HorizontalMode)
                {
                    DragClipTransform1.X = 0;
                    ContentDragTransform.X = 0;
                    LeftOrTopTransform.X = -(LeftOrTopContainer.ActualWidth + 20);
                    RightOrButtomTransform.X = (RightOrButtomContainer.ActualWidth + 20);
                }
                else
                {
                    DragClipTransform1.Y = 0;
                    ContentDragTransform.Y = 0;
                    LeftOrTopTransform.Y = -(LeftOrTopContainer.ActualHeight + 20);
                    RightOrButtomTransform.Y = (RightOrButtomContainer.ActualHeight + 20);

                }
            }
        }

        private SwipeListDirection _direction = SwipeListDirection.None;


        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                e.Complete();
                return;
            }

            var delta = e.Delta.Translation;
            var cumulative = e.Cumulative.Translation;

            var target = ((HorizontalMode?ActualWidth:ActualHeight / 5) * 1);

            if (_direction == SwipeListDirection.None)
            {
                _direction = HorizontalMode?(delta.X > 0
                    ? SwipeListDirection.Left
                    : SwipeListDirection.Right): (delta.Y > 0
                    ? SwipeListDirection.Top
                    : SwipeListDirection.Buttom);

                //if(HorizontalMode)
                //{
                //    LeftOrTopTransform.X = -(LeftOrTopContainer.ActualWidth + 20);
                //    RightOrButtomTransform.X = (RightOrButtomContainer.ActualWidth + 20);
                //}
                //else
                //{
                //    LeftOrTopTransform.Y = -(LeftOrTopContainer.ActualHeight + 20);
                //    RightOrButtomTransform.Y = (RightOrButtomContainer.ActualHeight + 20);
                //}

                DragClip.Rect = HorizontalMode? new Rect(_direction == SwipeListDirection.Left ? -ActualWidth : ActualWidth, 0, ActualWidth, ActualHeight)
                    : new Rect(0,_direction == SwipeListDirection.Top ? -ActualHeight : ActualHeight, ActualWidth, ActualHeight);

                if ((_direction == SwipeListDirection.Left || _direction == SwipeListDirection.Top) && LeftOrTopBehavior != SwipeListBehavior.Disabled)
                {
                    DragBackground.Background = LeftOrTopBackground;

                    LeftOrTopContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    RightOrButtomContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else if ((_direction == SwipeListDirection.Right || _direction == SwipeListDirection.Buttom) && RightOrButtomBehavior != SwipeListBehavior.Disabled)
                {
                    DragBackground.Background = RightOrButtomBackground;

                    LeftOrTopContainer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    RightOrButtomContainer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    e.Complete();
                    return;
                }
            }

            if (_direction == SwipeListDirection.Left || _direction == SwipeListDirection.Top)
            {
                var area1 = LeftOrTopBehavior == SwipeListBehavior.Collapse ? 1.5 : 2.5;
                var area2 = LeftOrTopBehavior == SwipeListBehavior.Collapse ? 2 : 3;

                if(HorizontalMode)
                {
                    ContentDragTransform.X = Math.Max(0, Math.Min(cumulative.X, ActualWidth));
                    DragClipTransform1.X = Math.Max(0, Math.Min(cumulative.X, ActualWidth));

                    if (ContentDragTransform.X < target * area1)
                    {
                        LeftOrTopTransform.X += (delta.X / 1.5);
                    }
                    else if (ContentDragTransform.X >= target * area1 && ContentDragTransform.X < target * area2)
                    {
                        LeftOrTopTransform.X += (delta.X * 2.5);
                    }
                    else
                    {
                        LeftOrTopTransform.X = Math.Max(0, Math.Min(cumulative.X, ActualWidth)) - LeftOrTopContainer.ActualWidth;
                    }

                    if (ContentDragTransform.X == 0 && delta.X < 0)
                    {
                        _direction = SwipeListDirection.None;
                    }
                }
                else
                {
                    ContentDragTransform.Y = Math.Max(0, Math.Min(cumulative.Y, ActualHeight));
                    DragClipTransform1.Y = Math.Max(0, Math.Min(cumulative.Y, ActualHeight));

                    if (ContentDragTransform.Y < target * area1)
                    {
                        LeftOrTopTransform.Y += (delta.Y / 1.5);
                    }
                    else if (ContentDragTransform.Y >= target * area1 && ContentDragTransform.Y < target * area2)
                    {
                        LeftOrTopTransform.Y += (delta.Y * 2.5);
                    }
                    else
                    {
                        LeftOrTopTransform.Y = Math.Max(0, Math.Min(cumulative.Y, ActualHeight)) - LeftOrTopContainer.ActualHeight;
                    }

                    if (ContentDragTransform.Y == 0 && delta.Y < 0)
                    {
                        _direction = SwipeListDirection.None;
                    }
                }

            }
            else if (_direction == SwipeListDirection.Right || _direction == SwipeListDirection.Buttom)
            {
                var area1 = RightOrButtomBehavior == SwipeListBehavior.Collapse ? 1.5 : 2.5;
                var area2 = RightOrButtomBehavior == SwipeListBehavior.Collapse ? 2 : 3;

                if (HorizontalMode)
                {
                    ContentDragTransform.X = Math.Max(-ActualWidth, Math.Min(cumulative.X, 0));
                    DragClipTransform1.X = Math.Max(-ActualWidth, Math.Min(cumulative.X, 0));

                    if (ContentDragTransform.X > -(target * area1))
                    {
                        RightOrButtomTransform.X += (delta.X / 1.5);
                    }
                    else if (ContentDragTransform.X <= -(target * area1) && ContentDragTransform.X > -(target * area2))
                    {
                        RightOrButtomTransform.X += (delta.X * 2.5);
                    }
                    else
                    {
                        RightOrButtomTransform.X = Math.Max(-ActualWidth, Math.Min(cumulative.X, 0)) + RightOrButtomContainer.ActualWidth;
                    }

                    if (ContentDragTransform.X == 0 && delta.X > 0)
                    {
                        _direction = SwipeListDirection.None;
                    }
                }
                else
                {
                    ContentDragTransform.Y = Math.Max(-ActualHeight, Math.Min(cumulative.Y, 0));
                    DragClipTransform1.Y = Math.Max(-ActualHeight, Math.Min(cumulative.Y, 0));

                    if (ContentDragTransform.Y > -(target * area1))
                    {
                        RightOrButtomTransform.Y += (delta.Y / 1.5);
                    }
                    else if (ContentDragTransform.Y <= -(target * area1) && ContentDragTransform.Y > -(target * area2))
                    {
                        RightOrButtomTransform.Y += (delta.Y * 2.5);
                    }
                    else
                    {
                        RightOrButtomTransform.Y = Math.Max(-ActualHeight, Math.Min(cumulative.Y, 0)) + RightOrButtomContainer.ActualHeight;
                    }

                    if (ContentDragTransform.Y == 0 && delta.Y > 0)
                    {
                        _direction = SwipeListDirection.None;
                    }
                }
            }
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            double target;
            if (HorizontalMode)
            {
                target = (ActualWidth / 5) * 2;
            }
            else
            {
                target = (ActualHeight / 5) * 2;
            }


            if (((_direction == SwipeListDirection.Left|| _direction == SwipeListDirection.Top) && LeftOrTopBehavior == SwipeListBehavior.Expand) ||
                ((_direction == SwipeListDirection.Right || _direction == SwipeListDirection.Buttom) && RightOrButtomBehavior == SwipeListBehavior.Expand))
            {
                if (HorizontalMode)
                {
                    target = (ActualWidth / 5) * 3;
                }
                else
                {
                    target = (ActualHeight / 5) * 3;
                }
            }

            Storyboard currentAnim;

            if (HorizontalMode)
            {
                if (_direction == SwipeListDirection.Left && ContentDragTransform.X >= target)
                {
                    if (LeftOrTopBehavior == SwipeListBehavior.Collapse)
                        currentAnim = CollapseAnimation(SwipeListDirection.Left, true);
                    else
                        currentAnim = ExpandAnimation(SwipeListDirection.Left);
                }
                else if (_direction == SwipeListDirection.Right && ContentDragTransform.X <= -target)
                {
                    if (RightOrButtomBehavior == SwipeListBehavior.Collapse)
                        currentAnim = CollapseAnimation(SwipeListDirection.Right, true);
                    else
                        currentAnim = ExpandAnimation(SwipeListDirection.Right);
                }
                else
                {
                    currentAnim = CollapseAnimation(SwipeListDirection.Left, false);
                }
            }
            else
            {
                if (_direction == SwipeListDirection.Top && ContentDragTransform.Y >= target)
                {
                    if (LeftOrTopBehavior == SwipeListBehavior.Collapse)
                        currentAnim = CollapseAnimation(SwipeListDirection.Top, true);
                    else
                        currentAnim = ExpandAnimation(SwipeListDirection.Top);
                }
                else if (_direction == SwipeListDirection.Buttom && ContentDragTransform.Y <= -target)
                {
                    if (RightOrButtomBehavior == SwipeListBehavior.Collapse)
                        currentAnim = CollapseAnimation(SwipeListDirection.Buttom, true);
                    else
                        currentAnim = ExpandAnimation(SwipeListDirection.Buttom);
                }
                else
                {
                    currentAnim = CollapseAnimation(SwipeListDirection.Top, false);
                }
            }

            currentAnim.Begin();
            _direction = SwipeListDirection.None;

            //e.Handled = true;
            //base.OnManipulationCompleted(e);
        }

        private Storyboard CollapseAnimation(SwipeListDirection direction, bool raise)
        {
            var transform = HorizontalMode ? "TranslateTransform.X" : "TranslateTransform.Y";
            var animDrag = CreateDouble(0, 300, ContentDragTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseOut });
            var animClip = CreateDouble(0, 300, DragClipTransform1, transform, new ExponentialEase { EasingMode = EasingMode.EaseOut });
            var animLeftOrTop = CreateDouble(-(HorizontalMode?LeftOrTopContainer.ActualWidth: LeftOrTopContainer.ActualHeight + 20), 300, LeftOrTopTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseOut });
            var animRightOrButtom = CreateDouble((HorizontalMode ? RightOrButtomContainer.ActualWidth: RightOrButtomContainer.ActualHeight + 20), 300, RightOrButtomTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseOut });

            var currentAnim = new Storyboard();
            currentAnim.Children.Add(animDrag);
            currentAnim.Children.Add(animClip);
            currentAnim.Children.Add(animLeftOrTop);
            currentAnim.Children.Add(animRightOrButtom);

            currentAnim.Completed += (s, args) =>
            {
                DragBackground.Background = null;

                if (HorizontalMode)
                {
                    ContentDragTransform.X = 0;
                    LeftOrTopTransform.X = -(LeftOrTopContainer.ActualWidth + 20);
                    RightOrButtomTransform.X = (RightOrButtomContainer.ActualWidth + 20);
                }
                else
                {
                    ContentDragTransform.Y = 0;
                    LeftOrTopTransform.Y = -(LeftOrTopContainer.ActualHeight + 20);
                    RightOrButtomTransform.Y = (RightOrButtomContainer.ActualHeight + 20);
                }


                Grid.SetColumn(DragBackground, 1);
                Grid.SetColumnSpan(DragBackground, 1);

            };

            if (raise)
            {
                if (ItemSwipe != null)
                    ItemSwipe(this, new ItemSwipeEventArgs(Content, direction));
            }

            return currentAnim;
        }

        private Storyboard ExpandAnimation(SwipeListDirection direction)
        {
            var currentAnim = new Storyboard();
            var transform = HorizontalMode ? "TranslateTransform.X" : "TranslateTransform.Y";
            var dimension = HorizontalMode ? ActualWidth : ActualHeight;
            if (direction == SwipeListDirection.Left || direction == SwipeListDirection.Top)
            {
                var animDrag = CreateDouble(dimension + 100, 300, ContentDragTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseOut });
                var animClip = CreateDouble(dimension, 300, DragClipTransform1, transform, new ExponentialEase { EasingMode = EasingMode.EaseOut });
                var animLeftOrTop = CreateDouble(dimension + 100, 300, LeftOrTopTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseIn });
                var animRightOrButtom = CreateDouble(dimension + 100, 300, RightOrButtomTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseIn });

                currentAnim.Children.Add(animDrag);
                currentAnim.Children.Add(animClip);
                currentAnim.Children.Add(animLeftOrTop);
                currentAnim.Children.Add(animRightOrButtom);
            }
            else if (direction == SwipeListDirection.Right|| direction == SwipeListDirection.Buttom)
            {
                var animDrag = CreateDouble(-dimension - 100, 300, ContentDragTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseOut });
                var animClip = CreateDouble(-dimension, 300, DragClipTransform1, transform, new ExponentialEase { EasingMode = EasingMode.EaseOut });
                var animLeftOrTop = CreateDouble(-dimension - 100, 300, LeftOrTopTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseIn });
                var animRightOrButtom = CreateDouble(-dimension - 100, 300, RightOrButtomTransform, transform, new ExponentialEase { EasingMode = EasingMode.EaseIn });

                currentAnim.Children.Add(animDrag);
                currentAnim.Children.Add(animClip);
                currentAnim.Children.Add(animLeftOrTop);
                currentAnim.Children.Add(animRightOrButtom);
            }

            currentAnim.Completed += (s, args) =>
            {
                if (ItemSwipe != null)
                    ItemSwipe(this, new ItemSwipeEventArgs(Content, direction));
            };

            return currentAnim;
        }

        private DoubleAnimation CreateDouble(double to, int duration, DependencyObject target, string path, EasingFunctionBase easing)
        {
            var anim = new DoubleAnimation();
            anim.To = to;
            anim.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
            anim.EasingFunction = easing;

            Storyboard.SetTarget(anim, target);
            Storyboard.SetTargetProperty(anim, path);

            return anim;
        }

        /// <summary>
        /// Occurs when the item is swiped from left or right.
        /// </summary>
        public event ItemSwipeEventHandler ItemSwipe;

        #region Horizontal mode
        public bool HorizontalMode
        {
            get { return (bool)GetValue(HorizontalModeProperty); }
            set { SetValue(HorizontalModeProperty, value); }
        }

        /// <summary>
        /// Identifies the LeftContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalModeProperty =
            DependencyProperty.Register(nameof(HorizontalMode), typeof(bool), typeof(SwipeableItem), new PropertyMetadata(true));
        #endregion

        #region LeftContentTemplate
        public DataTemplate LeftOrTopContentTemplate
        {
            get { return (DataTemplate)GetValue(LeftOrTopContentTemplateProperty); }
            set { SetValue(LeftOrTopContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the LeftContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftOrTopContentTemplateProperty =
            DependencyProperty.Register(nameof(LeftOrTopContentTemplate), typeof(DataTemplate), typeof(SwipeableItem), new PropertyMetadata(null));
        #endregion

        #region LeftBackground
        public Brush LeftOrTopBackground
        {
            get { return (Brush)GetValue(LeftOrTopBackgroundProperty); }
            set { SetValue(LeftOrTopBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the LeftBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftOrTopBackgroundProperty =
            DependencyProperty.Register(nameof(LeftOrTopBackground), typeof(Brush), typeof(SwipeableItem), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));
        #endregion

        #region LeftBehavior
        public SwipeListBehavior LeftOrTopBehavior
        {
            get { return (SwipeListBehavior)GetValue(LeftOrTopBehaviorProperty); }
            set { SetValue(LeftOrTopBehaviorProperty, value); }
        }

        /// <summary>
        /// Identifies the LeftBehavior dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftOrTopBehaviorProperty =
            DependencyProperty.Register(nameof(LeftOrTopBehavior), typeof(SwipeListBehavior), typeof(SwipeableItem), new PropertyMetadata(SwipeListBehavior.Collapse));
        #endregion

        #region RightContentTemplate
        public DataTemplate RightOrButtomContentTemplate
        {
            get { return (DataTemplate)GetValue(RightContentTemplateProperty); }
            set { SetValue(RightContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the RightContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty RightContentTemplateProperty =
            DependencyProperty.Register(nameof(RightOrButtomContentTemplate), typeof(DataTemplate), typeof(SwipeableItem), new PropertyMetadata(null));
        #endregion

        #region RightBackground
        public Brush RightOrButtomBackground
        {
            get { return (Brush)GetValue(RightOrButtomBackgroundProperty); }
            set { SetValue(RightOrButtomBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the RightBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty RightOrButtomBackgroundProperty =
            DependencyProperty.Register(nameof(RightOrButtomBackground), typeof(Brush), typeof(SwipeableItem), new PropertyMetadata(new SolidColorBrush(Colors.Red)));
        #endregion

        #region RightBehavior
        public SwipeListBehavior RightOrButtomBehavior
        {
            get { return (SwipeListBehavior)GetValue(RightOrButtomBehaviorProperty); }
            set { SetValue(RightOrButtomBehaviorProperty, value); }
        }

        public TranslateTransform DragClipTransform1
        {
            get
            {
                return DragClipTransform;
            }

            set
            {
                DragClipTransform = value;
            }
        }

        /// <summary>
        /// Identifies the RightBehavior dependency property.
        /// </summary>
        public static readonly DependencyProperty RightOrButtomBehaviorProperty =
            DependencyProperty.Register(nameof(RightOrButtomBehavior), typeof(SwipeListBehavior), typeof(SwipeableItem), new PropertyMetadata(SwipeListBehavior.Collapse));
        #endregion
    }
}
