using Godot;
using System;
using System.Collections.Generic;

public class Background : Node2D
{
    Random r;
    List<Sprite> stars;
    public override void _Ready()
    {
        stars = new List<Sprite>();
        r = new Random();
        Texture star = GD.Load<Texture>("res://Textures/star.png");
        for(int x = 0; x < 10; x++) {
            for(int y = 0; y < 24; y++) {
                Sprite sprite = new Sprite();
                sprite.Texture = star;
                sprite.Scale = new Vector2(0.2f, 0.2f);
                AddChild(sprite);
                int xx = 60 + 80 * x + r.Next(-40, 40);
                int yy = 80 * y + r.Next(-40, 40);
                sprite.Position = new Vector2(xx - 400, yy - 720);
                stars.Add(sprite);
            }
        }
        color = new Color(1, 1, 1, 1);
    }
    public bool fade;
    Color color;
    public override void _Process(float delta)
    {
        if(!Player.dead)
            foreach(Sprite s in stars) {
                s.Position += Vector2.Down;

                if(s.Position.y > 800) 
                    s.Position = new Vector2(s.Position.x, -900 + r.Next(-120, 120));
            }

        

        if(fade && color.a > 0) {
            color.a -= 0.01f;
            Modulate = color;
        }
    }
}
