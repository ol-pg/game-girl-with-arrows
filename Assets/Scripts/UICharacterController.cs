using UnityEngine.UI;
using UnityEngine;

public class UICharacterController : MonoBehaviour
{
    [SerializeField] private PressedButton left;
    [SerializeField] private PressedButton right;
    [SerializeField] private Button fire;
    [SerializeField] private Button jump;
    public PressedButton Left
    {
        get { return left; }
    }
    public PressedButton Right
    {
        get { return right; }
    }
    public Button Fire
    {
        get { return fire; }
    }
    public Button Jump
    {
        get { return jump; }
    }

    void Start()
    {
        Player.Instance.InitUIController(this);
    }
}
