using Godot;
using System;

public class Health : RigidBody2D {
    AudioPlayer audio;
    public override void _Ready() {
        ContactMonitor = true;
        ContactsReported = 4;
        Connect("body_entered", this, nameof(onHit));
        player = GetParent().GetNode("Player") as Player;
        audio = GetParent().GetNode("AudioPlayer") as AudioPlayer;
    }
    Player player;
    public override void _Process(float delta) {


        GlobalPosition = GlobalPosition.MoveToward(player.GlobalPosition, 5f);
        
    }

    void onHit(Godot.Object body) {
        Player hit = body as Player;

        if(hit != null) {
            if(hit.progressBar.Value < 100)
                hit.progressBar.Value -= 10;

            if(hit.progressBar.Value > 30)
                hit.progressBar.Modulate = Colors.Orange;
            if(hit.progressBar.Value > 70)
                hit.progressBar.Modulate = Colors.Red;
            hit.progressBar.Update();

            CPUParticles2D particles = new CPUParticles2D();
            particles.OneShot = true;
            particles.Amount = 24;
            particles.Spread = 180;
            particles.Color = Colors.Green;
            particles.Direction = Vector2.Zero;
            particles.InitialVelocity = 500;
            particles.ScaleAmount = 6;
            particles.Explosiveness = 1;
            particles.Randomness = 0.4f;
            particles.InitialVelocityRandom = 0.5f;
            GetParent().AddChild(particles);
            particles.Position = hit.Position;

            audio.play(audio.health);

            CallDeferred("free");
        }
    }
}
