using System;
using System.Linq;
using Microsoft.Phone.Shell;

namespace Hailstone.Util
{
    public static class ShellTileEx
    {
        public static void Create(Uri uri, StandardTileData data)
        {
            var bgImage = data.BackgroundImage;
            var backBgImage = data.BackBackgroundImage;

            bool found = false;
            if (bgImage != null && bgImage.IsAbsoluteUri)
            {
                data.BackgroundImage = null;
                found = true;
            }

            if (backBgImage != null && backBgImage.IsAbsoluteUri)
            {
                data.BackBackgroundImage = null;
                found = true;
            }

            ShellTile.Create(uri, data);

            //if no distant uri, do nothing
            if (!found)
                return;

            //find my tile
            ShellTile mytile = ShellTile.ActiveTiles.FirstOrDefault(currentTile => currentTile.NavigationUri == uri);
            if (mytile == null)
                return;

            //update the tile
            mytile.Update(new StandardTileData() { BackgroundImage = bgImage, BackBackgroundImage = backBgImage });

        }
    }
}