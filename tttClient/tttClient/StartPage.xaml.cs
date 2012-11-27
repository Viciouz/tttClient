using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace tttClient
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    partial class StartPage : Window
    {
        private ApiClient _api;
        private volatile bool _valid;


        public StartPage()
        {
            InitializeComponent();
            EnableUi(false);
        }

        #region Methods

        public bool ValidateBoardSize()
        {
            int nr1, nr2;
            bool hsize = int.TryParse(txt_hSize.Text, out nr1);
            bool vsize = int.TryParse(txt_vSize.Text, out nr2);

            if (!vsize)
                txt_vSize.Text = "Invalid integer";
            if (!hsize)
                txt_hSize.Text = "Invalid integer";

            return hsize && vsize && nr1 < 20 && nr2 < 20;
        }

        public List<string> GetGames()
        {
            var games = _api.GetGames();
            var list = new List<string>();
            var row = "";

            foreach (var g in games)
            {
                row = string.Format("id: {0} players: {1}", g.Id, g.Players.Count());
                list.Add(row);
            }
            return list;
        }

        private bool CheckUrl(string url)
        {
            List<Game> check = null;

            try
            {
                check = _api.GetGames();
            }
            catch (Exception)
            {
                return false;
            }

            if (check != null)
                return true;

            return false;
        }

        private void EnableUi(bool enable)
        {

            btn_create.IsEnabled = enable;
            btn_join.IsEnabled = enable;
            txt_hSize.IsEnabled = enable;
            txt_vSize.IsEnabled = enable;
            lb_games.IsEnabled = enable;
            cb_default.IsEnabled = enable;
        }

        #endregion

        #region Events

        private void BtnJoinClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _api.PostPlayer(lb_games.SelectedIndex + 1);
                var mainWindow = new MainWindow { GameId = lb_games.SelectedIndex + 1, PlayerId = 2 };
                mainWindow.Show();
            }
            catch (Exception)
            {
                btn_join.Content = "Error";
            }
           
        }

        private void BtnCreateClick(object sender, RoutedEventArgs e)
        {
            if (ValidateBoardSize())
            {
                int vSize = int.Parse(txt_vSize.Text);
                int hSize = int.Parse(txt_hSize.Text);

                var bs = new BoardSize { Width = vSize, Height = hSize };
                _api.PostGame(bs);
                lb_games.ItemsSource = GetGames();
                btn_create.Content = "created";
                var mainWindow = new MainWindow { GameId = lb_games.Items.Count, PlayerId = 1 };

                mainWindow.Show();
            }
            else
            {
                btn_create.Content = "not created";
            }
        }

        private void CbDefaultChecked(object sender, RoutedEventArgs e)
        {
            txt_hSize.IsEnabled = false;
            txt_vSize.IsEnabled = false;
            txt_url.IsEnabled = false;

            txt_url.Text = "http://localhost:50563/api";
            txt_hSize.Text = "5";
            txt_vSize.Text = "5";
        }

        private void CbDefaultUnchecked(object sender, RoutedEventArgs e)
        {
            txt_hSize.IsEnabled = true;
            txt_vSize.IsEnabled = true;
            txt_url.IsEnabled = true;

            txt_hSize.Text = "vertical size";
            txt_vSize.Text = "horizontal size";
        }

        private void BtnConnectClick(object sender, RoutedEventArgs e)
        {
            _api = new ApiClient { Url = txt_url.Text };
            MetaData.Url = txt_url.Text;
            btn_connect.Content = "loading";
            btn_connect.Background = Brushes.Yellow;
            EnableUi(false);

            var bw = new BackgroundWorker();
            bw.DoWork += ValidateUri;
            bw.RunWorkerCompleted += Completed;

            bw.RunWorkerAsync();
        }

        private void Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_valid)
            {
                btn_connect.Content = "connected";
                btn_connect.Background = Brushes.YellowGreen;
                lb_games.ItemsSource = GetGames();
                EnableUi(true);
            }
            else
            {
                btn_connect.Content = "not connected";
                btn_connect.Background = Brushes.Tomato;
                lb_games.ItemsSource = null;
            }
        }

        private void ValidateUri(object sender, DoWorkEventArgs e)
        {
            _valid = CheckUrl(_api.Url);
        }

        #endregion

    }
}
