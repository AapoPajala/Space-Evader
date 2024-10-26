using Godot;
using System;

public class AudioPlayer : AudioStreamPlayer2D
{
    
    public override void _Ready()
    {
        alien = GD.Load<AudioStream>("res://sounds/alien.wav");
        explosion = GD.Load<AudioStream>("res://sounds/explosion.wav");
        comet = GD.Load<AudioStream>("res://sounds/comet.wav");
        gameover = GD.Load<AudioStream>("res://sounds/gameover.wav");
        rocket = GD.Load<AudioStream>("res://sounds/rocket.wav");
        start = GD.Load<AudioStream>("res://sounds/start.wav");
        ammo = GD.Load<AudioStream>("res://sounds/ammo.wav");
        health = GD.Load<AudioStream>("res://sounds/health.wav");
        newhs = GD.Load<AudioStream>("res://sounds/newhs.wav");
    }
    public AudioStream explosion, comet, alien, gameover, rocket, start, ammo, health, newhs;
    public void play(AudioStream file) {
        Stream = file;
        Play(0);
    }

}
