﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Threading
 

public class HalfAngleMesh : MonoBehaviour {

	int maxNumSlices = 512;
	public int numSlices = 32;


	public GameObject directionalLight;
	public GameObject mainCamera;

	//for calcing the intersections in inverted space
	private GameObject invCamSphere;
	//for  parenting for rotation
	private GameObject invCamSphereRotation;


	private Vector3 cameraVector;
	private Vector3 lightVector;

	private Mesh planemesh;

	Vector3 invLightVector;
	Vector3 invCamPosition;
	Vector3 invCamVector;


	private List<Vector3> vertices = new List<Vector3> ();
	private List<int> indices = new List<int>();
	private List<Vector2> uvs = new List<Vector2>();

	private int totalVerts = 0;
	private Vector3[] boxVertices;
	private Vector3[] boxEdgeDirections;
	private int[] edgeTestOrder;
	private int[] vertexTestOrder;


	private GameObject[] planes;

	float min_dist;
		float max_dist;


	// Use this for initialization
	void Start() 
	{

		boxVertices = new Vector3[]{
			new Vector3( -0.5f, -0.5f, 0.5f),    //bottom left front
			new Vector3( -0.5f, 0.5f, 0.5f),     //top left front
			new Vector3( 0.5f, 0.5f, 0.5f),      //top right front
			new Vector3( 0.5f, -0.5f, 0.5f),     //bottom right front
			new Vector3( -0.5f, -0.5f, -0.5f),   //bottom left back
			new Vector3( -0.5f, 0.5f, -0.5f),    //top left back
			new Vector3( 0.5f, 0.5f, -0.5f),     //top right back
			new Vector3( 0.5f, -0.5f, -0.5f)     //bottom right back
		};



		boxEdgeDirections = new Vector3[]{
			new Vector3( 0.0f, -1.0f, 0.0f),    //down
			new Vector3( 1.0f, 0.0f, 0.0f),    //right
			new Vector3( 0.0f, 0.0f, -1.0f),    // back

			new Vector3( -1.0f, 0.0f, 0.0f),   //left  
			new Vector3( 0.0f, 1.0f, 0.0f),   //up
			new Vector3( 0.0f, 0.0f, -1.0f),   //back 

			new Vector3( 0.0f, 0.0f, 1.0f),		//forward    
			new Vector3( 0.0f, 1.0f, 0.0f),    //up
			new Vector3( 1.0f, 0.0f, 0.0f),		//right

			new Vector3( 0.0f, 0.0f, 1.0f),		//forward
			new Vector3(-1.0f, 0.0f, 0.0f),		//left
			new Vector3( 0.0f, -1.0f, 0.0f),		//down

		};

		edgeTestOrder = new int[]{
			0,2,5,
			0,2,7,
			0,5,7,
			2,5,7
		};

		vertexTestOrder = new int[]{
			1,
			3,
			4,
			6
		};

		
		
		
//		1
//
//			top left front          [1]
//				bottom left front   [0]
//				top right front     [2]
//				top left back       [5]
//		2
//			bottom right front      [3]
//				bottom left front   [0]
//				top right front     [2]
//				bottom right back   [7]
//
//		3
//			bottom left back to     [4]    
//				bottom left front   [0]
//				top left back       [5]
//				bottom right back   [7]
//			
//			
//		4
//			top right back to       [6]
//				top right front     [2]
//				top left back       [5]
//				bottom right back   [7]

	
		



//		planes = new GameObject[]{
//			
//			GameObject.CreatePrimitive(PrimitiveType.Plane), 
//			GameObject.CreatePrimitive(PrimitiveType.Plane), 
//			GameObject.CreatePrimitive(PrimitiveType.Plane), 
//			GameObject.CreatePrimitive(PrimitiveType.Plane), 
//			GameObject.CreatePrimitive(PrimitiveType.Plane), 
//			GameObject.CreatePrimitive(PrimitiveType.Plane), 
//			
//		};

//		for (int i=0; i < 6; i++){
//			planes[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
//		};
		//invertedCubeTransform = new GameObject ("invertedTransform");
		//invertedCubeTransform.transform.parent = gameObject.transform;
//		foreach (var spherePos in boxVertices)
//		{
//			GameObject tempGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//			tempGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
//			tempGO.transform.position = spherePos;
//			tempGO.transform.parent = gameObject.transform;
//		}
//		foreach (var spherePos in boxVertices)
//		{
//			GameObject tempGO2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//			tempGO2.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
//			tempGO2.transform.position = spherePos;
//			tempGO2.transform.parent = invertedCubeTransform.transform;
//		}
		//invCamSphereRotation = new GameObject ("invcamrotation");
		invCamSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		invCamSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		invCamSphere.transform.position = invCamSphere.transform.position;

		updateVectors ();

		planemesh = new Mesh();
		GetComponent<MeshFilter>().mesh = planemesh;



	}
	
