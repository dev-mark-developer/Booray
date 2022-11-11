using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Booray.Game
{



[CreateAssetMenu(fileName = "Skin_", menuName = "Create CardSkin")]
    public class CardSkinObject : ScriptableObject
    {
        public string skinName;
        public string skinID;

        public SpriteAtlas skinAtlas;

    }
}


