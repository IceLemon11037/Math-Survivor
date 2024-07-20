using System; // ����ϵͳ��
using System.Collections; // ���뼯�Ͽ�
using System.Collections.Generic; // ���뷺�ͼ��Ͽ�
using UnityEngine; // ����Unity�����
using UnityEngine.Experimental.Rendering; // ����Unityʵ������Ⱦ��
using UnityEngine.UI; // ����Unity UI��

public class PreviewManager : MonoBehaviour // ����PreviewManager�࣬�̳���MonoBehaviour
{
    //[SerializeField] private ImageManager imageManager; // ���л��ֶ�imageManager�����ڹ���ͼ��
    [SerializeField] private Camera renderCamera; // ���л��ֶ�renderCamera��������Ⱦͼ��
    //[SerializeField] private ToggleGroup toggleGroup; // ���л��ֶ�toggleGroup�����ڹ���Toggle��

    [SerializeField] private RawImage renderImg; // ���л��ֶ�renderImg��������ʾ��Ⱦͼ��
    [SerializeField] private RawImage croppedImg; // ���л��ֶ�croppedImg��������ʾ�ü�ͼ��
    [SerializeField] private RawImage scaledImg; // ���л��ֶ�scaledImg��������ʾ����ͼ��

    private Texture2D texture2D_256; // ���ڴ洢256x256������
    private RenderTexture renderTexture; // ���ڴ洢��Ⱦ����
    private Texture2D croppedTexture; // ���ڴ洢�ü��������
    private Texture2D scaledTexture; // ���ڴ洢���ź������

    public Texture2D ScaledTexture => scaledTexture; // ������ֻ�����ԣ�����scaledTexture

    private void Start() // �ڽű�����ʱ����
    {
        renderTexture = new RenderTexture(256, 256, 16); // ��ʼ��renderTextureΪ256x256�����Ϊ16
        texture2D_256 = new Texture2D(256, 256, TextureFormat.RGB24, false); // ��ʼ��texture2D_256Ϊ256x256��RGB24��ʽ��������mipmap

        renderCamera.targetTexture = renderTexture; // ������Ⱦ�����Ŀ������ΪrenderTexture
        if (renderImg) renderImg.texture = renderTexture; // ���renderImg��Ϊ�գ�����������ΪrenderTexture
    }

    public void PreviewImage() // Ԥ��ͼ��
    {
        renderCamera.Render(); // ��Ⱦ���ͼ��

        ShowRenderImage(renderTexture); // ��ʾ��Ⱦͼ��
        ShowCroppedImage(texture2D_256); // ��ʾ�ü�ͼ��
        ShowScaledImage(croppedTexture); // ��ʾ����ͼ��
    }

    /*public void SaveImage() // ����ͼ��
    {
        renderCamera.Render(); // ��Ⱦ���ͼ��

        ShowCroppedImage(texture2D_256); // ��ʾ�ü�ͼ��
        imageManager.SaveImage(texture2D_256, 0); // ����imageManager����ͼ�񣬱��Ϊ0
    }*/

    private void ShowRenderImage(RenderTexture rTexture) // ��ʾ��Ⱦͼ��
    {
        RenderTexture.active = rTexture; // ��rTexture����Ϊ���Ⱦ����
        Rect rectReadPixels = new Rect(0, 0, rTexture.width, rTexture.height); // �����ȡ���صľ�������
        texture2D_256.ReadPixels(rectReadPixels, 0, 0); // �ӻ��Ⱦ�����ж�ȡ���ص�texture2D_256
        texture2D_256.Apply(); // Ӧ�ö�ȡ����������
        if (renderImg) renderImg.texture = texture2D_256; // ���renderImg��Ϊ�գ�����������Ϊtexture2D_256
        RenderTexture.active = null; // �����Ⱦ��������Ϊ��
    }

