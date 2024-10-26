using Godot;
using System;

public class Ammo : RigidBody2D {
    AudioPlayer audio;
    public override void _Ready() {
        ContactMonitor = true;
        ContactsReported = 4;
        Connect("body_entered", this, nameof(onHit));
        audio = GetParent().GetNode("AudioPlayer") as AudioPlayer;
    }
    
    void onHit(Godot.Object body) {
        Player hit = body as Player;

        if(hit != null) {
            hit.ammo += 3;
            hit.ammoCounter.Text = ""+hit.ammo;
            audio.play(audio.ammo);
            CallDeferred("free");
        }
    }
}
