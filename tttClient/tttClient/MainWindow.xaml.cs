using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Windows.Threading;

namespace tttClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Declarations

        private ApiClient _api;
        private DispatcherTimer _dTimer;
        private Grid _dynamicGrid;
        private Game _game;
        private List<SquareButton> _btnList;
        private List<Move> _moves;

        private int _squareSize;
        private int _ticks;
        private bool _updated;

        public int GameId { get; set; }
        public int PlayerId { get; set; }


        #endregion

        #region Methods

        public void CreateBoard(Game game)
        {
            _btnList = new List<SquareButton>();
            _dynamicGrid = new Grid();

            _dynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            _dynamicGrid.VerticalAlignment = VerticalAlignment.Top;

            ColumnDefinition gridCol;
            RowDefinition gridRow;
            SquareButton squareBtn;

            int counter = 0;
            int padding = _squareSize + 2;

            for (int i = 0; i < game.Vsize; i++)
            {
                gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(padding);
                _dynamicGrid.ColumnDefinitions.Add(gridCol);

                for (int j = 0; j < game.Hsize; j++)
                {
                    gridRow = new RowDefinition();
                    gridRow.Height = new GridLength(padding);
                    _dynamicGrid.RowDefinitions.Add(gridRow);
                    squareBtn = CreateButton("", game.Board[counter++], j, i);

                    Grid.SetRow(squareBtn, i);
                    Grid.SetColumn(squareBtn, j);
                    _dynamicGrid.Children.Add(squareBtn);
                    _btnList.Add(squareBtn);
                }
            }
      
            this.grid.Children.Add(_dynamicGrid);
        }

        public SquareButton CreateButton(string text, int playerTurn, int xPos, int yPos)
        {
            var btn = new SquareButton();
            btn.Width = _squareSize;
            btn.Height = _squareSize;
            btn.Content = text;
            btn.X = xPos;
            btn.Y = yPos;
            btn.Player = playerTurn;
            btn.Name = "btn";

            switch (playerTurn)
            {
                case 1:
                    btn.Background = Brushes.RoyalBlue;
                    break;
                case 2:
                    btn.Background = Brushes.Red;
                    break;

                default:
                    btn.Background = Brushes.White;
                    btn.Click += BtnClick;
                    break;
            }

            return btn;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public bool SetGame()
        {
            try
            {
                _game = _api.GetGame(GameId);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void SetTimer()
        {
            _dTimer = new DispatcherTimer();
            _dTimer.Tick += new EventHandler(DispatcherTimerTick);
            _dTimer.Interval = new TimeSpan(0, 0, 0, 1);
            _dTimer.Start();
        }

        public void UpdateUi()
        {
            lbl_players.Content = "Players: " + _game.Players.Count();
            lbl_ticks.Content = "Ticks: " + _ticks;
            CreateBoard(_game);
            DisableSquares();
            
        }

        public bool DisableSquares()
        {
            if (PlayerId != _game.CurrentPlayer)
            {
                foreach (var btn in _btnList)
                    btn.Click -= BtnClick;

                return true;
            }

            return false;
        }

        public int SquareSize(int vSize, int hSize)
        {
            if (vSize <= 5 && hSize <= 5)
                return 40;
            if (vSize <= 7 && hSize <= 7)
                return 25;
            if (vSize <= 10 && hSize <= 10)
                return 20;
            if (vSize <= 15 && hSize <= 15)
                return 15;
            if (vSize <= 20 && hSize <= 20)
                return 10;

            return 20;
        }

        public void Initiate()
        {
            _api = new ApiClient();
            _api.Url = MetaData.Url;

            lbl_gameId.Content = "GameId: " + GameId;
            lbl_playerId.Content = "PlayerId: " + PlayerId;
            rect_player.Fill = PlayerId == 1 ? Brushes.RoyalBlue : Brushes.Red;

            SetTimer();
            SetGame();
            UpdateUi();

            _squareSize = SquareSize(_game.Vsize, _game.Hsize);
        }

        public void CurrentTurnUi()
        {
            if (_game.CurrentPlayer == PlayerId)
            {
                lbl_player.Content = "Your Turn";
                if (PlayerId == 1)
                    lbl_player.Background = Brushes.RoyalBlue;
                else
                    lbl_player.Background = Brushes.Red;

            }
            else
            {
                lbl_player.Content = "Their Turn";
                if (PlayerId == 1)
                    lbl_player.Background = Brushes.Red;
                else
                    lbl_player.Background = Brushes.RoyalBlue;
            }
        }

        public bool CheckWinner(Game game)
        {
            switch (game.Winner)
            {
                case 1:
                    lbl_player.Content = "RED WINS!";
                    lbl_player.Background = Brushes.Red;
                    return true;
                case 2:
                    lbl_player.Content = "BLUE WINS!";
                    lbl_player.Background = Brushes.RoyalBlue;
                    return true;
                default:
                    return false;
            }
        }

        public void ActivateTicks()
        {
            if (PlayerId == _game.CurrentPlayer)
                _ticks--;
            else
                _ticks++;
        }

        #endregion

        #region Events

        private void BtnClick(object sender, RoutedEventArgs e)
        {
            ((SquareButton) sender).Background = _game.CurrentPlayer == 2 ? Brushes.Red : Brushes.RoyalBlue;
            var btn = sender as SquareButton;          
            try
            {
                _api.PostMove(GameId, new Move { X = btn.X, Y = btn.Y, Player = PlayerId });
                lbl_status.Content = "Status: Ok!";
            }
            catch (Exception ex)
            {
                lbl_status.Content = "Status: Invalid move!";
            }
            _updated = false;

        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Initiate();

        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            SetGame();

            if (_game.CurrentPlayer == PlayerId && !_updated)
            {
                UpdateUi();
                _updated = true;
                _ticks++;
            }

            if (!CheckWinner(_game))
                CurrentTurnUi();

        }

        #endregion
    }
}
