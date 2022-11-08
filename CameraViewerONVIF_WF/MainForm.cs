using System;
using System.Windows.Forms;
using Ozeki.Camera;
using Ozeki.Media;

namespace Camera_Viewer_Connect_ONVIF_WF
{
    public partial class MainForm : Form
    {
        private OzekiCamera _camera;
        private DrawingImageProvider _imageProvider;
        private MediaConnector _connector;
        private CameraURLBuilderWF _myCameraUrlBuilder;
        //var url = "192.168.1.40:80;Username = admin;Password = admin;Transport = UDP; ;
        public MainForm()
        {
            InitializeComponent();

            _connector = new MediaConnector();
            _imageProvider = new DrawingImageProvider();
            // Create video viewer UI control
            _myCameraUrlBuilder = new CameraURLBuilderWF();
            // Bind the camera image to the UI control
            videoViewerWF1.SetImageProvider(_imageProvider);
        }

        // Connect camera video channel to image provider and start
        //private void connectBtn_Click(object sender, EventArgs e)
        //{
        //    var result = _myCameraUrlBuilder.ShowDialog();

        //    //if (result == DialogResult.OK)
        //   // {
        //       // tb_cameraUrl.Text = _myCameraUrlBuilder.CameraURL;

        //        btn_Connect.Enabled = true;
        //   // }
        //}

        private void btn_Connect_Click(object sender, EventArgs e)
        {
           // tb_cameraUrl.Text = "192.168.1.40:80;Username = admin;Password = admin;Transport = UDP;" ;
            if (_camera != null)
            {
                _camera.CameraStateChanged -= _camera_CameraStateChanged;
                _camera.Disconnect();
                _connector.Disconnect(_camera.VideoChannel, _imageProvider);
                _camera.Dispose();
                _camera = null;
            }

            btn_Connect.Enabled = false;

            var url = "192.168.1.40:80;Username = admin;Password = admin;Transport = UDP;";

            _camera = new OzekiCamera(url);
      
            _camera.CameraStateChanged += _camera_CameraStateChanged;
        
            _connector.Connect(_camera.VideoChannel, _imageProvider);

            _camera.Start();
            videoViewerWF1.Start();
        }

        void _camera_CameraStateChanged(object sender, CameraStateEventArgs e)
        {
            InvokeThread(() =>
            {
                StateChanged(e.State);

                if(e.State == CameraState.Streaming)
                    Streaming();

                if(e.State == CameraState.Disconnected)
                    Disconnect();
            });
        }

        private void StateChanged(CameraState state)
        {
            statusLabel.Text = state.ToString();
        }

        private void Disconnect()
        {
            btn_Connect.Enabled = true;
            btn_Disconnect.Enabled = false;
        }

        private void Streaming()
        {
            btn_Disconnect.Enabled = true;
        }

        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            if (_camera == null) return;

            _camera.Disconnect();
            _connector.Disconnect(_camera.VideoChannel, _imageProvider);
            _camera = null;
        }

        void InvokeThread(Action action)
        {
            BeginInvoke(action);
        }
    }
}