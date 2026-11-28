using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Gem : MonoBehaviour
{
    //variables 

    public GameObject gem;
    public bool playerInRange;
    protected Player player;
    public bool hasGem = false;
    protected PlayerInput input;
    protected InputAction collectAction;
    
    void Awake()
	{
		input = new PlayerInput();
        collectAction = input.Player.Interact;
    }

	void OnEnable()
	{
		input.Player.Enable();
	}

	void OnDisable()
	{
		input.Player.Disable();
	}
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent<Player>(out Player player))
        {
            playerInRange = true;
            this.player = player;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player)) playerInRange = false;
    }

    //calls player script of collecting gem
    void CollectGem()
    {
        player.equipGem(this);
    }

    public void DropGem()
    {
        RemoveEffects(player);
        transform.position = player.transform.position + new Vector3(2f, 0, 0);
        gameObject.SetActive(true);
        playerInRange = false;
    }

    public abstract void ApplyEffects(Player player);

    protected abstract void RemoveEffects(Player player);

    //checks if "C" key was pressed to collect gem, also rotates from y-axis
    void Update()
    {
        if(playerInRange == true && collectAction.WasPressedThisFrame()) CollectGem();
        transform.Rotate(new Vector3(0,45,0) * Time.deltaTime);
    }


    
}
