using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effect/TextSpacing")]
[RequireComponent(typeof(UnityEngine.UI.Text))]//Text组件是必须的
public class TextSpacing : BaseMeshEffect
{
    public enum HorizontalAligmentType
    {
        Left,
        Center,
        Right
    }

    public float fSpacing = 1.0f;
    public override void ModifyMesh(VertexHelper vh)
    {
        //一个文字有4个顶点  4个顶点组成两个三角面  如果顶点少于4个 说明text内容为空 就没必要去做字间距调整了
        if (!IsActive() || vh.currentVertCount < 4 || fSpacing == 0)
        {
            return;
        }

        Text cText = transform.GetComponent<Text>();

        // 对齐方式，对于不同的对齐方式 对文字顶点的坐标偏移值计算方式不同
        HorizontalAligmentType alignment;
        if (cText.alignment == TextAnchor.LowerLeft || cText.alignment == TextAnchor.MiddleLeft || cText.alignment == TextAnchor.UpperLeft)
        {
            alignment = HorizontalAligmentType.Left;
        }
        else if (cText.alignment == TextAnchor.LowerCenter || cText.alignment == TextAnchor.MiddleCenter || cText.alignment == TextAnchor.UpperCenter)
        {
            alignment = HorizontalAligmentType.Center;
        }
        else
        {
            alignment = HorizontalAligmentType.Right;
        }

        int nWordCount = vh.currentVertCount / 4;//总字数
        //总的字间距偏移值
        float fTotalSpace = (nWordCount - 1) * fSpacing;
        float fOffsetX = 0.0f;
        UIVertex vertex = new UIVertex();
        for (int index = 0; index < nWordCount; index++)
        {
            if (alignment == HorizontalAligmentType.Left)
            {
                //左对齐的话 相当于对于每个字 坐标往右偏移对应字间距值  第一个字不用偏移 
                fOffsetX = index * fSpacing;
            }
            else if (alignment == HorizontalAligmentType.Right)
            {
                //和左对齐类似
                fOffsetX = index * fSpacing - fTotalSpace;
            }
            else if (alignment == HorizontalAligmentType.Center)
            {
                //居中对齐 类似左对齐之后 整体往左偏移总字间距的50% 那么在计算的时候直接减50%总字间距就好了
                fOffsetX = index * fSpacing - fTotalSpace / 2;
            }
            for (int pIndex = 0; pIndex < 4; pIndex++)
            {
                int nVerticeIndex = index * 4 + pIndex;//对应索引
                vh.PopulateUIVertex(ref vertex, nVerticeIndex);//取出对应对应索引的顶点
                vertex.position += new Vector3(fOffsetX, 0, 0);//顶点偏移
                vh.SetUIVertex(vertex, nVerticeIndex);//更新下顶点坐标信息
            }
        }
    }
}