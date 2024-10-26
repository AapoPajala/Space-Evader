using Godot;
using System;

public class Rocket : RigidBody2D
{
    AudioPlayer audio;
    public override void _Ready()
    {
        audio = GetParent().GetNode("AudioPlayer") as AudioPlayer;
        ContactMonitor = true;
        ContactsReported = 4;
        Connect("body_entered", this, nameof(onHit));
        h = GD.Load<PackedScene>("res://Objects/Health.tscn");
        life = 3f;
    }
    PackedScene h;

    public override void _Process(float delta) {
        if(life > 0) life -= delta;
        else QueueFree();
    }

    float life;

    void onHit(Godot.Object body) {
        RigidBody2D hit = body as RigidBody2D;

        if (hit != null && hit.GetType() != typeof(Health)) {
            CPUParticles2D particles = new CPUParticles2D();
            particles.OneShot = true;
            particles.Amount = 48;
            particles.Spread = 180;
            particles.Color = Colors.Red;
            particles.Direction = Vector2.Zero;
            particles.InitialVelocity = 500;
            particles.ScaleAmount = 6;
            particles.Explosiveness = 1;
            particles.Randomness = 0.4f;
            particles.InitialVelocityRandom = 0.5f;
            GetParent().AddChild(particles);
            particles.Position = hit.Position;

            if(hit.Name.Contains("Alien")) {
                Health health = h.Instance() as Health;
                GetParent().CallDeferred("add_child", health);
                health.GlobalPosition = hit.GlobalPosition;
            }

            audio.play(audio.explosion);


            CallDeferred("free");
            hit.CallDeferred("free");
        }
    }
}
