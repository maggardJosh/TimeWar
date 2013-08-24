﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class World
{
    private List<Player> playerList = new List<Player>();
    private List<Bullet> bulletList = new List<Bullet>();
    private List<FNode> spawnPoints = new List<FNode>();
    private List<Powerup> powerups = new List<Powerup>();

    FNode playerSpawn;

    private FTmxMap tmxMap = new FTmxMap();
    private FTilemap tilemap;
    private FContainer playerLayer = new FContainer();

    public FTilemap Tilemap { get { return tilemap; } }

    public FCamObject gui;

    private Clock clock;

    public World()
    {

        Futile.stage.AddChild(playerLayer);

        tmxMap.LoadTMX("Maps/mapOne");
        tilemap = (FTilemap)(tmxMap.getLayerNamed("Tilemap"));
        FTilemap objectLayer = (FTilemap)(tmxMap.getLayerNamed("Objects"));

        for (int xInd = 0; xInd < objectLayer.widthInTiles; xInd++)
            for (int yInd = 0; yInd < objectLayer.heightInTiles; yInd++)
            {
                switch (objectLayer.getFrameNum(xInd, yInd))
                {
                    case 0:

                        break;
                    case 10:
                        FNode newSpawn = new FNode();
                        newSpawn.x = xInd * tilemap._tileWidth + tilemap._tileWidth / 2;
                        newSpawn.y = -yInd * tilemap._tileHeight - tilemap._tileHeight / 2;
                        spawnPoints.Add(newSpawn);
                        break;
                    case 11:
                        playerSpawn = new FNode();
                        playerSpawn.x = xInd * tilemap._tileWidth + tilemap._tileWidth / 2;
                        playerSpawn.y = -yInd * tilemap._tileHeight - tilemap._tileHeight / 2;
                        break;
                }
            }

        playerLayer.AddChild(tmxMap);
    }

    public bool isWalkable(int tileX, int tileY)
    {
        return tilemap.getFrameNum(tileX, tileY) != 1;
    }

    public void addPlayer(Player p)
    {
        if (p.isControlled)
            p.SetPosition(playerSpawn.GetPosition());
        else
            p.SetPosition(spawnPoints[RXRandom.Int(spawnPoints.Count)].GetPosition());
        float scaleChance = RXRandom.Float();
        if (scaleChance < .1f)
            p.setScale(3.0f);
        else if (scaleChance < .3f)
            p.setScale(2.0f);
        playerLayer.AddChild(p);
        playerList.Add(p);
        p.setWorld(this);
    }

    public void Update()
    {
        
        for (int ind = 0; ind < powerups.Count; ind++)
        {
            Powerup powerup = powerups[ind];
            foreach (Player p in playerList)
            {
                if(p.isControlled)
                if (powerup.checkCollision(p))
                {
                    p.collectPowerUp(powerup.PType);
                    powerup.RemoveFromContainer();
                    powerups.Remove(powerup);
                    ind--;
                }
            }
        }
        for (int ind = 0; ind < bulletList.Count; ind++)
        {
            Bullet b = bulletList[ind];
            b.Update();
            for (int playerInd = 0; playerInd < playerList.Count; playerInd++)
            {
                Player p = playerList[playerInd];
                if (b.checkCollision(p))
                {
                    p.setScale(p.scale - 1.0f, false);
                    if (p.scale <= 0)
                    {
                        p.RemoveFromContainer();
                        playerList.Remove(p);
                        playerInd--;
                        FloatIndicator floatInd = new FloatIndicator("+00:00:0" + p.secondValue, p.GetPosition());
                        playerLayer.AddChild(floatInd);
                        clock.percentage += p.secondValue / 10.0f;
                        if (p.secondValue == 3)
                        {
                            Powerup powerup = new Powerup(Powerup.PowerupType.SHOTGUN);
                            powerups.Add(powerup);
                            powerup.SetPosition(p.GetPosition());
                            playerLayer.AddChild(powerup);
                        }
                    }
                    b.RemoveFromContainer();
                    bulletList.Remove(b);
                    ind--;
                    break;
                }
                else
                    if (tilemap.getFrameNum((int)(b.x / tilemap._tileWidth), (int)(-b.y / tilemap._tileHeight)) == 1)
                    {
                        b.RemoveFromContainer();
                        bulletList.Remove(b);
                        ind--;
                        break;
                    }
            }
        }
    }

    internal void addBullet(Bullet b)
    {
        bulletList.Add(b);
        playerLayer.AddChild(b);
    }

    internal void setGUI(FCamObject gui)
    {
        tilemap.clipNode = gui;
        this.gui = gui;
    }

    internal void setClock(Clock clock)
    {
        this.clock = clock;
    }
}
