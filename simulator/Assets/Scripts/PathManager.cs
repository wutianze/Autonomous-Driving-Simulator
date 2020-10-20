using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;

public class PathManager : MonoBehaviour {

	public CarPath path;

	public GameObject prefab;
	
	public Transform startPos;

	Vector3 span = Vector3.zero;

	public float spanDist = 5f;// length of one part 

	public float roadWidth = 5f;

	public int numSpans = 100;

	public float turnInc = 1f;

	public bool sameRandomPath = true;

	public int randSeed = 2;

	//public bool doMakeRandomPath = true;

	//public bool doLoadScriptPath = false;

	//public bool doLoadPointPath = false;

	public bool doBuildRoad = false;

	public bool doChangeLanes = false;

	public int smoothPathIter = 0;

	public bool doShowPath = false;

    //public string pathToLoad = "none";

	public RoadBuilder roadBuilder;
	public RoadBuilder semanticSegRoadBuilder;

	public LaneChangeTrainer laneChTrainer;

	void Awake () 
	{
		if(sameRandomPath)
			Random.InitState(randSeed);

		InitNewRoad(0);			
	}

	// method 0: randomPath, method 1: script road
	public void InitNewRoad(int method)
	{
		Debug.Log(string.Format("Test Debug"));
		switch (method){
			case 0: MakeRandomPath();
			break;
			case 1: MakePointPath();//MakeScriptedPath();
			break;
			default:MakeRandomPath();
			break;
		}
		/*
		if(doMakeRandomPath)
		{
			MakeRandomPath();
		}
		else if (doLoadScriptPath)
		{
			MakeScriptedPath();
		}
		else if(doLoadPointPath)
		{
			MakePointPath();
		}
		*/
		if(smoothPathIter > 0)
			SmoothPath();

		//Should we build a road mesh along the path?
		if(doBuildRoad && roadBuilder != null)
			roadBuilder.InitRoad(path);

		if(doBuildRoad && semanticSegRoadBuilder != null)
			semanticSegRoadBuilder.InitRoad(path);

		if(laneChTrainer != null && doChangeLanes)
		{
			laneChTrainer.ModifyPath(ref path);
		}

		if(doShowPath)
		{
			for(int iN = 0; iN < path.nodes.Count; iN++)
			{
				Vector3 np = path.nodes[iN].pos;
				GameObject go = Instantiate(prefab, np, Quaternion.identity) as GameObject;
				go.tag = "pathNode";
				go.transform.parent = this.transform;
			}
		}
	}

	public void DestroyRoad()
	{
		GameObject[] prev = GameObject.FindGameObjectsWithTag("pathNode");

		foreach(GameObject g in prev)
			Destroy(g);

		if(roadBuilder != null)
			roadBuilder.DestroyRoad();
	}

    public Vector3 GetPathStart()
    {
        return startPos.position;
    }

    public Vector3 GetPathEnd()
    {
		/*int iN = path.nodes.Count - 1;

        if(iN < 0)
            return GetPathStart();

        return path.nodes[iN].pos;*/
		return path.vertices[path.vertices.Length - 1];
    }

	void SmoothPath()
	{
		while(smoothPathIter > 0)
		{
			path.SmoothPath();
			smoothPathIter--;
		}
	}

