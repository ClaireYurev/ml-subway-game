using System.Collections.Generic;
using UnityEngine;


// # Easter Egg : int32 2201010001
// Easter Egg: int64
// Subway top-view game

public class SpawnController : MonoBehaviour
{
    [SerializeField] Locomotive _locomotivePrefab;
    [SerializeField] LineRenderer _lineRendererPrefab;

    [SerializeField] Transform _locomotivesParent;
    [SerializeField] Transform _stationsParent;
    [SerializeField] Transform _passengersParent;
    [SerializeField] Transform _lineRendererParent;
    [SerializeField] RectTransform _stationsSpawnArea;

    [SerializeField] LevelProperties _levelProperties;
    
    int[] _stationsIndexes;
    float[] _stationsProbabilities;

    void Start()
    {
        PassengersFactory.Instance.Init(_levelProperties, _passengersParent);
        GetProbabilities();
    }

    public Line CreateLine(Color color, int type)
    {
        Line line = new Line
        {
            LineType = type,
            Color = color,
            LineRenderer = Instantiate(_lineRendererPrefab, _lineRendererParent)
        };
        line.LineRenderer.Line = line;

        return line;
    }
    
    public Locomotive SpawnLocomotive()
    {
        Locomotive locomotive = Instantiate(_locomotivePrefab, _locomotivesParent);
        locomotive.DestinationReached += () => UserProgress.Current.Score++;
        return locomotive;
    }
    
    public Station SpawnStation(List<Station> stations)     // Here is where the action starts // # Easter Egg: int32 2201010001 // # Easter Egg 2: int32 2201010001
    {
        Vector3[] corners = new Vector3[4];
        _stationsSpawnArea.GetWorldCorners(corners);
        Vector3 min = corners[0];
        Vector3 max = corners[2];
		
        float minSpace = 0.6f;
        float delta = 0.5f;
		
        Vector3 position = new Vector3();
        Vector3[] positions = new Vector3[99];  // NEW!!! Changed () to [] twice and inserted 10
        positions[0] = new Vector3();     // Zero
        // positions[] = new Vector3(,);  // 
        // positions[1] = new Vector3(-13,4);  // LEFT-UPPERMOST
        positions[01] = new Vector3(-13,2);  // Jane
        positions[02] = new Vector3(-12,2);  // Runnymede
        positions[03] = new Vector3(-11,2);  // High Park
        positions[04] = new Vector3(-10,2);  // Keele
        positions[05] = new Vector3(-9,2);   // Dundas West    
        positions[06] = new Vector3(-8,2);   // Lansdowne
        positions[07] = new Vector3(-7,2);   // Dufferin
        positions[08] = new Vector3(-6,2);   // Ossignton
        positions[09] = new Vector3(-5,2);   // Christie
        positions[10] = new Vector3(-4,2);   // Bathurst
        positions[11] = new Vector3(-3,2);   // Spadina
        positions[12] = new Vector3(-2,2);   // St George
        positions[13] = new Vector3(-1,2);   // Bay
        positions[14] = new Vector3(0,2);    // Bloor-Yonge
        positions[15] = new Vector3(1,2);    // Sherbourne
        // All of a sudden, St Clair West station will show up below
        positions[16] = new Vector3(-4,5);          // St Clair West
        positions[17] = new Vector3(-3,4);          // Dupont
        // Line 1 is skipping Spadina & St George, but we want the user to connect them
        positions[18] = new Vector3(-2,1);          // Museum
        positions[19] = new Vector3(-2,0);          // Queen's Park
        positions[20] = new Vector3(-2,-1);         // St Patrick
        positions[21] = new Vector3(-2,-2);         // Osgoode
        positions[22] = new Vector3(-2,-3);         // St Andrew
        positions[23] = new Vector3(-1,-4);     // UNION
        positions[24] = new Vector3(0,-3);   // King
        positions[25] = new Vector3(0,-2);   // Queen
        positions[26] = new Vector3(0,-1);   // Dundas
        positions[27] = new Vector3(0,0);    // College
        positions[28] = new Vector3(0,1);    // Wellesley
        // Line 1 is again skipping Bloor-Yonge, but we want the user to connect them
        positions[29] = new Vector3(0,3);    // Rosedale
        positions[30] = new Vector3(0,4);    // Summerhill
        positions[31] = new Vector3(0,5);    // St Clair
        positions[32] = new Vector3(-7,7);          // Eglinton West
        positions[33] = new Vector3(0,6);    // Davisville
        positions[34] = new Vector3(0,7);    // Eglinton
        positions[35] = new Vector3(-7,8);          // Glencairn
        positions[36] = new Vector3(-7,9);          // Lawrence West
        positions[37] = new Vector3(0,9);    // Lawrence
        positions[38] = new Vector3(-7,10);         // Yorkdale
        positions[39] = new Vector3(0,11);   // York Mills
        positions[40] = new Vector3(-7,11);         // Wilson
        positions[41] = new Vector3(0,12);   // Sheppard-Yonge
        positions[42] = new Vector3(-7,12);         // Sheppard West           

        //positions[] = new Vector3(,);  // 
        //positions[9] = new Vector3(13,4);  // RIGHT-UPPERMOST

        bool correctPosition = false;
        int i = 0;
        
        while (!correctPosition && i < 100)
        {
            i++;
            correctPosition = true;
            // OLD CODE:   position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
            position = positions[i];     //  New code logic here

            position.x = Mathf.RoundToInt(position.x / delta) * delta;
            position.y = Mathf.RoundToInt(position.y / delta) * delta;
			
            foreach (Station s in stations)
            {
                if (Vector3.Distance(s.Position, position) >= minSpace)
                    continue;

                correctPosition = false;
                break;
            }
        }

        if (i >= 100) return null;
        
        int id = stations.Count;
        int stationType = Probabilities.GetRandomValue(_stationsIndexes, _stationsProbabilities);
        Station station = _levelProperties.StationsProperties.Stations[stationType].StationPrefab;
        station = Instantiate(station, _stationsParent);
        station.Init(id, stationType, position, _levelProperties.FullStationDuration, true);

        return station;
    }
    
    public Station SpawnStation(Vector3 position, int stationType, int stationId)
    {
        Station station = _levelProperties.StationsProperties.Stations[stationType].StationPrefab;
        station = Instantiate(station, _stationsParent);
        station.Init(stationId, stationType, position, _levelProperties.FullStationDuration, false);

        return station;
    }
    
    void GetProbabilities()
    {
        _stationsIndexes = new int[_levelProperties.StationsProperties.Stations.Length];
        _stationsProbabilities = new float[_levelProperties.StationsProperties.Stations.Length];
		
        for (int i = 0; i < _levelProperties.StationsProperties.Stations.Length; i++)
        {
            _stationsIndexes[i] = i;
            _stationsProbabilities[i] = _levelProperties.StationsProperties.Stations[i].Probability;
        }
    }
}
