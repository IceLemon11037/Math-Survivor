using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using Baidu.Aip.Ocr;
using Newtonsoft.Json.Linq;
using TMPro;

public class HandwritingRecognition : MonoBehaviour
{
    private string apiKey = "3izYFLybulST57Vk2iK82hwD";
    private string secretKey = "7rBNQm2UbruLGipindER3ufomWHEOwfW";

    //�ٶȶ�0CR�ͻ���
    private Ocr client;

    public Rect screenshotRect;
    public TextMeshProUGUI resultText;
    void Awake()
    {
        //��ʼ���ٶ�0CR�ͻ���
        client = new Baidu.Aip.Ocr.Ocr(apiKey, secretKey);

        //��ʼ����ͼ��������
        //screenshotRect = new Rect(0, 0, Screen.width, Screen.height);
    }

    void Update()
    {
        //���¿ո�����н�ͼ������0CRʶ��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CaptureScreenshotAndRecognize());
        }
    }

    public void OnButtonClick()
    {
        //��ͼ����OCRʶ��
        StartCoroutine(CaptureScreenshotAndRecognize());
    }

    IEnumerator CaptureScreenshotAndRecognize()
    {
        yield return new WaitForEndOfFrame();

        //��ͼ
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(screenshotRect, 0, 8);
        //screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 8);
        screenshot.Apply();
        //����ͼ��������ʱ���
        byte[] bytes = screenshot.EncodeToPNG();
        string imagePath = Application.temporaryCachePath + "/screenshot.png";
        File.WriteAllBytes(imagePath, bytes);

        //���ðٶ�0CR�ӿڽ�������ʶ��
        var image = File.ReadAllBytes(imagePath);
        var options = new Dictionary<string, object>
        {
            {"language_type", "CHN_ENG"},
            { "detect direction","true"},
            { "detect language", "true"}
        };
        var result = client.GeneralBasic(image, options);

        //����ʶ����
        string text = "";
        if (result.ContainsKey("words_result"))
        {
            var wordsResult = result["words_result"] as JArray;
            foreach (var word in wordsResult)
            {
                text += word["words"].ToString() + "\n";
            }
                
        }

        //���ʶ����
        Debug.Log("����ʶ����:\n" + text);
        resultText.SetText("Result:" + text);
        //���ðٶ�SDK������д����ʶ��
        try
        {
            //��д����ʶ�𣬿��ܻ��׳�������쳣����ʹ��try/catch����
            Debug.Log("��ʼ�����ڡ������ϱ�");
            var handwritingResult = client.Handwriting(image); 
            Debug.Log("��д�����τe���:\n" + handwritingResult);

            //����п�ѡ����
            var handwritingOptions = new Dictionary<string, object> { };
            //������������д����ʶ��
            handwritingResult= client.Handwriting(image, handwritingOptions);
            Debug.Log("����������д����ʶ����:\n"+ handwritingResult);
            
            // ������д����ʶ����
            string handwritingText = "";
            if (handwritingResult.ContainsKey("words_result"))
            {
                var wordsResult = handwritingResult["words result"] as JArray; 
                foreach (var word in wordsResult)
                {
                    handwritingText += word["words"].ToString() + "\n";
                }
            }

            //�����д����ʶ����
            Debug.Log("��д����ʶ����:\n" + handwritingText);
            resultText.SetText("Result:" + text);
        }

        catch (System.Exception ex)
        {
            //Debug.LogError("��д����ʶ�����쳣:" + ex.Message);
        }

    }

}
