﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Clock : FRadialWipeSprite
{
    protected float clockMargin = 5;
    private float normalScale = 2.0f;
    protected float tickScale = 5.0f;
    protected FLabel timeLabel;
    protected FSprite clockBackground;
    protected FLabel label;
    public Clock()
        : base("clock", true, 0, 1.0f)
    {
        label = new FLabel("Small", "Time Left");
        timeLabel = new FLabel("Small", "10");
        timeLabel.color = Color.black;
        x = Futile.screen.halfWidth - width * tickScale / 2 - clockMargin;
        y = Futile.screen.halfHeight - height * tickScale / 2 - clockMargin;
        timeLabel.SetPosition(this.GetPosition());
        this.scale = normalScale;
        clockBackground = new FSprite("clock");
        clockBackground.SetPosition(GetPosition());
        label.x = x;
        label.y = y - height/2 * tickScale / 2;

    }

    public override void HandleAddedToContainer(FContainer container)
    {
        container.AddChild(clockBackground);
        container.AddChild(timeLabel);
        base.HandleAddedToContainer(container);
        container.AddChild(label);
    }
    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        clockBackground.RemoveFromContainer();
        Futile.instance.SignalUpdate -= Update;
        timeLabel.RemoveFromContainer();
        base.HandleRemovedFromStage();
        label.RemoveFromContainer();
    }

    int lastSecond = 10;

    public virtual void Update()
    {
        timeLabel.MoveToFront();
        percentage -= UnityEngine.Time.deltaTime * .1f;
        if (Mathf.FloorToInt(percentage * 10) != lastSecond)
        {
            lastSecond = Mathf.FloorToInt(percentage * 10);
            timeLabel.text =""+ lastSecond;
            this.scale = tickScale;
        }
        else
        {
            if (this.scale >= normalScale)
                this.scale -= UnityEngine.Time.deltaTime * (tickScale - normalScale);
        }
        timeLabel.scale = this.scale;


        if (this.percentage < .3f)
            this.color = new Color(percentage*3, 0, 0);
        else if (this.percentage < .5f)
            this.color = new Color(percentage * 2, percentage * 2, 0);
        else
            this.color = new Color( 1.0f - percentage, percentage*2,0);

        clockBackground.scale = this.scale;
        
    }
}
