using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    public Image imageComponent;

    public void SelectImage()
    {
        // 이미지 파일을 선택하는 파일 다이얼로그를 엽니다.
        string imagePath = UnityEditor.EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");

        // 이미지 파일이 선택되지 않은 경우 리턴합니다.
        if (string.IsNullOrEmpty(imagePath))
            return;

        // 선택한 이미지 파일을 Texture2D로 로드합니다.
        Texture2D imageTexture = new Texture2D(2, 2); // 더미 텍스쳐를 생성합니다.
        byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
        imageTexture.LoadImage(imageData);

        // 로드한 이미지를 Image 컴포넌트에 설정합니다.
        imageComponent.sprite = Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), Vector2.zero);
    }
}
