using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Настройки перемещения")]
    public float panSpeed = 15f; // Скорость движения камеры
    public float panBorderThickness = 10f; // Толщина невидимой рамки у края экрана для мыши
    public Vector2 limitX = new Vector2(-20f, 20f); // Ограничения карты (чтобы камера не улетела в пустоту)
    public Vector2 limitY = new Vector2(-20f, 20f); // Ограничения карты (чтобы камера не улетела в пустоту)

    [Header("Настройки зума")]
    public float scrollSpeed = 2f; // Скорость скролла
    public float minZoom = 3f;     // Максимальное приближение
    public float maxZoom = 10f;    // Максимальное отдаление

    [Header("Перемещение мышью")]
    public bool mouseMove = false; // Перемещение мышью к краю экрана (в тестовых целях выкл)

    private Camera cam;

    void Start()
    {
        // Получаем компонент камеры, на котором висит скрипт
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        Vector3 pos = transform.position;

        // Движение ВПРАВО (Клавиша D, Стрелка Вправо, или мышка у правого края)
        if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow) || (mouseMove? (Input.mousePosition.x >= Screen.width - panBorderThickness) : false))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        // Движение ВЛЕВО (Клавиша A, Стрелка Влево, или мышка у левого края)
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow) || (mouseMove? (Input.mousePosition.x <= panBorderThickness) : false))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // Движение ВВЕРХ (Клавиша W, Стрелка Вверх, или мышка у верхнего края)
        if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow) || (mouseMove? (Input.mousePosition.y >= Screen.height - panBorderThickness) : false))
        {
            pos.y += panSpeed * Time.deltaTime;
        }

        // Движение ВНИЗ (Клавиша S, Стрелка Вниз, или мышка у нижнего края)
        if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow) || (mouseMove? (Input.mousePosition.y <= panBorderThickness) : false))
        {
            pos.y -= panSpeed * Time.deltaTime;
        }


        // Ограничиваем движение камеры по оси 
        pos.x = Mathf.Clamp(pos.x, limitX.x, limitX.y);
        pos.y = Mathf.Clamp(pos.y, limitY.x, limitY.y);

        // Применяем новые координаты
        transform.position = pos;
    }

    private void ZoomCamera()
    {
        // Читаем колесико мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            // Меняем размер ортографической камеры (чем меньше размер, тем ближе)
            cam.orthographicSize -= scroll * scrollSpeed;
            // Ограничиваем зум, чтобы не уйти в минус или не отдалить слишком сильно
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
