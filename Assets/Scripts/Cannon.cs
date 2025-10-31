using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

public class Cannon : MonoBehaviour, IPlayerActions
{
    [SerializeField] private float _multiplier = 10f;
    [SerializeField] private float _maxRotation;
    [SerializeField] private float _minRotation;
    [SerializeField] private float _maxSpeed;
    
    private float _offset = -51.6f;
    private float _angle;
    private Vector2 _mousePosition;

    [SerializeField] private GameObject _shootPoint;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject PotencyBar;

    private float _initialScaleX;
    private float _projectileSpeed = 0;
    private bool _isRaising = false;
    private Vector2 _direction;
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.SetCallbacks(this);
        _initialScaleX = PotencyBar.transform.localScale.x;
    }

    private void Start() => inputActions.Enable();
    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    void Update()
    {   
        _direction = _mousePosition - new Vector2(transform.position.normalized.x, transform.position.normalized.y);
        var newAngle = (Mathf.Atan2(_direction.y, _direction.x) * 180f / Mathf.PI + _offset);

        if (newAngle >= _minRotation && newAngle <= _maxRotation)
        {
            _angle = newAngle;
            transform.rotation = Quaternion.Euler(0, 0, _angle);
        }

        if (_isRaising)
        {
            if (_projectileSpeed <= _maxSpeed)
            {
                _projectileSpeed = Time.deltaTime * _multiplier + _projectileSpeed;
            }
            CalculateBarScale();
        }
    }

    private void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, _projectileSpeed / _maxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.started) _isRaising = true;
        if (context.canceled)
        {
            var bullet = Instantiate(_bullet, _shootPoint.transform.position, Quaternion.identity);

            var velocity = new Vector2(_projectileSpeed * _direction.normalized.x, _projectileSpeed * _direction.normalized.y);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = velocity;

            _projectileSpeed = 0f;
            _isRaising = false;
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
            _mousePosition.Normalize();
        }
    }
}
