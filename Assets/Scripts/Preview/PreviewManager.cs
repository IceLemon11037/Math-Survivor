using System; // 引入系统库
using System.Collections; // 引入集合库
using System.Collections.Generic; // 引入泛型集合库
using UnityEngine; // 引入Unity引擎库
using UnityEngine.Experimental.Rendering; // 引入Unity实验性渲染库
using UnityEngine.UI; // 引入Unity UI库

public class PreviewManager : MonoBehaviour // 定义PreviewManager类，继承自MonoBehaviour
{
    //[SerializeField] private ImageManager imageManager; // 序列化字段imageManager，用于管理图像
    [SerializeField] private Camera renderCamera; // 序列化字段renderCamera，用于渲染图像
    //[SerializeField] private ToggleGroup toggleGroup; // 序列化字段toggleGroup，用于管理Toggle组

    [SerializeField] private RawImage renderImg; // 序列化字段renderImg，用于显示渲染图像
    [SerializeField] private RawImage croppedImg; // 序列化字段croppedImg，用于显示裁剪图像
    [SerializeField] private RawImage scaledImg; // 序列化字段scaledImg，用于显示缩放图像

    private Texture2D texture2D_256; // 用于存储256x256的纹理
    private RenderTexture renderTexture; // 用于存储渲染纹理
    private Texture2D croppedTexture; // 用于存储裁剪后的纹理
    private Texture2D scaledTexture; // 用于存储缩放后的纹理

    public Texture2D ScaledTexture => scaledTexture; // 公开的只读属性，返回scaledTexture

    private void Start() // 在脚本启动时调用
    {
        renderTexture = new RenderTexture(256, 256, 16); // 初始化renderTexture为256x256，深度为16
        texture2D_256 = new Texture2D(256, 256, TextureFormat.RGB24, false); // 初始化texture2D_256为256x256，RGB24格式，不生成mipmap

        renderCamera.targetTexture = renderTexture; // 设置渲染相机的目标纹理为renderTexture
        if (renderImg) renderImg.texture = renderTexture; // 如果renderImg不为空，设置其纹理为renderTexture
    }

    public void PreviewImage() // 预览图像
    {
        renderCamera.Render(); // 渲染相机图像

        ShowRenderImage(renderTexture); // 显示渲染图像
        ShowCroppedImage(texture2D_256); // 显示裁剪图像
        ShowScaledImage(croppedTexture); // 显示缩放图像
    }

    /*public void SaveImage() // 保存图像
    {
        renderCamera.Render(); // 渲染相机图像

        ShowCroppedImage(texture2D_256); // 显示裁剪图像
        imageManager.SaveImage(texture2D_256, 0); // 调用imageManager保存图像，编号为0
    }*/

    private void ShowRenderImage(RenderTexture rTexture) // 显示渲染图像
    {
        RenderTexture.active = rTexture; // 将rTexture设置为活动渲染纹理
        Rect rectReadPixels = new Rect(0, 0, rTexture.width, rTexture.height); // 定义读取像素的矩形区域
        texture2D_256.ReadPixels(rectReadPixels, 0, 0); // 从活动渲染纹理中读取像素到texture2D_256
        texture2D_256.Apply(); // 应用读取的像素数据
        if (renderImg) renderImg.texture = texture2D_256; // 如果renderImg不为空，设置其纹理为texture2D_256
        RenderTexture.active = null; // 将活动渲染纹理设置为空
    }

    private void ShowCroppedImage(Texture2D texture) // 显示裁剪图像
    {
        int width = texture.width; // 获取纹理宽度
        int height = texture.height; // 获取纹理高度

        int left = width; // 初始化左边界
        int right = 0; // 初始化右边界
        int bottom = height; // 初始化下边界
        int top = 0; // 初始化上边界
        Color[] pixels = texture.GetPixels(); // 获取纹理像素数据

        bool hasColorPixel = false; // 标记是否存在有颜色的像素
        for (int y = 0; y < height; y++) // 遍历每一行像素
        {
            for (int x = 0; x < width; x++) // 遍历每一列像素
            {
                Color32 pixel = pixels[x + y * width]; // 获取当前像素
                if (pixel.r > 0) // 如果像素的红色通道值大于0
                {
                    hasColorPixel = true; // 标记存在有颜色的像素
                    left = Mathf.Min(left, x); // 更新左边界
                    right = Mathf.Max(right, x); // 更新右边界
                    bottom = Mathf.Min(bottom, y); // 更新下边界
                    top = Mathf.Max(top, y); // 更新上边界
                }
            }
        }

        if (!hasColorPixel) return; // 如果没有有颜色的像素，直接返回

        int newWidth = right - left + 1; // 计算新的宽度
        int newHeight = top - bottom + 1; // 计算新的高度

        croppedTexture = new Texture2D(newWidth, newHeight); // 初始化裁剪纹理
        Color[] croppedPixels = new Color[newWidth * newHeight]; // 初始化裁剪后的像素数组
        for (int y = bottom; y <= top; y++) // 遍历裁剪区域的每一行像素
        {
            for (int x = left; x <= right; x++) // 遍历裁剪区域的每一列像素
            {
                int sourceIndex = x + y * width; // 计算源像素索引
                int targetIndex = (x - left) + (y - bottom) * newWidth; // 计算目标像素索引
                croppedPixels[targetIndex] = pixels[sourceIndex]; // 将源像素复制到目标像素数组
            }
        }
        croppedTexture.SetPixels(croppedPixels); // 设置裁剪纹理的像素数据
        croppedTexture.Apply(); // 应用像素数据

        if (croppedImg) croppedImg.texture = croppedTexture; // 如果croppedImg不为空，设置其纹理为裁剪纹理
    }

    private void ShowScaledImage(Texture2D croppedTexture) // 显示缩放图像
    {
        if (!croppedTexture) return; // 如果裁剪纹理为空，直接返回

        Texture2D originalTexture = croppedTexture; // 原始纹理为裁剪纹理

        int targetSize = 22; // 目标尺寸
        int paddedSize = targetSize + 2 * 3; // 带填充的尺寸

        float scale = Mathf.Min((float)targetSize / originalTexture.width, (float)targetSize / originalTexture.height); // 计算缩放比例

        int newWidth = Mathf.RoundToInt(originalTexture.width * scale); // 计算新的宽度
        int newHeight = Mathf.RoundToInt(originalTexture.height * scale); // 计算新的高度

        scaledTexture = new Texture2D(paddedSize, paddedSize); // 初始化缩放纹理

        int offsetX = (paddedSize - newWidth) / 2; // 计算X轴偏移
        int offsetY = (paddedSize - newHeight) / 2; // 计算Y轴偏移

        for (int y = 0; y < paddedSize; y++) // 遍历每一行像素
        {
            for (int x = 0; x < paddedSize; x++) // 遍历每一列像素
            {
                if (x < offsetX || x >= offsetX + newWidth || y < offsetY || y >= offsetY + newHeight) // 如果像素在填充区域内
                {
                    scaledTexture.SetPixel(x, y, Color.black); // 设置像素为黑色
                }
                else
                {
                    float u = (float)(x - offsetX) / newWidth; // 计算纹理坐标U
                    float v = (float)(y - offsetY) / newHeight; // 计算纹理坐标V
                    scaledTexture.SetPixel(x, y, originalTexture.GetPixelBilinear(u, v)); // 设置像素为双线性插值后的颜色
                }
            }
        }

        scaledTexture.Apply(); // 应用像素数据

        if (scaledImg) scaledImg.texture = scaledTexture; // 如果scaledImg不为空，设置其纹理为缩放纹理
    }
}
