using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreData {
    public string score;
    public string playerId;
    public long timestamp;
}

[System.Serializable]
public class RequestData {
    public ScoreData payload;
    public string signature;
}
