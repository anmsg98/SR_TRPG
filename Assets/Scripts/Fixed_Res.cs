using UnityEngine;

public class Fixed : MonoBehaviour
{
    // 해상도 비율 고정 (16:9, 해상도를 벗어나는 부분은 검은색으로 처리)
    void Start()
    {
        var camera = GetComponent<Camera>();        
        var r = camera.rect;
        var scaleheight = ((float)Screen.width / Screen.height) / (16f / 9f);
        var scalewidth = 1f / scaleheight;
        if (scaleheight < 1f)
        {
            r.height = scaleheight;
            r.y = (1f - scaleheight) / 2f;
        }
        else
        {
            r.width = scalewidth;
            r.x = (1f - scalewidth) / 2f;
        }         
        camera.rect = r;
        
    }     
    void OnPreCull() => GL.Clear(true, true, Color.black);
    
}