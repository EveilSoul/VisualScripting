using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Linq;

public class ContinueGenerator : MonoBehaviour
{
    public static string Token = "1707297164:AAHBYPZfgng19YyaS86YIV-UWau563Q2u5U";
    public int ID = -1;

    public Text Content;
    public Slider Slider;


    public void OnGenerateClick()
    {
        Content.text = "";
        if (ID == -1)
        {
            NodeData.GetMyNodeData(ref ID, transform);
        }
        var text = GraphController.GetAllNodeTextById(ID);
        prevText = text;

        SendRequest(text);
        StartCoroutine(ReceiveAnswer());

        //var Bot = new TelegramBotClient("1707297164:AAH_83Ig6zQ2k0y1fkusIbjO8DrbjIRToxg"); // инициализируем API
        //Bot.SetWebhookAsync("");
        //var updates = Bot.GetUpdatesAsync();
        //resText = updates.Result.Last().ChannelPost.Text;
        //Content.text = resText;
    }

    public void OnSaveClick()
    {
        var nodeTransform = GraphController.Nodes[ID].GetComponent<RectTransform>();
        var offset = new Vector2(175, 0);
        var node = GraphController.CreateNodeWithText(nodeTransform.anchoredPosition + offset, resText.Substring(prevText.Length - 1));

        ConnectionManager.ForceConnection(GraphController.Nodes[ID], node.GetComponent<Node>());
    }

    private string resText;
    private string prevText;

    private void SendRequest(string text)
    {
        string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
        //string apiToken = "1707297164:AAHzY0X75MA0YlUum4s3pFcxsElmBwsnBf4";
        string chatId = "@Chachane";
        urlString = string.Format(urlString, Token, chatId, text);
        WebRequest request = WebRequest.Create(urlString);
        Stream rs = request.GetResponse().GetResponseStream();
        StreamReader reader = new StreamReader(rs);
        string line = "";
        StringBuilder sb = new StringBuilder();
        while (line != null)
        {
            line = reader.ReadLine();
            if (line != null)
                sb.Append(line);
        }
    }

    IEnumerator ReceiveAnswer()
    {
        yield return StartCoroutine(ReceiveAnswerCoroutine());

        var newText = prevText.Substring(0, prevText.Length - 1) + "<b>" + resText.Substring(prevText.Length - 1) + "</b>" ;
        Content.text = newText;
    }

    IEnumerator ReceiveAnswerCoroutine()
    {
        var Bot = new TelegramBotClient(Token); // инициализируем API
        Bot.SetWebhookAsync("");
        var time = 4;
        Telegram.Bot.Types.Update last = null;
        int offset = 0;

        
        yield return StartCoroutine(StartWaiting(time));


        var i = 0;
        do
        {
            var updates = Bot.GetUpdatesAsync(offset:offset);
            var res = updates.Result;

            if (res.Length == 0)
                break;

            last = res.Last();
            offset = last.Id + 1;
        } while (true);

        

        resText = last.ChannelPost.Text;
        //while (true)
        //{
        //    var updates = Bot.GetUpdatesAsync(); // получаем массив обновлений
        //    //updates.Result.Last().ChannelPost.Text;
        //    foreach (var update in updates.Result) // Перебираем все обновления
        //    {
        //        resText = updates.Result.Last().ChannelPost.Text;
        //        yield break;
        //    }
        //    yield return null;
        //}
    }

    IEnumerator StartWaiting(float time)
    {
        Slider.value = 0;
        var step = 0.1f / time;
        
        while (Slider.value < 1)
        {
            Slider.value += step;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
