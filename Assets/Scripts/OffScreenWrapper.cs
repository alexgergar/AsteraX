using UnityEngine;


public class OffScreenWrapper : MonoBehaviour
{
    bool isWrappingX = false;
    bool isWrappingY = false;

    private Collider objectCollider;

    private float halfObjectWidth;
    private float halfObjectHeight;


    private void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            objectCollider = gameObject.AddComponent<Collider>();
        }
        objectCollider = GetComponent<Collider>();

        // only need half of h/w bc normally exits normally at center of object 
        halfObjectWidth = objectCollider.bounds.size.x / 2;
        halfObjectHeight = objectCollider.bounds.size.y / 2;


    }

    private void Update()
    {
        ScreenWrap();
    }


    private void ScreenWrap()
    {
        if (!enabled) return;

        bool isOutOfBounds = ScreenBounds.OutOfBounds(new Vector2(Mathf.Abs(transform.position.x) - halfObjectWidth, Mathf.Abs(transform.position.y) - halfObjectHeight));

        // if object is visable then no need to screen wrapp
        if (!isOutOfBounds)
        {
            isWrappingX = false;
            isWrappingY = false;
            return;
        }

        // if already true, no need to screen wrap
        if (isWrappingX && isWrappingY)
        {
            return;
        }

        var cam = Camera.main;
        Vector3 newPosition = transform.position;

        bool isXOutOfBounds = ScreenBounds.IsXOutOfBounds(new Vector2(transform.position.x + halfObjectWidth, transform.position.y + halfObjectHeight));
        bool IsYOutOfBounds = ScreenBounds.IsYOutOfBounds(new Vector2(transform.position.x + halfObjectWidth, transform.position.y + halfObjectHeight));

        // if not wrapped already or off screen and x is out of bounds
        if (!isWrappingX && isXOutOfBounds)
        {
            // cam position is x=0, y=0 and thus scene is laid out like mirror
            // everything to left is neg coords, right is positive coords
            // so can just add - to reverse the position
            newPosition.x = -newPosition.x;

            // this helps if there is lag of mirroring the position or if switched multiple times
            isWrappingX = true;
        }

        if (!isWrappingY && IsYOutOfBounds)
        {
            newPosition.y = -newPosition.y;
            isWrappingY = true;
        }

        transform.position = newPosition;

    }
}
