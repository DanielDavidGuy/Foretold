using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gems : MonoBehaviour
{
    //variables 

    public GameObject gem;
    public bool playerInRange;
    private Player player;
    public bool hasGem = false;

    //list of gem types
    public enum gems
    {
        yellowGem, blueGem, redGem
    }

    public gems gemType;
         
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Player")
        {
            playerInRange = true;
            player = other.GetComponent<Player>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerInRange = false;
        }
    }

    //calls player script of collecting gem
    void CollectGem()
    {
        player.equipGem(this);
    }

    public void DropGem()
    {
        RemoveEffects(player);
        
    }

    //assign gem effects based on type
    public void ApplyEffects(Player player)
    {
        if (gemType == gems.yellowGem)
        {

            player.jumpForce += 10f;
            player.maxSpeed -= 2f;
        }

        else if (gemType == gems.blueGem)
        {
            player.jumpForce -= 2f;
            player.maxSpeed += 6f;
        }

        else if (gemType == gems.redGem)
        {

        }
    }

    //remove effects based on type
    void RemoveEffects(Player player)
    {
        if (gemType == gems.yellowGem)
        {

            player.jumpForce -= 10f;
            player.maxSpeed += 2f;
        }

        else if (gemType == gems.blueGem)
        {
            player.jumpForce += 2f;
            player.maxSpeed -= 6f;
        }

        else if (gemType == gems.redGem)
        {

        }

        transform.position = player.transform.position + new Vector3(2f, 0, 0);
        gameObject.SetActive(true);
        playerInRange = false;
    }

    //checks if "C" key was pressed to collect gem, also rotates from y-axis
    void Update()
    {
        if(playerInRange == true && Keyboard.current.cKey.wasPressedThisFrame)
        {
            CollectGem();
        }

        transform.Rotate(new Vector3(0,45,0) * Time.deltaTime);
    }


    
}
