
using System;
using Godot;

public class Utils {
    public static class Colors {
        public static readonly Color RED = new Color(255, 0, 0);
        public static readonly Color GREEN = new Color(0, 255, 0);
        public static readonly Color BLUE = new Color(0, 0, 255);
        public static readonly Color YELLOW = new Color(195, 140, 50);
    }

    public static class Tiles {
        public static readonly int TILE_MAP_SOURCE_ID = 1;
        public static readonly int FOOD_TILE_ALT_ID = 1;
        public static readonly int CELL_TILE_ALT_ID = 2;

        public static bool TileIsEmpty(TileMapLayer tileMap, Vector2I coordinates) {
            return GetCellAltId(tileMap, coordinates) < 0;
        }

        public static bool TileContainsFood(TileMapLayer tileMap, Vector2I coordinates) {
            return GetCellAltId(tileMap, coordinates) == FOOD_TILE_ALT_ID;
        }

        public static bool TileContainsCell(TileMapLayer tileMap, Vector2I coordinates) {
            return GetCellAltId(tileMap, coordinates) == CELL_TILE_ALT_ID;
        }

        public static void ClearTile(TileMapLayer tileMap, Vector2I coordinates) {
            tileMap.SetCell(coordinates, -1);
        }

        private static int GetCellAltId(TileMapLayer tileMap, Vector2I coordinates) {

            int cellSourceId = tileMap.GetCellSourceId(coordinates);

            if (cellSourceId < 0) {
                return -1;
            }

            return tileMap.GetCellAlternativeTile(coordinates);
        }
    }
}
