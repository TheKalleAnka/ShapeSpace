﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Camera
{
    private readonly Viewport _viewport;

    public Camera(Viewport viewport)
    {
        _viewport = viewport;

        Rotation = 0;
        Zoom = 0.8f;
        Origin = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
        Position = Vector2.Zero;
    }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public float Zoom { get; set; }
    public Vector2 Origin { get; set; }

    public Matrix GetViewMatrix()
    {
        return
            Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
            Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom, Zoom, 1) *
            Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
    }
}
