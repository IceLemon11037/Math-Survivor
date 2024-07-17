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

    //百度度0CR客户端
    private Ocr client;

    public Rect screenshotRect;
    public TextMeshProUGUI resultText;
    void Awake()
    {
        //初始化百度0CR客户端
        client = new Baidu.Aip.Ocr.Ocr(apiKey, secretKey);

        //初始化截图矩形区域
        //screenshotRect = new Rect(0, 0, Screen.width, Screen.height);
    }

    void Update()
    {
        //按下空格键进行截图并进行0CR识别
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CaptureScreenshotAndRecognize());
        }
    }

    public void OnButtonClick()
    {
        //截图进行OCR识别
        StartCoroutine(CaptureScreenshotAndRecognize());
    }

    IEnumerator CaptureScreenshotAndRecognize()
    {
        yield return new WaitForEndOfFrame();

        //截图
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(screenshotRect, 0, 8);
        //screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 8);
        screenshot.Apply();
        //将载图保存人临时宴件
        byte[] bytes = screenshot.EncodeToPNG();
        string imagePath = Application.temporaryCachePath + "/screenshot.png";
        File.WriteAllBytes(imagePath, bytes);

        //调用百度0CR接口进行文字识别
        var image = File.ReadAllBytes(imagePath);
        var options = new Dictionary<string, object>
        {
            {"language_type", "CHN_ENG"},
            { "detect direction","true"},
            { "detect language", "true"}
        };
        var result = client.GeneralBasic(image, options);

        //解析识别结果
        string text = "";
        if (result.ContainsKey("words_result"))
        {
            var wordsResult = result["words_result"] as JArray;
            foreach (var word in wordsResult)
            {
                text += word["words"].ToString() + "\n";
            }
                
        }

        //输出识别结果
        Debug.Log("文字识别结果:\n" + text);
        resultText.SetText("Result:" + text);
        //调用百度SDK进行手写文字识别
        try
        {
            //于写文字识别，可能会抛出网络等异常，请使用try/catch捕获
            Debug.Log("开始进行于”文字认别");
            var handwritingResult = client.Handwriting(image); 
            Debug.Log("手写文字认別结果:\n" + handwritingResult);

            //如果有可选参数
            var handwritingOptions = new Dictionary<string, object> { };
            //带参数调用于写文字识别
            handwritingResult= client.Handwriting(image, handwritingOptions);
            Debug.Log("带参数的手写文字识别结果:\n"+ handwritingResult);
            
            // 解析手写文字识别结果
            string handwritingText = "";
            if (handwritingResult.ContainsKey("words_result"))
            {
                var wordsResult = handwritingResult["words result"] as JArray; 
                foreach (var word in wordsResult)
                {
                    handwritingText += word["words"].ToString() + "\n";
                }
            }

            //输出手写文字识别结果
            Debug.Log("手写文字识别结果:\n" + handwritingText);
            resultText.SetText("Result:" + text);
        }

        catch (System.Exception ex)
        {
            //Debug.LogError("手写文字识别发生异常:" + ex.Message);
        }

    }

}
