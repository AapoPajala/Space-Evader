using Godot;
using System;
using System.Collections.Generic;

public class Spawner : Node2D
{
    PackedScene[] objects;
    AudioPlayer audio;
    public override void _Ready()
    {
        objects = new PackedScene[4];
        objects[0] = GD.Load<PackedScene>("res://Objects/Asteroid.tscn");
        objects[1] = GD.Load<PackedScene>("res://Objects/Comet.tscn");
        objects[2] = GD.Load<PackedScene>("res://Objects/Alien.tscn");
        objects[3] = GD.Load<PackedScene>("res://Objects/Ammo.tscn");

        audio = GetNode("AudioPlayer") as AudioPlayer;

        alive = new List<Node2D>();
        trash = new List<Node2D>();
    }

    float time;
    Random r = new Random();
    List<Node2D> alive, trash;
    int n;
    public override void _Process(float delta)
    {
        if(time <= 0 && !Player.dead) {
            int c = r.Next(10);

            int i = 0;
            if(c == 7) {
                i = 3;
            }
            if(c == 8) {
                i = 1;
                audio.play(audio.comet);
            }
            if(c == 9) {
                i = 2;
            }

            int sc = r.Next(n);
            if(sc > 10) i = 1;
            
            if(i == 2) audio.play(audio.alien);

            RigidBody2D obj = objects[i].Instance() as RigidBody2D;

            AddChild(obj);

            obj.Position = new Vector2(r.Next(-310, 310), -920);

            float vel = 1000;

            if(i == 1) vel = 1500;

            obj.LinearVelocity = new Vector2(0, vel + n);

            alive.Add(obj);

            int rs = n > 25 ? 1 : 0;
            int rd = n > 50 ? 2 : 0;

            time = r.Next(2 - rs, 4 - rd);

            foreach(Node2D n in alive) {
                if(IsInstanceValid(n))
                    if(n != null && n.Position.y > 1200) trash.Add(n);
            }
            foreach(Node2D d in trash) {
                d.QueueFree();
                alive.Remove(d);
                n++;
            } trash.Clear();

        } else time -= delta;

    }

}
