using UnityEngine;
public class bird_player : MonoBehaviour
{

    public float force = 500.0f;

    public bool FlyEnded = false;



    void Update()
    {   //||(Input.GetTouch(0).phase == TouchPhase.Began)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {

            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, force));


            GetComponent<Animator>().StopPlayback();
            GetComponent<Animator>().Play("birds");
            GetComponent<Animator>().SetBool("Fly", true);
        }

        if (FlyEnded)
        {
            GetComponent<Animator>().SetBool("Fly", false);
            FlyEnded = false;
        }
        Quaternion target = Quaternion.Euler(0, 0, Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.y * 20, -90, 90));
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 10);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Ground" || coll.gameObject.tag == "Destroyer_column")
        {
            GUI_game.Gui_instance.Play.SetActive(false);
            GUI_game.Gui_instance.Pause(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "ScoreUP")
        {
            GUI_game.Gui_instance.UpScore();
            Destroy(other.gameObject);
        }
    }
}
