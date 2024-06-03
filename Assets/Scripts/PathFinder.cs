using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    List<GridTile> openList = new List<GridTile>();
    List<GridTile> closedList = new List<GridTile>();

    //A* algorithm for player movement.
    public List<GridTile> findPath(GridTile start, GridTile end)
    {
        openList.Clear();
        closedList.Clear();

        openList.Add(start);
        while(openList.Count > 0)
        {
            GridTile currentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile.Equals(end))
            {
                //finalize path
                return getFinishedList(start, end);
            }

            foreach (var tile in getNeighbourTiles(currentTile))
            {
                //This will likely need to be updated to allow moving onto objective.
                if (closedList.Contains(tile) || !tile.status.Equals("NotOccupied"))
                {
                    continue;
                }
                
                tile.G = GetManhattenDistance(start, tile);
                tile.H = GetManhattenDistance(end, tile);

                tile.previous = currentTile;


                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                }
            }
        }

        return new List<GridTile>();
    }

    public List<GridTile> findArrowPath(GridTile start, GridTile end)
    {
        openList.Clear();
        closedList.Clear();

        openList.Add(start);
        while (openList.Count > 0)
        {
            GridTile currentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == end)
            {
                //finalize path
                return getFinishedList(start, end);
            }

            foreach (var tile in getNeighbourAttackTiles(currentTile))
            {
                if (tile.Equals(end) && tile.status.Equals("Enemy"))
                {
                    tile.previous = currentTile;
                    openList.Add(tile);
                }

                //TODO: so this shows up.
                //? Should be fixed now.
                if (closedList.Contains(tile) || (!tile.status.Equals("NotOccupied") && !tile.status.Equals("Objective")))
                {
                    continue;
                }
                tile.G = GetManhattenDistance(start, tile);
                tile.H = GetManhattenDistance(end, tile);

                tile.previous = currentTile;


                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                }
            }
        }

        return new List<GridTile>();
    }

    public List<GridTile> findTruePath(GridTile start, GridTile end)
    {
        //? This is the True Path calculator, it basically just finds either a straight line, or the quickest path to a point.
        
        openList.Clear();
        closedList.Clear();

        openList.Add(start);
        while (openList.Count > 0)
        {
            GridTile currentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == end)
            {
                //finalize path
                return getFinishedList(start, end);
            }

            foreach (var tile in getNeighbourTrueTiles(currentTile))
            {
                if (closedList.Contains(tile) )
                {
                    continue;
                }
                tile.G = GetManhattenDistance(start, tile);
                tile.H = GetManhattenDistance(end, tile);

                tile.previous = currentTile;


                if (!openList.Contains(tile))
                {
                    openList.Add(tile);
                }
            }
        }

        return new List<GridTile>();
    }

    private List<GridTile> getFinishedList(GridTile start, GridTile end)
    {
        List<GridTile> finishedList = new List<GridTile>();
        GridTile currentTile = end;

        /**while (currentTile != start)
        {
            if (!finishedList.Contains(currentTile))
            {
                finishedList.Add(currentTile);
                if (currentTile == start)
                {
                    break;
                }
                currentTile = currentTile.previous;
            }
            else
            {
                break;
            }
        }*/

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            if(currentTile.Equals(start))
            {
                break;
            }
            currentTile = currentTile.previous;
        }

        finishedList.Reverse();

        //Debug.Log("Finished List:");
        //Debug.Log(start + " | " + end);
        //Debug.Log(finishedList.Count);

        return finishedList;
    }

    public int GetManhattenDistance(GridTile start, GridTile tile)
    {
        return Mathf.Abs(start.gridPosition.x - tile.gridPosition.x) + Mathf.Abs(start.gridPosition.y - tile.gridPosition.y);
    }

    //. Returns a list of all GridTiles within a certain tile range (int range) from the center GridTile start
    public List<GridTile> getTilesInRange(GridTile start, int range)
    {
        List<GridTile> inRangeTiles = new List<GridTile>();
        inRangeTiles.Add(start);
        int step = 0;

        List<GridTile> previousStep = new List<GridTile>();
        previousStep.Add(start);

        while (step < range)
        {
            var surroundingTiles = new List<GridTile>();

            foreach (var tile in previousStep)
            {
                surroundingTiles.AddRange(getNeighbourAttackTiles(tile));
            }

            inRangeTiles.AddRange(surroundingTiles);
            previousStep = surroundingTiles.Distinct().ToList();
            step++;
        }

        return inRangeTiles.Distinct().ToList();
    }

    public Vector3Int getAbove(Vector3Int v)
    {
        return new Vector3Int(v.x, v.y, v.z + 1);
    }

    public List<GridTile> getNeighbourTiles(GridTile currentGridTile)
    {  

        var map = MapManager.instance.map;

        List<GridTile> neighbours = new List<GridTile>();

        Vector3Int locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck) &&
            !map.ContainsKey(getAbove(locationToCheck)) &&
            !map[locationToCheck].status.Equals("Occupied") &&
            !map[locationToCheck].status.Equals("Obstacle"))
        {
            neighbours.Add(map[locationToCheck]);
        }

        return neighbours;
    }

    /**public List<GridTile> getNeighbourTiles(GridTile currentGridTile)
    {
        var map = MapManager.instance.map;

        List<GridTile> neighbours = new List<GridTile>();

        //if (map.ContainsKey(locationToCheck) && (!map[locationToCheck].status.Equals("Occupied") || GetManhattenDistance(map[locationToCheck], map[character.gridPos]) == character.movementRange + character.weapons[0].attackRange)) neighbours.Add(map[locationToCheck]);

        //Right
        Vector3Int locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y);
        if (map.ContainsKey(locationToCheck) && !map[locationToCheck].status.Equals("Occupied") && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        //Left
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y);
        if (map.ContainsKey(locationToCheck) && !map[locationToCheck].status.Equals("Occupied") && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        //Up
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1);
        if (map.ContainsKey(locationToCheck) && !map[locationToCheck].status.Equals("Occupied") && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        //Down
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1);
        if (map.ContainsKey(locationToCheck) && !map[locationToCheck].status.Equals("Occupied") && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        return neighbours;
    }*/

    public List<GridTile> getNeighbourAttackTiles(GridTile currentGridTile)
    {
        var map = MapManager.instance.map;

        List<GridTile> neighbours = new List<GridTile>();

        Vector3Int locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);
        
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        return neighbours;
    }

    /**public List<GridTile> getNeighbourAttackTiles(GridTile currentGridTile)
    {
        var map = MapManager.instance.map;

        List<GridTile> neighbours = new List<GridTile>();

        //Right
        Vector3Int locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y);
        if (map.ContainsKey(locationToCheck) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        //Left
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y);
        if (map.ContainsKey(locationToCheck) && !map.ContainsKey(getAbove(locationToCheck)) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        //Up
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1);
        if (map.ContainsKey(locationToCheck) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        //Down
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1);
        if (map.ContainsKey(locationToCheck) && !map[locationToCheck].status.Equals("Obstacle")) neighbours.Add(map[locationToCheck]);

        return neighbours;
    }*/

    public List<GridTile> getNeighbourTrueTiles(GridTile currentGridTile)
    {
        var map = MapManager.instance.map;

        List<GridTile> neighbours = new List<GridTile>();

        Vector3Int locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z + 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1, currentGridTile.gridPosition.z - 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        return neighbours;

        /**var map = MapManager.instance.map;

        List<GridTile> neighbours = new List<GridTile>();

        //Right
        Vector3Int locationToCheck = new Vector3Int(currentGridTile.gridPosition.x + 1, currentGridTile.gridPosition.y);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        //Left
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x - 1, currentGridTile.gridPosition.y);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        //Up
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y + 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        //Down
        locationToCheck = new Vector3Int(currentGridTile.gridPosition.x, currentGridTile.gridPosition.y - 1);
        if (map.ContainsKey(locationToCheck)) neighbours.Add(map[locationToCheck]);

        return neighbours;*/
    }

    public List<GridTile> getCube(GridTile start, int baseRange, int influenceRange = 0)
    {
        List<GridTile> inRangeTiles = new List<GridTile>();

        if (baseRange % 2 == 0)
        {
            /*
             * Even Condition:
             * Treats gridPos as the bottom right most position
             */
            var calcRange = influenceRange + Mathf.FloorToInt(Mathf.Floor(baseRange / 2));

            for (int x = -calcRange + 1; x <= calcRange; x++)
            {
                for (int y = -calcRange + 1; y <= calcRange; y++)
                {
                    for (int z = 2; z > -1; z--)
                    {
                        var location = new Vector3Int(start.gridPosition.x + x, start.gridPosition.y + y, start.gridPosition.z);

                        if (MapManager.instance.map.ContainsKey(location))
                        {
                            inRangeTiles.Add(MapManager.instance.map[location]);
                            break;
                        }
                    }
                }
            }

        }
        else
        {
            /*
             * Odd Condition:
             * Treats gridPos as the center position
             */

            var calcRange = influenceRange + Mathf.FloorToInt(Mathf.Floor(baseRange/2));

            for (int x = -calcRange; x <= calcRange; x++)
            {
                for (int y = -calcRange; y <= calcRange; y++)
                {
                    for (int z = 1; z > -1; z--)
                    {
                        var location = new Vector3Int(start.gridPosition.x + x, start.gridPosition.y + y, start.gridPosition.z);

                        if (MapManager.instance.map.ContainsKey(location))
                        {
                            inRangeTiles.Add(MapManager.instance.map[location]);
                            break;
                        }
                    }
                }
            }
        }

        return inRangeTiles.Distinct().ToList();
    }
}
