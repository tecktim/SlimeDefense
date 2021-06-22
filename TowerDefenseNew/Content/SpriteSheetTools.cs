using System.Collections.Generic;
using System.Text;
using TowerDefenseNew.GameObjects;

namespace TowerDefenseNew
{
    internal static class SpriteSheetTools
    {
        /// <summary>
        /// Calculates the texture coordinates for a sprite inside a sprite sheet.
        /// </summary>
        /// <param name="spriteId">The sprite number. Starts with 0 in the upper left corner and increase in western reading direction up to #sprites - 1.</param>
        /// <param name="columns">Number of sprites per row.</param>
        /// <param name="rows">Number of sprites per column.</param>
        /// <returns>Texture coordinates for a single sprite</returns>
        internal static Rect CalcTexCoords(uint spriteId, uint columns, uint rows)
        {
            var result = new Rect(0f, 0f, 1f, 1f);
            uint row = spriteId / columns;
            uint col = spriteId % columns;

            float x = col / (float)columns;
            float y = 1f + 0.0026f / 2 - (row + 1f) / rows;
            float width = 1f / columns;
            float height = (1f + 0.0026f / 2) / rows;

            result = new Rect(x, y, width, height);
            return result;
        }

        internal static IEnumerable<uint> StringToSpriteIds(string text, uint firstCharacter)
        {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
            foreach (var asciiCharacter in asciiBytes)
            {
                yield return asciiCharacter - firstCharacter;
            }
        }
    }
}
