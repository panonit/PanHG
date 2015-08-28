using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing;

namespace PanHG
{
    public class AnimatedGif : System.Windows.Controls.Image
    {
        private Bitmap _bitmap; // Local bitmap member to cache image resource
        private BitmapSource _bitmapSource;
        public delegate void FrameUpdatedEventHandler();

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Override the OnInitialized method
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Loaded += new RoutedEventHandler(AnimatedCaseLoader_Loaded);
            this.Unloaded += new RoutedEventHandler(AnimatedCaseLoader_Unloaded);

        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name.Equals("Visibility"))
            {
                System.Diagnostics.Debug.WriteLine(e.Property.Name + " " + e.NewValue.ToString());
            }
        }

        /// <summary>
        /// Load the embedded image for the Image.Source
        /// </summary>

        void AnimatedCaseLoader_Loaded(object sender, RoutedEventArgs e)
        {
            // Get GIF image from Resources
            if (Properties.Resources.Loader != null)
            {
                _bitmap = Properties.Resources.Loader;
                Width = _bitmap.Width;
                Height = _bitmap.Height;

                _bitmapSource = GetBitmapSource();
                Source = _bitmapSource;
            }
            StartAnimate();
        }

        /// <summary>
        /// Close the FileStream to unlock the GIF file
        /// </summary>
        private void AnimatedCaseLoader_Unloaded(object sender, RoutedEventArgs e)
        {
            StopAnimate();
        }

        /// <summary>
        /// Start animation
        /// </summary>
        public void StartAnimate()
        {
            ImageAnimator.Animate(_bitmap, OnFrameChanged);
        }

        /// <summary>
        /// Stop animation
        /// </summary>
        public void StopAnimate()
        {
            ImageAnimator.StopAnimate(_bitmap, OnFrameChanged);
        }

        /// <summary>
        /// Event handler for the frame changed
        /// </summary>
        private void OnFrameChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                   new FrameUpdatedEventHandler(FrameUpdatedCallback));
        }

        private void FrameUpdatedCallback()
        {
            ImageAnimator.UpdateFrames();

            if (_bitmapSource != null)
                _bitmapSource.Freeze();

            // Convert the bitmap to BitmapSource that can be display in WPF Visual Tree
            _bitmapSource = GetBitmapSource();
            Source = _bitmapSource;
            InvalidateVisual();
        }

        private BitmapSource GetBitmapSource()
        {
            IntPtr handle = IntPtr.Zero;

            try
            {
                handle = _bitmap.GetHbitmap();
                _bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                if (handle != IntPtr.Zero)
                    DeleteObject(handle);
            }

            return _bitmapSource;
        }

    }
}