    private void ShowCroppedImage(Texture2D texture) // ��ʾ�ü�ͼ��
    {
        int width = texture.width; // ��ȡ������
        int height = texture.height; // ��ȡ����߶�

        int left = width; // ��ʼ����߽�
        int right = 0; // ��ʼ���ұ߽�
        int bottom = height; // ��ʼ���±߽�
        int top = 0; // ��ʼ���ϱ߽�
        Color[] pixels = texture.GetPixels(); // ��ȡ������������

        bool hasColorPixel = false; // ����Ƿ��������ɫ������
        for (int y = 0; y < height; y++) // ����ÿһ������
        {
            for (int x = 0; x < width; x++) // ����ÿһ������
            {
                Color32 pixel = pixels[x + y * width]; // ��ȡ��ǰ����
                if (pixel.r > 0) // ������صĺ�ɫͨ��ֵ����0
                {
                    hasColorPixel = true; // ��Ǵ�������ɫ������
                    left = Mathf.Min(left, x); // ������߽�
                    right = Mathf.Max(right, x); // �����ұ߽�
                    bottom = Mathf.Min(bottom, y); // �����±߽�
                    top = Mathf.Max(top, y); // �����ϱ߽�
                }
            }
        }

        if (!hasColorPixel) return; // ���û������ɫ�����أ�ֱ�ӷ���

        int newWidth = right - left + 1; // �����µĿ��
        int newHeight = top - bottom + 1; // �����µĸ߶�

        croppedTexture = new Texture2D(newWidth, newHeight); // ��ʼ���ü�����
        Color[] croppedPixels = new Color[newWidth * newHeight]; // ��ʼ���ü������������
        for (int y = bottom; y <= top; y++) // �����ü������ÿһ������
        {
            for (int x = left; x <= right; x++) // �����ü������ÿһ������
            {
                int sourceIndex = x + y * width; // ����Դ��������
                int targetIndex = (x - left) + (y - bottom) * newWidth; // ����Ŀ����������
                croppedPixels[targetIndex] = pixels[sourceIndex]; // ��Դ���ظ��Ƶ�Ŀ����������
            }
        }
        croppedTexture.SetPixels(croppedPixels); // ���òü��������������
        croppedTexture.Apply(); // Ӧ����������

        if (croppedImg) croppedImg.texture = croppedTexture; // ���croppedImg��Ϊ�գ�����������Ϊ�ü�����
    }

    private void ShowScaledImage(Texture2D croppedTexture) // ��ʾ����ͼ��
    {
        if (!croppedTexture) return; // ����ü�����Ϊ�գ�ֱ�ӷ���

        Texture2D originalTexture = croppedTexture; // ԭʼ����Ϊ�ü�����

        int targetSize = 22; // Ŀ��ߴ�
        int paddedSize = targetSize + 2 * 3; // �����ĳߴ�

        float scale = Mathf.Min((float)targetSize / originalTexture.width, (float)targetSize / originalTexture.height); // �������ű���

        int newWidth = Mathf.RoundToInt(originalTexture.width * scale); // �����µĿ��
        int newHeight = Mathf.RoundToInt(originalTexture.height * scale); // �����µĸ߶�

        scaledTexture = new Texture2D(paddedSize, paddedSize); // ��ʼ����������

        int offsetX = (paddedSize - newWidth) / 2; // ����X��ƫ��
        int offsetY = (paddedSize - newHeight) / 2; // ����Y��ƫ��

        for (int y = 0; y < paddedSize; y++) // ����ÿһ������
        {
            for (int x = 0; x < paddedSize; x++) // ����ÿһ������
            {
                if (x < offsetX || x >= offsetX + newWidth || y < offsetY || y >= offsetY + newHeight) // ������������������
                {
                    scaledTexture.SetPixel(x, y, Color.black); // ��������Ϊ��ɫ
                }
                else
                {
                    float u = (float)(x - offsetX) / newWidth; // ������������U
                    float v = (float)(y - offsetY) / newHeight; // ������������V
                    scaledTexture.SetPixel(x, y, originalTexture.GetPixelBilinear(u, v)); // ��������Ϊ˫���Բ�ֵ�����ɫ
                }
            }
        }

        scaledTexture.Apply(); // Ӧ����������

        if (scaledImg) scaledImg.texture = scaledTexture; // ���scaledImg��Ϊ�գ�����������Ϊ��������
    }
}
