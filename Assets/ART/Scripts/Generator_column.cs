using UnityEngine;

public class Generator_column : MonoBehaviour
{

    public float Upper = 6.0f;
    public float Downer = 2.0f;
    public float padding = 7.0f;
    public float rate = 1.0f;
    public moves_column column;
    public GameObject scoreuper;

    // Use this for initialization
    void Start()
    {

        InvokeRepeating("Generate", 0, rate);
    }


    void Generate()
    {

        float offset = Random.Range(Upper, Downer);
        moves_column colom1 = Instantiate(column, new Vector3(transform.position.x, transform.position.y + offset, 0), Quaternion.identity);
        moves_column colom2 = Instantiate(column, new Vector3(transform.position.x, transform.position.y + offset - padding, 0), Quaternion.identity);
        GameObject score = Instantiate(scoreuper, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        score.transform.SetParent(colom2.transform);
    }
}
