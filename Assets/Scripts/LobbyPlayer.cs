using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class LobbyPlayer : ScriptableObject {

	public Sprite image;
	public int teamNo;
	public new string name;
    public string guess;
    public bool AI;
    public int type;
    public int id;
    public bool local;
}
