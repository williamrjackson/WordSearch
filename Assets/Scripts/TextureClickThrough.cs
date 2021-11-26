using UnityEngine;
using UnityEngine.EventSystems;

public class TextureClickThrough : MonoBehaviour
{
    [SerializeField] protected RectTransform RawImageRectTrans;
    [SerializeField] protected Camera RenderToTextureCamera;

    public Vector3 RelativeMouse
    {
        get
        {
            var mousePosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RawImageRectTrans, mousePosition, null, out Vector2 localPoint);
            Vector2 normalizedPoint = Rect.PointToNormalized(RawImageRectTrans.rect, localPoint);
            var renderRay = RenderToTextureCamera.ViewportToWorldPoint(normalizedPoint);
            return renderRay;
        }
    }

    private void Update()
    {
        var mousePosition = Input.mousePosition;
        if (RectTransformUtility.RectangleContainsScreenPoint(RawImageRectTrans, mousePosition.ToVector2()))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RawImageRectTrans, mousePosition, null, out localPoint);
            Vector2 normalizedPoint = Rect.PointToNormalized(RawImageRectTrans.rect, localPoint);
            var renderRay = RenderToTextureCamera.ViewportPointToRay(normalizedPoint);

            if (Physics.Raycast(renderRay, out var raycastHit))
            {
                var overLetter = raycastHit.collider.gameObject.GetComponent<LetterUnit>();
                if (overLetter != null)
                {
                    overLetter.MouseOver();
                }
            }
        }
        else
        {
            LetterUnit.MouseExit();
        }
    }
}