	/*
	void MakePointPath()
	{
		string filename = GlobalState.script_path;

		TextAsset bindata = Resources.Load(filename) as TextAsset;

		if(bindata == null)
			return;

		string[] lines = bindata.text.Split('\n');

		Debug.Log(string.Format("found {0} path points. to load", lines.Length));

		path = new CarPath();

		Vector3 np = Vector3.zero;

		float offsetY = -0.1f;

		foreach(string line in lines)
		{
			string[] tokens = line.Split(' ');

			if (tokens.Length <= 5)
				continue;
			int index = int.Parse(tokens[0]);
			np.x = float.Parse(tokens[1]);
			np.y = float.Parse(tokens[2]) + offsetY;
			np.z = float.Parse(tokens[3]);
			for (int i = 4;i < tokens.Length;i++) {
				LineToDraw tmpltd = new LineToDraw();
				tmpltd.start = index;
				tmpltd.end = int.Parse(tokens[i]);
				path.lines.Add(tmpltd);
			}
			PathNode p = new PathNode();
			p.pos = np;
			path.nodes.Add(p);
		}
			
	}*/
	void MakePointPath()
	{
		string filename = GlobalState.script_path;

		string[] lines = File.ReadAllLines(filename);

		Debug.Log(string.Format("found {0} path points. to load", lines.Length));

		path = new CarPath();
		Vector3 s = startPos.position;
		s.x = s.x - 2.5f;
		float offsetY = 0.1f;

		string[] pathSetting = lines[0].Split(' ');
		int numVerts = int.Parse(pathSetting[0]);
		int numTriIndecies = (numVerts - int.Parse(pathSetting[1])*2)*3;
		int numThings = int.Parse(pathSetting[2]);
		Debug.Log(string.Format("numVerts: {0}, numTriIndecies: {1}, numThings: {2}", numVerts, numTriIndecies, numThings));
		path.initScriptsCarPath(numVerts, numTriIndecies, numThings);
		
		string[] points = lines[1].Split(',');
		for(int i = 0; i < points.Length; i++)
        {
			string[] onePoint = points[i].Split(' ');
			Debug.Log(string.Format("read line: {0},{1},{2},{3}",int.Parse(onePoint[0]),float.Parse(onePoint[1]), float.Parse(onePoint[2]), float.Parse(onePoint[3])));
			path.vertices[int.Parse(onePoint[0])] = new Vector3(float.Parse(onePoint[1]), float.Parse(onePoint[2]) + offsetY, float.Parse(onePoint[3])) + s;
        }

		string[] putThings = lines[2].Split(',');
		for (int i = 0; i < numThings; i++) {
			string[] oneThing = putThings[i].Split(' ');
			Debug.Log(string.Format("read thing: {0},{1},{2},{3}", float.Parse(oneThing[1]), float.Parse(oneThing[2]), float.Parse(oneThing[3]), float.Parse(oneThing[4])));
			ThingObject thingTmp = new ThingObject();
			thingTmp.thing = oneThing[0];
			thingTmp.pos = new Vector3(float.Parse(oneThing[1]), float.Parse(oneThing[2]) + offsetY, float.Parse(oneThing[3])) + s;
			thingTmp.thing_rot = Quaternion.Euler(0.0f, float.Parse(oneThing[4]), 0f);
			path.things[i] = thingTmp;
		}

		int triIndex = 0;
		int uvIndex = 0;
		int pointIndex = 0;
		for (int i = 3; i < lines.Length; i++) {
			string[] pointTmp = lines[i].Split(' ');
			for (int j = 0; j < pointTmp.Length; j = j + 2) {
				path.uv[int.Parse(pointTmp[j])] = new Vector2(0.2f * uvIndex, 0.0f);//设置贴图坐标
				path.uv[int.Parse(pointTmp[j+1])] = new Vector2(0.2f * uvIndex, 1.0f);
				path.normals[int.Parse(pointTmp[j])] = Vector3.up;
				path.normals[int.Parse(pointTmp[j+1])] = Vector3.up;
				uvIndex = uvIndex + 1;
				if (j + 3 >= pointTmp.Length) { break; }
				path.tri[triIndex] = int.Parse(pointTmp[j]);
				path.tri[triIndex+1] = int.Parse(pointTmp[j+2]);
				path.tri[triIndex+2] = int.Parse(pointTmp[j+1]);
				path.tri[triIndex+3] = int.Parse(pointTmp[j+2]);
				path.tri[triIndex+4] = int.Parse(pointTmp[j+3]);
				path.tri[triIndex+5] = int.Parse(pointTmp[j+1]);
				triIndex = triIndex + 6;
			}
		}
	}

