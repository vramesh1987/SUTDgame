using UnityEngine;
using System.Collections;
using System;

public class CamController : MonoBehaviour {

	public Material mat;
	public int hitsCount = 0;
	public Vector3 u, v;
	// Use this for initialization
	void Start () {
		//mat = new Material(Shader.Find("VertexLit"));
		mat = new Material( "Shader \"Lines/Colored Blended\" {" +                             
		                     "SubShader { Pass { " +
		                     "    Blend SrcAlpha OneMinusSrcAlpha " +
		                     "    ZWrite Off Cull Off Fog { Mode Off } " +
		                     "    BindChannels {" +
		                     "      Bind \"vertex\", vertex Bind \"color\", color }" +
		                     "} } }" );
		}

	void OnPostRender() {
			GL.PushMatrix();
			mat.SetPass(0);
			for (int i=0; i<BallController.uQ.Count; i++) {
						GL.Begin (GL.QUADS);
						GL.Color (Color.yellow);
						u = (Vector3)BallController.uQ[i];
						v = (Vector3)BallController.vQ[i];
						//GL.Vertex (BallController.startVertex);
						//GL.Vertex (BallController.endVertex);
						//GL.Vertex (new Vector3 (BallController.endVertex.x + 0.15f, BallController.endVertex.y, BallController.endVertex.z));
						//GL.Vertex (new Vector3 (BallController.startVertex.x + 0.15f, BallController.startVertex.y, BallController.startVertex.z));
						GL.Vertex (u);
						GL.Vertex (v);
						GL.Vertex (new Vector3 (v.x + 0.15f, v.y, v.z));
						GL.Vertex (new Vector3 (u.x + 0.15f, u.y, u.z));
						GL.End ();
				}
			updateTimeLine();
			GL.PopMatrix();   
			if (BallController.hits != hitsCount) {
			GetComponent<AudioSource>().Play();
				hitsCount = BallController.hits;
			}
	}

	void updateTimeLine(){
		GL.Begin (GL.QUADS);
		GL.Color (new Color(0.702F, 0.9255F, 0.9490F));
		//u = (Vector3)BallController.uQ[i];
		//v = (Vector3)BallController.vQ[i];
		float len = (BallController.adjustedTime) / (Convert.ToInt32 (BallController.editString));
		GL.Vertex (new Vector3 (Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, 0)).x, GameObject.Find ("TimeLine").transform.position.y - 0.2f, 0));
		GL.Vertex (new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x, GameObject.Find("TimeLine").transform.position.y+0.2f, 0));
		GL.Vertex (new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(len, 0, 0)).x, GameObject.Find("TimeLine").transform.position.y+0.2f, 0));
		GL.Vertex (new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(len, 0, 0)).x, GameObject.Find("TimeLine").transform.position.y-0.2f, 0));
		GL.End ();
	
	}
	// Update is called once per frame
	void Update () {

	}
}
