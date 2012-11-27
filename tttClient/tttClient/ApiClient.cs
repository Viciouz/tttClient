using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace tttClient
{
    public class ApiClient
    {
        private JavaScriptSerializer _jss = new JavaScriptSerializer();
        public string Url { get; set; }

        public List<Game> GetGames()
        {
            WebRequest request = SetupRequest(Url + "/games", "GET", "application/json");
            WebResponse response = request.GetResponse();

            var sr = new StreamReader(response.GetResponseStream());
            string data = sr.ReadToEnd();
            var games = _jss.Deserialize<List<Game>>(data);

            return games;
        }

        public Game GetGame(int id)
        {
            WebRequest request = SetupRequest(Url + "/games/" + id, "GET", "application/json");
            WebResponse response = request.GetResponse();

            var sr = new StreamReader(response.GetResponseStream());
            var jss = new JavaScriptSerializer();
            string data = sr.ReadToEnd();
            sr.Close();
            var game = jss.Deserialize<Game>(data);

            return game;
        }

        public void PostMove(int id, Move move)
        {
            var json = _jss.Serialize(move);
            var enc = Encoding.ASCII;
            var ba = enc.GetBytes(json);

            WebRequest request = SetupRequest(Url + "/games/" + id + "/moves", "POST", "application/json");
            request.ContentLength = ba.Length;

            var stream = request.GetRequestStream();
            
            stream.Write(ba, 0, ba.Length);
            stream.Close();

            var resp = request.GetResponse();
            resp.Close();
        }

        public int PostPlayer(int gameId)
        {
            var json = _jss.Serialize(new { id = "1" });
            var enc = Encoding.ASCII;
            var ba = enc.GetBytes(json);

            WebRequest request = SetupRequest(Url + "/games/" + gameId + "/players", "POST", "application/json");
            request.ContentLength = ba.Length;

            var stream = request.GetRequestStream();
            stream.Write(ba, 0, ba.Length);
            stream.Close();

            var resp = request.GetResponse();
            resp.Close();

            return 1;
        }

        public int PostGame(BoardSize bs)
        {
            var json = _jss.Serialize(bs);
            var enc = Encoding.ASCII;
            var ba = enc.GetBytes(json);

            WebRequest request = SetupRequest(Url + "/games", "POST", "application/json");
            request.ContentLength = ba.Length;

            var stream = request.GetRequestStream();

            stream.Write(ba, 0, ba.Length);
            stream.Close();

            var resp = request.GetResponse();
            resp.Close();

            return 1;
        }

        private WebRequest SetupRequest(string link, string crud, string contenttype)
        {
            var request = WebRequest.Create(link);
            request.ContentType = contenttype;
            request.Method = crud;

            return request;
        }
    }
}