	// Update is called once per frame
	void Update()
	{


//		invCamSphere.transform.position = new Vector3 (0.0f, 0.0f, 0.0f);
//		invCamSphere.transform.rotation = Quaternion.identity;
////
//		Vector3 invRot = gameObject.transform.rotation.eulerAngles;
//////		Vector3 inverseScale = new Vector3(1.0f / gameObject.transform.localScale.x, 1.0f / transform.localScale.y, 1.0f / transform.localScale.z);
//		invCamSphere.transform.Rotate(new Vector3(invRot.x * -1.0f,
//		                                          invRot.y * -1.0f,
//		                                          invRot.z * -1.0f));
//		invCamSphere.transform.Translate(new Vector3(transform.position.x * -1.0f,
//			                                                       transform.position.y * -1.0f,
//			                                              transform.position.z * -1.0f) + mainCamera.transform.position);


		updateVectors();



		//Debug.Log ("inverted pos is " + invPos);
		//Debug.Log ("inverted scale is " + inverseScale);
		//Debug.Log ("inverted rotation is " + invRot);

		Vector3 halfEulerAngles;
		Quaternion halfQuat;
		Vector3 halfVector;
		totalVerts = 0;
//
//		if (directionalLight != null)
//		{
//
////			Debug.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), lightVector, Color.yellow);
////			Debug.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), invLightVector, Color.green);
//
//			//Debug.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), lightVector, Color.yellow);
//
//
//		
//
//		} 
//
//		if (mainCamera != null)
//		{
//			
//		} 

		//half angle
		if ((mainCamera != null) && (directionalLight != null))
		{


			float DotProductcamLight = Vector3.Dot(invLightVector.normalized, invCamVector.normalized);
			//Debug.Log("dot product is " + DPcamLight);

			bool bViewInverted =  DotProductcamLight >= 0;

			halfVector = (( bViewInverted ? invCamVector.normalized :invCamVector.normalized * -1) 
			              + invLightVector.normalized).normalized;
			Debug.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), halfVector, Color.red);
			//halfEulerAngles = Quaternion.LookRotation(halfVector).eulerAngles;
			//halfQuat =  Quaternion.LookRotation(halfVector);
			//Debug.Log("halfEuler = " + halfEulerAngles);


			max_dist = Vector3.Dot(halfVector, boxVertices[0]);
			min_dist = max_dist;
			int max_index = 0;
			int count = 0;
			for(int i=1;i<8;i++) {
				float dist = Vector3.Dot(halfVector, boxVertices[i]);
				if(dist > max_dist) {
					max_dist = dist;
					max_index = i;
				}
				if(dist<min_dist)
					min_dist = dist;
			}
//			Debug.Log("max_dist is " + max_dist);
//			Debug.Log("min_dist is " + min_dist);
			Matrix4x4 M = Matrix4x4.TRS (transform.position, transform.rotation, transform.localScale);

