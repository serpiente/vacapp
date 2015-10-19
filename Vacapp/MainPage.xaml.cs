using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

// Directives
using Microsoft.Devices;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;


using System.Windows.Threading;
using Microsoft.Xna.Framework;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;


namespace Vacapp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Variables
        private int savedCounter = 0;
        PhotoCamera cam;
        MediaLibrary library = new MediaLibrary();

        System.Windows.Point Point1, Point2;

      

        private void OriginalImage_MouseMove(object sender, MouseEventArgs e)
        {
            Point2 = e.GetPosition(viewfinderCanvas);
        }
        //Mouse Up
        private void OriginalImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point2 = e.GetPosition(viewfinderCanvas);
        }
        //Mouse Down
        private void OriginalImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point1 = e.GetPosition(viewfinderCanvas);//Set first touchable cordinates as point1
            Point2 = Point1;
            rect.Visibility = Visibility.Visible;
        }

      
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);    
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            //Used for rendering the cropping rectangle on the image.
            rect.SetValue(Canvas.LeftProperty, (Point1.X < Point2.X) ? Point1.X : Point2.X);
            rect.SetValue(Canvas.TopProperty, (Point1.Y < Point2.Y) ? Point1.Y : Point2.Y);
            rect.Width = (int)Math.Abs(Point2.X - Point1.X);
            rect.Height = (int)Math.Abs(Point2.Y - Point1.Y);
        }

     
        //Code for initialization, capture completed, image availability events; also setting the source for the viewfinder.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            
            // Check to see if the camera is available on the phone.
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true))
            {
                // Initialize the camera, when available.
                if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary))                
                {
                    // Otherwise, use standard camera on back of phone.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                }

                // Event is fired when the PhotoCamera object has been initialized.
                cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                // Event is fired when the capture sequence is complete.
                cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);

                // Event is fired when the capture sequence is complete and an image is available.
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);

                // Event is fired when the capture sequence is complete and a thumbnail image is available.
                cam.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureThumbnailAvailable);

                //Set the VideoBrush source to the camera.
                viewfinderBrush.SetSource(cam);
                
            }
            else
            {
                // The camera is not supported on the phone.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    txtDebug.Text = "A Camera is not available on this phone.";
                });

                // Disable UI.
                ShutterButton.IsEnabled = false;
            }
        }
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                // Dispose camera to minimize power consumption and to expedite shutdown.
                cam.Dispose();

                // Release memory, ensure garbage collection.
                cam.Initialized -= cam_Initialized;
                cam.CaptureCompleted -= cam_CaptureCompleted;
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
                cam.CaptureThumbnailAvailable -= cam_CaptureThumbnailAvailable;
            }
        }

        // Update the UI if initialization succeeds.
        void cam_Initialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    txtDebug.Text = "Camera initialized.";
                });                                    
            }
        }

        // Ensure that the viewfinder is upright in LandscapeRight.
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (cam != null)
            {
                // LandscapeRight rotation when camera is on back of phone.
                int landscapeRightRotation = 180;                

                // Rotate video brush from camera.
                if (e.Orientation == PageOrientation.LandscapeRight)
                {
                    // Rotate for LandscapeRight orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = landscapeRightRotation };
                }
                else
                {
                    // Rotate for standard landscape orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                }
            }

            base.OnOrientationChanged(e);
        }

        private void ShutterButton_Click(object sender, RoutedEventArgs e)
        {
            if (cam != null)
            {
                try
                {
                    // Start image capture.                   
                    cam.CaptureImage();                  
                    
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        // Cannot capture an image until the previous capture has completed.
                        txtDebug.Text = ex.Message;
                    });
                }
            }
        }

        void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            // Increments the savedCounter variable used for generating JPEG file names.
            savedCounter++;
        }

        // Informs when full resolution picture has been taken, saves to local media library and isolated storage.
        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs ce)
        {           
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var bmp = new WriteableBitmap(0, 0).FromStream(ce.ImageStream);
                var resized = bmp.Resize(160, 100, WriteableBitmapExtensions.Interpolation.Bilinear);
                FinalCroppedImage.Source = resized;
                double orgWidth = bmp.PixelWidth;
                double orgHeight = bmp.PixelHeight;

                double disWidth = viewfinderCanvas.Width;
                double disHeight = viewfinderCanvas.Height;

                double widthRatio = orgWidth / disWidth;
                double heightRatio = orgHeight / disHeight;

                int xoffset = (int)(((Point1.X < Point2.X) ? Point1.X : Point2.X) * widthRatio);
                int yoffset = (int)(((Point1.Y < Point2.Y) ? Point1.Y : Point2.X) * heightRatio);
                int width = Math.Abs((int) ((Point1.X-Point2.X)*widthRatio));
                int height = Math.Abs((int) ((Point1.Y-Point2.Y)*heightRatio));
                var croppedBmp = bmp.Crop(xoffset, yoffset, width, height);
                croppedBmp.SaveToMediaLibrary("mycroppedd.jpg");
            });
            
            string fileName = savedCounter + ".jpg";
            try
            {   // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Imagen tomada disponible, guardando";
                });

                // Save picture to the library camera roll.
                library.SavePictureToCameraRoll(fileName, ce.ImageStream);
                               
                // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "Imagen se guardo al rollo de la camara.";
                });

                // Set the position of the stream back to start
                ce.ImageStream.Seek(0, SeekOrigin.Begin);
                
                // Save picture as JPEG to isolated storage.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;                 
                        // Copy the image to isolated storage. 
                        while ((bytesRead = ce.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }

                // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    txtDebug.Text = "La imagen se guarado en almacenamiento aislado.";
                });
            }
            finally
            {
                // Close image stream
                ce.ImageStream.Close();
            }

        }

        // Informs when thumbnail picture has been taken, saves to isolated storage
        // User will select this image in the pictures application to bring up the full-resolution picture. 
        public void cam_CaptureThumbnailAvailable(object sender, ContentReadyEventArgs e)
        {
            string fileName = savedCounter + "_th.jpg";          
            try
            {                

                // Save thumbnail as JPEG to isolated storage.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the thumbnail to isolated storage. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }               
            }
            finally
            {
                // Close image stream
                e.ImageStream.Close();
            }
        }

        //Navigate to see individual cow info
        private void RecogButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            String sCowNumber = numInput.Text;
            int CowNumber = -1;
            if(Int32.TryParse(sCowNumber, out CowNumber))
            {
                if (BDOperations.getCow(CowNumber) == null)
                {
                    MessageBoxResult result = MessageBox.Show("La vaca no existe en el sistema");    
                }
                else
                {
                    NavigationService.Navigate(new Uri("/seeCow.xaml?numero=" + CowNumber, UriKind.Relative));
                }
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Porfavor digite un numero valido en la casilla");                
                System.Diagnostics.Debug.WriteLine("pailas");
            }             
        }

        //When text input box gets focus delete informational text
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }
        
        //Navigate to See all the cows in the database
        private void cowsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/seeCows.xaml", UriKind.Relative));
        }

        private void ConfigurationButton_Click(object sender, RoutedEventArgs e)
        {

        }
       
    }
}