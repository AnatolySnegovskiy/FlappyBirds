using UnityEngine;

public class moves_column : MonoBehaviour {
    public float speed=2.0f;
    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(-speed, 0));
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        // Убрать препятствие при столкновении с границей
        if (coll.gameObject.tag == "Destroyer_column")
        {
            Destroy(gameObject);
        }
    }

}