			//Debug.DrawLine(M.MultiplyPoint3x4(halfVector*max_dist), M.MultiplyPoint3x4(halfVector*min_dist));
			//Debug.DrawRay(transform.position + M.MultiplyPoint3x4(halfVector*min_dist),M.MultiplyPoint3x4(halfVector*max_dist)*2);
			//Debug.DrawRay(transform.position, M.MultiplyVector(halfVector*max_dist));
//			for (int i=0; i < 6; i++){
//
//				Vector3 newpos = (M.MultiplyPoint3x4(halfVector*min_dist));
//				Vector3 newdir = (M.MultiplyVector(halfVector*max_dist));
//
//				planes[i].transform.position = newpos+ ((newdir*2)*((float)i/6.0f));
//				planes[i].transform.LookAt(mainCamera.transform.position);
//				planes[i].transform.rotation = Quaternion.LookRotation(newdir);
//				planes[i].transform.rotation *= Quaternion.Euler(bViewInverted ? -90 : 90, 0, 0);
//
//
//
//			
//			}

						//Debug.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), invLightVector, Color.green);
						//Debug.DrawLine(invCamPosition, invCamPosition + invCamVector, Color.blue);


//			for (int o = 0; o < 512; o++){
//				float t = 0;
//				if(	calcRayPlaneItersection(boxVertices[0], 
//				                            new Vector3(0.0f, 1.0f, 0.0f), 
//				                            halfVector*0.3f, -halfVector, 
//				                            ref t)){
//												//Debug.Log( "intersected! " + t);
//												Debug.DrawLine(	M.MultiplyPoint3x4(boxVertices[0]+ new Vector3(0.0f, t, 0.0f)),
//					               								M.MultiplyPoint3x4(boxVertices[0]+ new Vector3(0.0f, t+0.05f, 0.0f)));
//			}
			//Debug.DrawLine(boxVertices[0], boxVertices[1]);

			Vector3 newpos = ((halfVector*min_dist));
			Debug.Log(newpos);
			Vector3 newdir = ((halfVector*max_dist));
			Debug.Log(newdir);
			float halfVectorLength = ((halfVector*max_dist)-(halfVector*min_dist)).magnitude;
			Debug.Log("mag is " + halfVectorLength);
			float stepSize = halfVectorLength/(float)numSlices;

			for (int s = 0; s < numSlices; s++){

				List<Vector3> localVertexList = new List<Vector3>();
				List<int> localIndicesList = new List<int>();
				List<Vector2> localuvList = new List<Vector2>();


				int numVerticesThisSlice = 0;
				Vector3 C = newpos+(halfVector.normalized*s*stepSize)+(((halfVector.normalized*stepSize)/2));


				for (int e = 0; e < 12; e++){

					int edgeTestIndex = vertexTestOrder[(int)((e/(int)3))];
					//set start point	
					Vector3 boxPoint = boxVertices[edgeTestIndex];
					//set dir
					Vector3 dir = boxEdgeDirections[e];  

					float t= 0;

					if(	calcRayPlaneItersection(
							boxPoint, 
							dir, 
		                    //new Vector3(0.0f, 0.0f, 0.0f),
							C,
							halfVector, 
		                    ref t)){

									//Debug.Log( "intåersected! " + t);
									//Debug.DrawLine(	M.MultiplyPoint3x4(boxPoint + (dir*t)),
					               	//				M.MultiplyPoint3x4(boxPoint + (dir*(t+0.008f))));
									numVerticesThisSlice++;
									localVertexList.Add(boxPoint + (dir*t));
									localuvList.Add(new Vector2(0.0f, 0.0f));
									}
		           
				}
				localVertexList.Sort( (Vector3 A, Vector3 B) =>{
					
					//compare function return -1 or 1}
					//if ( pos then
					
					if (Vector3.Dot(halfVector, Vector3.Cross(A-C, B-C)) <= 0 ){
						return 1;
					}
					else{
						return -1;
					}

					//if neg then
					//return 1;
					//http://forum.unity3d.com/threads/sorting-arrays.136097/
					//					Less than zero
					
					//						
					//					This instance precedes obj in the sort order.
					//							
					//					Zero
					//							
					//							
					//					This instance occurs in the same position in the sort order as obj.
					//							
					//					Greater than zero
					//							
					//							
					//					This instance follows obj in the sort order. 
					}
				);

				
//				There's no need to convert everything to 2D.

//You have the center C and the normal n. To determine whether point B is clockwise or counterclockwise from point A,
//calculate dot(n, cross(A-C, B-C)). If the result is positive, B is counterclockwise from A; if it's negative, B is clockwise from A.
				//add verts and indices to list
				//int vert = 1;
				Vector3 centroid = new Vector3(0.0f, 0.0f, 0.0f);


				localuvList.Add(new Vector2(0.0f, 0.0f));
				for(int p = 0; p < numVerticesThisSlice; p++){
					localIndicesList.Add(totalVerts);
					if (bViewInverted){
						localIndicesList.Add( ((p+2)%numVerticesThisSlice+1)+totalVerts);
						localIndicesList.Add( ((p+1)%numVerticesThisSlice+1)+totalVerts);
					}
					else{
						localIndicesList.Add( ((p+1)%numVerticesThisSlice+1)+totalVerts);
						localIndicesList.Add( ((p+2)%numVerticesThisSlice+1)+totalVerts);
					}


					centroid += localVertexList[p];
				}

				centroid /= numVerticesThisSlice;
				localVertexList.Insert(0,centroid);

				vertices.AddRange(localVertexList);
				indices.AddRange(localIndicesList);
				uvs.AddRange(localuvList);
				totalVerts +=numVerticesThisSlice+1;


			}
			planemesh.Clear();
			planemesh.vertices = vertices.ToArray();
			planemesh.triangles = indices.ToArray();
			planemesh.uv = uvs.ToArray();
			GetComponent<MeshFilter>().mesh = planemesh;