	/*
	void MakeScriptedPath()
	{
		TrackScript script = new TrackScript();

		if(script.Read(GlobalState.script_path))
		{
			path = new CarPath();
			TrackParams tparams = new TrackParams();
			tparams.numToSet = 0;
			tparams.rotCur = Quaternion.identity;
			tparams.lastPos = startPos.position;

			float dY = 0.0f;
			//float turn = 0f;

			Vector3 s = startPos.position;
			s.y = 0.5f;
			span.x = 0f;
			span.y = 0f;
			span.z = spanDist;
			float turnVal = 10.0f;
			float totalTurn = 0.0f;// record the direction
			foreach(TrackScriptElem se in script.track)
			{
				Debug.Log(se.state.ToString());
				string thing = "";// "" means just road, others can be things in the road
				float thing_offset = 0.0f;
				float thing_rot = 0.0f;
				if(se.state == TrackParams.State.AngleDY)
				{
					turnVal = se.value;// set how many degrees every node turn
				}
				else if(se.state == TrackParams.State.CurveY)
				{
					//turn = 0.0f;
					dY = se.value * turnVal;
				}
				else if(se.state == TrackParams.State.CONE)
				{
					thing = "cone";
					thing_offset = se.value;
					thing_rot = se.value2;
				}
				else if(se.state == TrackParams.State.BLOCK)
				{
					thing = "block";
					thing_offset = se.value;
					thing_rot = se.value2;
				}
				else
				{
					dY = 0.0f;
					//turn = 0.0f;
				}

				for(int i = 0; i < se.numToSet; i++)
				{
					totalTurn += dY;
					Vector3 np = s;
					PathNode p = new PathNode();
					p.pos = np;
					p.thing = thing;
					p.thing_offset = thing_offset;

					p.thing_rot = Quaternion.Euler(0.0f, thing_rot + totalTurn, 0f);
					path.nodes.Add(p);

					//turn = dY;

					Quaternion rot = Quaternion.Euler(0.0f, dY, 0f);// y turn
					span = rot * span.normalized;
					span *= spanDist;
					s = s + span;
				}
					
			}
		}
	}*/

	void MakeRandomPath()
	{
		path = new CarPath();

		Vector3 s = startPos.position;
		float turn = 0f;
		s.y = 0.5f;

		span.x = 0f;
		span.y = 0f;
		span.z = spanDist;
		for(int iS = 0; iS < numSpans; iS++)
		{
			Vector3 np = s;
			PathNode p = new PathNode();
			p.pos = np;
			path.nodes.Add(iS,p);

			float t = Random.Range(-1.0f * turnInc, turnInc);

			turn += t;

			Quaternion rot = Quaternion.Euler(0.0f, turn, 0f);
			span = rot * span.normalized;

			if(SegmentCrossesPath( np + (span.normalized * 100.0f), 90.0f ))
			{
				//turn in the opposite direction if we think we are going to run over the path
				turn *= -0.5f;
				rot = Quaternion.Euler(0.0f, turn, 0f);
				span = rot * span.normalized;
			}

			span *= spanDist;

			s = s + span;
		}
	}

	public bool SegmentCrossesPath(Vector3 posA, float rad)
	{
		foreach(KeyValuePair<int, PathNode> pn in path.nodes)
		{
			float d = (posA - pn.Value.pos).magnitude;

			if(d < rad)
				return true;
		}

		return false;
	}

	public void SetPath(CarPath p)
	{
		path = p;

		GameObject[] prev = GameObject.FindGameObjectsWithTag("pathNode");

		Debug.Log(string.Format("Cleaning up {0} old nodes. {1} new ones.", prev.Length, p.nodes.Count));

		DestroyRoad();

		foreach(KeyValuePair<int,PathNode> pn in path.nodes)
		{
			GameObject go = Instantiate(prefab, pn.Value.pos, Quaternion.identity) as GameObject;
			go.tag = "pathNode";
		}
	}
}
