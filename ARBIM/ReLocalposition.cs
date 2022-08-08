//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ReLocalposition : MonoBehaviour
//{
//    public static ReLocalposition instance;

//	private GameObject obj;
//	public GameObject Obj
//	{
//		get { return obj; }
//		set { obj = value; }
//	}

//	void Awake()
//	{
//        if (instance == null) instance = this;
//	}

//	void Start()
//	{

//	}

//	public void RelocateAndRescale()
//	{
//		Transform[] childrenOnAir = obj.transform.GetComponentsInChildren<Transform>(); // rootµµ Æ÷ÇÔ
		
//		foreach (var child in childrenOnAir)
//		{
//			if (child != obj.transform.root)
//				child.transform.localPosition += new Vector3(222498f, -151.6f, 452011.5f);

//			obj.transform.localPosition = new Vector3(-2.17f, 0f, -0.55f);
//		}

//		obj.transform.localScale *= 0.01f;
//	}
//}
