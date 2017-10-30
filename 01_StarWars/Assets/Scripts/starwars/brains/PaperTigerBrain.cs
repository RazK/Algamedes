// Original: Aviad & Ori
// Authors: Raz & Gal

using System;
using UnityEngine;
using System.Collections.Generic;
using StarWars.Actions;
using Infra.Utils;
using NUnit.Framework;
using StarWars.UI;
using UnityEditor;
using UnityEngine.Windows.Speech;
using Action = StarWars.Actions.Action;


namespace StarWars.Brains {
public class PaperTigerBrain : SpaceshipBrain {
    public override string DefaultName {
        get {
            return "PaperTiger";
        }
    }
    public override Color PrimaryColor {
        get {
            return new Color((float)0xFF, (float)0xA0, (float)0x00, 0xFF);
        }
    }
    public override SpaceshipBody.Type BodyType {
        get {
            return SpaceshipBody.Type.XWing;
        }
    }
    
    [SerializeField] private const float SafeDistance = 2f;
    [SerializeField] private const float ShieldUpTime = 15f;
    [SerializeField] private const float MyShotLimit = 30;
    
    [SerializeField] private const int TurnsToRun = 40;
    [SerializeField] private int _runningFor = 0;
    
    private float _timer;

    private Action RaiseShield()
    {
        _timer = ShieldUpTime;
        return ShieldUp.action;
    }
    
    
    /// <summary>
    /// Meet the PaperTiger spaceship. Sure, it can act tough when nobody is a threat, shooting at the other ships,
    /// but if someone fights back? it runs right off. Even worse! If it can it tries to get close to a third ship
    /// hoping this will shake it's chaser off.
    /// </summary>
    public override Action NextAction()
    {
        var timeToRun = false;
        var nearestShip = FindNearsetShip();
        var fromNearestVector = nearestShip.ClosestRelativePosition(spaceship);
        var nearestAngleToMe = nearestShip.Forward.GetAngle(fromNearestVector);

        if (IsBeingShotAt()) {
            if (spaceship.CanRaiseShield) {
                return RaiseShield();
            }
            timeToRun = true;
            _runningFor = 0;
        }

        // in case of collisions at least both ships will be killed
        if (fromNearestVector.magnitude <= SafeDistance && spaceship.CanRaiseShield) {
            return RaiseShield();
        }

        // If the nearest ship faces the PaperTiger - it runs away
        if (Math.Abs(nearestAngleToMe) <= 20)
        {
            timeToRun = true;
            _runningFor = 0;
        }
        
        if (timeToRun || _runningFor < TurnsToRun)
        {
            ++_runningFor;
            
            // Trying to find the second nearest and get close to it
            var secondNearest = FindSecondNearsetShip();
            if (secondNearest != null) {
                var toSecondNearestVector = spaceship.ClosestRelativePosition(secondNearest);
                var secondTargetVector = toSecondNearestVector + secondNearest.Forward;
                var angleTosecond = secondTargetVector.GetAngle(spaceship.Forward);
                return chooseDirection(angleTosecond);
            }
            
            // Or just run...
            var targetVector = fromNearestVector;
            var angle = targetVector.GetAngle(spaceship.Forward);

            return chooseDirection(angle);
        }

        // If there's no one to run from it shoots
        var toNearestVector = spaceship.ClosestRelativePosition(nearestShip);
        var shootingTargetVector = toNearestVector;
        var shootingAngle = shootingTargetVector.GetAngle(spaceship.Forward);
        
        if (spaceship.CanShoot) 
        {
            return Shoot.action;
        }
        return chooseDirection(shootingAngle);
    }


    private Action chooseDirection(float angle)
    {
        if (angle >= 10f) 
        {
            return TurnLeft.action;
        } 
        else if (angle <= -10f) 
        {
            return TurnRight.action;
        }
        return TurnLeft.action;
    }
    
    
    // From DarthShipBrain
    private Spaceship FindNearsetShip() {
        return FindNearest(Space.Spaceships, IsNotMe);
    }

    private Spaceship FindSecondNearsetShip() {
        return FindNearest(Space.Spaceships, IsNotMeOrNearest);
    }
    
    // From DarthShipBrain
    private bool IsBeingShotAt() {
        var nearestShot = FindNearest(Space.Shots, IsNotMyShot);
        if (nearestShot != null) {
            return spaceship.ClosestRelativePosition(nearestShot).magnitude <= SafeDistance;
        }
        return false;
    }

    // From DarthShipBrain
    private T FindNearest<T>(IList<T> list, System.Func<T ,bool> predicate = null) where T : SpaceObject {
        T nearest = null;
        var minDistance = float.MaxValue;
        foreach (var obj in list) {
            if (!obj.IsAlive) continue;
            var distance = spaceship.ClosestRelativePosition(obj).magnitude;
            if ((nearest == null
                    || distance < minDistance)
                    && (predicate == null || predicate(obj))) {
                nearest = obj;
                minDistance = distance;
            }
        }
        return nearest;
    }

    // From DarthShipBrain
    private bool IsNotMyShot(Shot shot) {
        var angleToShot = shot.Forward.GetAngle(spaceship.Forward);
        return -MyShotLimit > angleToShot || angleToShot > MyShotLimit;
    }

    // From DarthShipBrain
    private bool IsNotMe(Spaceship ship) {
        return ship != spaceship;
    }
    
    private bool IsNotMeOrNearest(Spaceship ship) {
        return IsNotMe(ship) && ship != FindNearsetShip();
    }
}
}
