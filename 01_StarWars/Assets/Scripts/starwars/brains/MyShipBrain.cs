using UnityEngine;
using StarWars.Actions;
using Infra.Utils;
using StarWars.UI;

namespace StarWars.Brains {
public class MyShipBrain : SpaceshipBrain {
    public override string DefaultName {
        get {
            return "MyShip";
        }
    }
    public override Color PrimaryColor {
        get {
            return new Color((float)0x76 / 0xFF, (float)0x35 / 0xFF, (float)0x35 / 0xFF, 1f);
        }
    }
    public override SpaceshipBody.Type BodyType {
        get {
            return SpaceshipBody.Type.XWing;
        }
    }

    private Spaceship target = null;

    /// <summary>
    ///  
    /// 
    /// 
    /// </summary>
    public override Action NextAction()
    {
        var pos = spaceship.Position;
        var bounds = Game.Size / 2.2f;

        var outOfX = false;
        var outOfY = false;
        
        if (pos.x < -bounds.x || bounds.x < pos.x)
        {
            outOfX = true;
        }
        if (pos.y < -bounds.y || bounds.y < pos.y)
        {
            outOfY = true;
        }

        if (outOfX)
        {
            if (outOfY)
            {
                return Shoot.action;
            }
            else
            {
                return TurnLeft.action;
            }
        }
        if (outOfY)
        {
            return TurnRight.action;
        }
        
//        if (pos.x >= bounds.x) pos.x -= bounds.x * 2;
//        if (pos.y < -bounds.y) pos.y += bounds.y * 2;
//        if (pos.y >= bounds.y) pos.y -= bounds.y * 2;
        else
        {
            return DoNothing.action;
        }
        

    }

	
}
}