			vertices.Clear();
			indices.Clear();
			uvs.Clear();
			totalVerts = 0;
//			invertedCubeTransform.transform.localScale = inverseScale;
//			invertedCubeTransform.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

//			Quaternion halfToForward = Quaternion.FromToRotation(transform.forward, halfEulerAngles );
//			Debug.Log("quat inv is " + halfToForward.ToString());
//			invertedCubeTransform.transform.rotation = halfToForward;


		}

	}

	void updateVectors()
	{


		//this is what we calc the inverse from and also what we mult our final intersection points by
		Matrix4x4 M = Matrix4x4.TRS (transform.position, transform.rotation, transform.localScale);

		if (directionalLight != null)
		{
			lightVector = directionalLight.transform.forward;
			//Debug.Log("directional light assinged " + lightVector.x + " ," + lightVector.y + " ," + lightVector.z);
		} 
		else
		{ 
			//Debug.Log("directional light wasn't assigned, renering will not work");
		}
		
		if (mainCamera != null)
		{
			cameraVector = (gameObject.transform.position - mainCamera.transform.position ).normalized;
			//Debug.Log("camera vector assinged " + cameraVector.x + " ," + cameraVector.y + " ," + cameraVector.z);
		} 
		else
		{ 
			//Debug.Log("camera wasn't assigned, rendering will not work");
		}

		invCamPosition = mainCamera.transform.position;
		

		invCamPosition = M.inverse.MultiplyPoint3x4 (invCamPosition);
		invLightVector = M.inverse.MultiplyVector(lightVector).normalized;
		invCamSphere.transform.position = invCamPosition;
		invCamVector = -invCamPosition.normalized;

		//Debug.Log("inv camera vector assinged " + invCamVector.x + " ," + invCamVector.y + " ," + invCamVector.z);

	}


	//calc ray intersection
	bool calcRayPlaneItersection(Vector3 boxEdgeOrigin, Vector3 boxEdgeDirection, Vector3 planeOrigin, Vector3 planeNormal, ref float intersectionDistanceAlongEdge)
	{
		float denominator = Vector3.Dot( planeNormal, boxEdgeDirection );
		if (denominator != 0.0f){
			Vector3 boxEdgeOriginMinusPlaneOrigin = planeOrigin - boxEdgeOrigin ;
			intersectionDistanceAlongEdge = (Vector3.Dot(boxEdgeOriginMinusPlaneOrigin, planeNormal) / denominator);
			return ((intersectionDistanceAlongEdge  >= 0)&& (intersectionDistanceAlongEdge  <= 1));
			//return true;

		}//
		return false;
	}

	//fill vertexList
	void fillVertexList()
	{
	
	}

}
