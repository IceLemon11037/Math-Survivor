using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class MnistTest : MonoBehaviour
{
    public NNModel model; // �洢������ģ�͵Ķ���
    public Texture2D image; // �洢������ͼ����������
    public PredictionPlot predictionPlot; // Ԥ�������ӻ������
    public PreviewManager previewManager; // Ԥ�������������ڻ�ȡԤ������������

    private Model runtimeModel; // ����ʱ���ص�������ģ��
    private IWorker engine; // ��������������

    private float[] predicted; // �洢Ԥ��Ľ������
    private bool isProcessing; // ��־λ����ʾ��ǰ�Ƿ����ڽ�������

    private void Start()
    {
        // ����ģ�Ͳ�������������
        runtimeModel = ModelLoader.Load(model);
        engine = WorkerFactory.CreateWorker(runtimeModel);

        // ������ͼ���������
        Tensor input = new Tensor(image, 1); // ������������
        Tensor output = engine.Execute(input).PeekOutput(); // ִ��������ȡ�������
        input.Dispose(); // �ͷ����������ڴ�
        predicted = output.AsFloats().SoftMax().ToArray(); // ���������ת��Ϊ�������飬������ softmax ��һ��
        output.Dispose(); // �ͷ���������ڴ�

        // ��ӡԤ����
        string result = "";
        foreach (float predict in predicted)
        {
            result += predict + ",";
        }
        Debug.Log(result);
    }

    // �ڻ�������ʱ���ã����ڽ�������
    public void OnDrawTexture(Texture texture)
    {
        if (!isProcessing)
        {
            DrawInference(texture); // ����������
        }
    }

    // ִ������ĺ���������һ��������Ϊ����
    private void DrawInference(Texture texture)
    {
        isProcessing = true; // ������������ı�־Ϊ true
        int channel = 1; // ����ͨ����
        Tensor input = new Tensor(texture, channel); // ������������
        Tensor output = engine.Execute(input).PeekOutput(); // ִ��������ȡ�������
        input.Dispose(); // �ͷ����������ڴ�
        predicted = output.AsFloats().SoftMax().ToArray(); // ���������ת��Ϊ�������飬������ softmax ��һ��
        output.Dispose(); // �ͷ���������ڴ�
        predictionPlot.UpdatePlot(predicted); // ����Ԥ�����Ŀ��ӻ�
        isProcessing = false; // ������������ñ�־Ϊ false
    }

    // ��Ԥ����ִ������ĺ���
    public void DrawInferentFromPreview()
    {
        if (!previewManager.ScaledTexture) return; // ���Ԥ�������������ڣ��򷵻�

        isProcessing = true; // ������������ı�־Ϊ true
        int channel = 1; // ����ͨ����
        Tensor input = new Tensor(previewManager.ScaledTexture, channel); // ������������
        Tensor output = engine.Execute(input).PeekOutput(); // ִ��������ȡ�������
        input.Dispose(); // �ͷ����������ڴ�
        predicted = output.AsFloats().SoftMax().ToArray(); // ���������ת��Ϊ�������飬������ softmax ��һ��
        output.Dispose(); // �ͷ���������ڴ�
        predictionPlot.UpdatePlot(predicted); // ����Ԥ�����Ŀ��ӻ�
        isProcessing = false; // ������������ñ�־Ϊ false
    }

    // ����������ʱ���ã��ͷ������������Դ
    private void OnDestroy()
    {
        engine?.Dispose(); // �ͷ������������Դ
    }
}
