using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Telegram.Bot;

public class TextGenerator : MonoBehaviour
{
    private static string resText;

    public static string GenerateText(string text)
    {
        string urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
        string apiToken = "1707297164:AAH_83Ig6zQ2k0y1fkusIbjO8DrbjIRToxg";
        string chatId = "@Chachane";
        urlString = string.Format(urlString, apiToken, chatId, text);
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

        var res = ReceiveAnswer();
        return res.Result;
    }

    static async Task<string> ReceiveAnswer()
    {

        var Bot = new TelegramBotClient("1707297164:AAH_83Ig6zQ2k0y1fkusIbjO8DrbjIRToxg"); // �������������� API
        await Bot.SetWebhookAsync("");
        //Bot.SetWebhook(""); // �����������! ������� ������ �������� � ������� ��� ����
        while (true)
        {
            var updates = await Bot.GetUpdatesAsync(); // �������� ������ ����������

            foreach (var update in updates) // ���������� ��� ����������
            {
                return update.ChannelPost.Text;
            }

        }
    }

    IEnumerator StartReceiveCoroutine(TelegramBotClient botClient)
    {
        yield return StartCoroutine(ReceiveAnswerCoroutine(botClient));

    }

    IEnumerator ReceiveAnswerCoroutine(TelegramBotClient botClient)
    {
        while (true)
        {
            var updates = botClient.GetUpdatesAsync(); // �������� ������ ����������

            foreach (var update in updates.Result) // ���������� ��� ����������
            {
                resText = update.ChannelPost.Text;
                yield break;
            }
            yield return null;
        }
    }
}
