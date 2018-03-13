using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour {

    public GameObject paddle;
    public GameObject ball;
    Rigidbody2D brb;
    float yvel;                 //the variable that will be assigned by the neural network
    float paddleMinY = 8.8f;    //max paddle y position
    float paddleMaxY = 17.4f;   //minimum paddle y position
    float paddleMaxSpeed = 15;
    public float numSaved = 0;
    public float numMissed = 0;

    ANN ann;

	// Use this for initialization
	void Start () {
        ann = new ANN(6, 1, 1, 4, 0.01);
        brb = ball.GetComponent<Rigidbody2D>();
	}
	
    List<double> Run(double bx, double by, double bvx, double bvy, double px, double py, double pv, bool train) {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        inputs.Add(bx);
        inputs.Add(by);
        inputs.Add(bvx);
        inputs.Add(bvy);
        inputs.Add(px);
        inputs.Add(py);
        outputs.Add(pv);
        if (train)
            return (ann.Train(inputs, outputs));
        else
            return (ann.CalcOutput(inputs, outputs));
    }

	// Update is called once per frame
	void Update () {
        float posy = Mathf.Clamp(paddle.transform.position.y + (yvel * Time.deltaTime * paddleMaxSpeed), paddleMinY, paddleMaxY);   //calculate new position based on the calculated input from the last frame
        paddle.transform.position = new Vector3(paddle.transform.position.x, posy, paddle.transform.position.z);                    //set new position

        List<double> output = new List<double>();
        int layerMask = 1 << 8;                     //this is the int of the back wall(we move the single '1' bit to the left by 8 bits
        RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, brb.velocity, 1000, layerMask);

        if(hit.collider != null) {
            if(hit.collider.gameObject.tag == "tops") { //reflect off top
                Vector3 reflection = Vector3.Reflect(brb.velocity, hit.normal);
                hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
            }
        }

        if (hit.collider != null && hit.collider.gameObject.tag == "backwall") {//if the raycast from the balls direction hits a back wall
            float dy = (hit.point.y - paddle.transform.position.y);                 //calculate the difference betweent he raycast hit point and the paddle's current position

            output = Run(ball.transform.position.x,
                            ball.transform.position.y,
                            brb.velocity.x, brb.velocity.y,
                            paddle.transform.position.x,
                            paddle.transform.position.y,
                            dy, true);      //train the algorithm and get it's result
            yvel = (float)output[0];    //result of neural net training that will modify the position upon next frame
        } else                              //if the raycast from the ball does not hit a wall do nothing
            yvel = 0; //paddle is not going to move
	}
}
