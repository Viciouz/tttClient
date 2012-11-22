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

namespace tttClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          

        }

        public Game CreateGame()
        {
            const int id = 3, hsize = 5,
                vsize = 5, winner = 0;

            int[] players = new int[3]{ 0, 1, 2 },
                gameboard = new int[9]{
                0,0,0,
                0,0,0,
                0,0,0};

            var game = new Game(id, players, gameboard, hsize, vsize, winner);

            return game;
        }

        public void CreateBoard(Game game)
        {
            Grid dynamicGrid = new Grid();
            dynamicGrid.Width = 400;
            dynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            dynamicGrid.VerticalAlignment = VerticalAlignment.Top;

            ColumnDefinition gridCol;
            RowDefinition gridRow;
            Button btn;
            int counter = 0;


            for (int i = 0; i < game.Hsize; i++)
            {
                gridCol = new ColumnDefinition();
                gridCol.Width = new GridLength(40);
                dynamicGrid.ColumnDefinitions.Add(gridCol);

                for (int j = 0; j < game.Vsize; j++)
                {
                    gridRow = new RowDefinition();
                    gridRow.Height = new GridLength(40);
                    dynamicGrid.RowDefinitions.Add(gridRow);
                    btn = CreateButton("hej");
                   
                    Grid.SetRow(btn, i);
                    Grid.SetColumn(btn, j);
                    dynamicGrid.Children.Add(btn);
                    counter++;

                }
            }

            this.grid.Children.Add(dynamicGrid);
        }

        public Button CreateButton(string text)
        {

            var btn = new Button();
            btn.Width = 35;
            btn.Height = 35;
            btn.Content = text;
            btn.Background = Brushes.White;
            return btn;
        }

        public void GetGame()
        {
            WebRequest request = WebRequest.Create("http://localhost:50563/api/games/1");
        //    request.Headers[HttpResponseHeader.ContentType] = "application/json";
        
                WebResponse response = request.GetResponse();
                
                var sr = new StreamReader(response.GetResponseStream());
                var jss = new JavaScriptSerializer();
                string data = sr.ReadToEnd();
                    var game = jss.Deserialize<Game>(data);
                    CreateBoard(game);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            Debug.Print("Loaded!");
            GetGame();
        }

    }
}
