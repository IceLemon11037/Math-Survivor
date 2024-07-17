using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class MnistTest : MonoBehaviour
{
    public NNModel model; // 存储神经网络模型的对象
    public Texture2D image; // 存储待测试图像的纹理对象
    public PredictionPlot predictionPlot; // 预测结果可视化的组件
    public PreviewManager previewManager; // 预览管理器，用于获取预览的缩放纹理

    private Model runtimeModel; // 运行时加载的神经网络模型
    private IWorker engine; // 神经网络推理引擎

    private float[] predicted; // 存储预测的结果数组
    private bool isProcessing; // 标志位，表示当前是否正在进行推理

    private void Start()
    {
        // 加载模型并创建推理引擎
        runtimeModel = ModelLoader.Load(model);
        engine = WorkerFactory.CreateWorker(runtimeModel);

        // 对输入图像进行推理
        Tensor input = new Tensor(image, 1); // 创建输入张量
        Tensor output = engine.Execute(input).PeekOutput(); // 执行推理并获取输出张量
        input.Dispose(); // 释放输入张量内存
        predicted = output.AsFloats().SoftMax().ToArray(); // 将输出张量转换为浮点数组，并进行 softmax 归一化
        output.Dispose(); // 释放输出张量内存

        // 打印预测结果
        string result = "";
        foreach (float predict in predicted)
        {
            result += predict + ",";
        }
        Debug.Log(result);
    }

    // 在绘制纹理时调用，用于进行推理
    public void OnDrawTexture(Texture texture)
    {
        if (!isProcessing)
        {
            DrawInference(texture); // 调用推理函数
        }
    }

    // 执行推理的函数，接受一个纹理作为输入
    private void DrawInference(Texture texture)
    {
        isProcessing = true; // 设置正在推理的标志为 true
        int channel = 1; // 设置通道数
        Tensor input = new Tensor(texture, channel); // 创建输入张量
        Tensor output = engine.Execute(input).PeekOutput(); // 执行推理并获取输出张量
        input.Dispose(); // 释放输入张量内存
        predicted = output.AsFloats().SoftMax().ToArray(); // 将输出张量转换为浮点数组，并进行 softmax 归一化
        output.Dispose(); // 释放输出张量内存
        predictionPlot.UpdatePlot(predicted); // 更新预测结果的可视化
        isProcessing = false; // 推理结束，设置标志为 false
    }

    // 从预览中执行推理的函数
    public void DrawInferentFromPreview()
    {
        if (!previewManager.ScaledTexture) return; // 如果预览缩放纹理不存在，则返回

        isProcessing = true; // 设置正在推理的标志为 true
        int channel = 1; // 设置通道数
        Tensor input = new Tensor(previewManager.ScaledTexture, channel); // 创建输入张量
        Tensor output = engine.Execute(input).PeekOutput(); // 执行推理并获取输出张量
        input.Dispose(); // 释放输入张量内存
        predicted = output.AsFloats().SoftMax().ToArray(); // 将输出张量转换为浮点数组，并进行 softmax 归一化
        output.Dispose(); // 释放输出张量内存
        predictionPlot.UpdatePlot(predicted); // 更新预测结果的可视化
        isProcessing = false; // 推理结束，设置标志为 false
    }

    // 当对象被销毁时调用，释放推理引擎的资源
    private void OnDestroy()
    {
        engine?.Dispose(); // 释放推理引擎的资源
    }
}
