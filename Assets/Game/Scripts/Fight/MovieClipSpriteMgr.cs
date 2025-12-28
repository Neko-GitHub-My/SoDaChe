using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MovieClipSpriteMgr
{
    public static MovieClipSpriteMgr Instance = new MovieClipSpriteMgr();

    private Dictionary<string, Sprite[][]> spriteClips = new Dictionary<string, Sprite[][]>();
    
    public async Task<Sprite[][]> PreloadMoveClip(string assetUrl) {
        Texture2D texture = await ResMgr.Instance.AwaitGetAsset<Texture2D>(assetUrl);
        
        int row = 8;
        int col = 8;

        int _pieceWidth = texture.width / col;
        int _pieceHeight = texture.height / row;

        Sprite[][] _bitmapArr = new Sprite[row][];
        for (int i = 0; i < row; i++)
        {
            _bitmapArr[i] = new Sprite[col];

            for (int j = 0; j < col; j++)
            {
                _bitmapArr[i][j] = Sprite.Create(texture, new Rect(j * _pieceWidth, i * _pieceHeight, _pieceWidth, _pieceHeight), new Vector2(0, 0));
            }
        }
        // end

        Debug.Log(texture.name);

        return _bitmapArr;
    }

    public Sprite[][] Get(string name) {
        return this.spriteClips[name];
    }

    public async void Init() {
        // 10003
        Sprite[][] _bitmapArr = await this.PreloadMoveClip("Charactors/10003/role/ltz8_w1");
        this.spriteClips.Add("ltz8_w1", _bitmapArr);

        _bitmapArr = await this.PreloadMoveClip("Charactors/10003/role/ltz8");
        this.spriteClips.Add("ltz8", _bitmapArr);
        // end

    }
}
