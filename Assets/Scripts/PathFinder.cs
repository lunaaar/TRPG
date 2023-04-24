using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    //A* algorithm for player movement.
    public List<GridTile> findPath(GridTile start, GridTile end)
    {
        List<GridTile> openList = new List<GridTile>();
        List<GridTile> closedList = new List<GridTile>();

        openList.Add(start);
        while(openList.Count > 0)
        {
            GridTile currentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(currentTile);

            if (!currentTile.isOccupied)
            {
                closedList.Add(currentTile);
            }

            if(currentTile.Equals(end))
            {
                //finalize path
                return getFinishedList(start, end);
            }

            foreach (var tile in getNeighbourTiles(currentTile))
            {
                if (closedList.Contains(tile))
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

    //Returns a list of all GridTiles within a certain tile range (int range) from the center GridTile start
    public List<GridTile> getTilesInRange(GridTile start, int range)
    {
        List<GridTile> openList = new List<GridTile>();
        List<GridTile> closedList = new List<GridTile>();
        openList.Add(start);

        while (openList.Count > 0)
        {
            GridTile currentTile = openList[0];

            openList.Remove(currentTile);

            if (!currentTile.isOccupied)
            {
                closedList.Add(currentTile);
            }

            foreach (var tile in getNeighbourTiles(currentTile))
            {
                if (closedList.Contains(tile))
                {
                    continue;
                }

                if (GetManhattenDistance(start, tile) <= range && !openList.Contains(tile))
                {
                    openList.Add(tile);
                }

            }
        }

        return closedList;
    }

    private List<GridTile> getFinishedList(GridTile start, GridTile end)
    {
        List<GridTile> finishedList = new List<GridTile>();
        GridTile currentTile = end;

        while (currentTile != start)
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
        }

        finishedList.Reverse();

        return finishedList;
    }

    private int GetManhattenDistance(GridTile start, GridTile tile)
    {
        return Mathf.Abs(start.gridPosition.x - tile.gridPosition.x) + Mathf.Abs(start.gridPosition.y - tile.gridPosition.y);
    }

    public List<GridTile> getNeighbourTiles(GridTile currentGridTile)
    {
        var map = MapManager.Instance.map;

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

        return neighbours;
    }
}
