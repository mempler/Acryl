using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acryl.Graphics
{
    public interface ILayerStack
    {
        List<Layer> Layers { get; }
        
        void Push(Layer layer);
        void Pop(Layer layer);
        
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        void Update(GameTime gameTime);
    }

    public class LayerStack : ILayerStack
    {
        public List<Layer> Layers { get; }
        
        public void Push(Layer layer)
        {
            Layers.Add(layer);
        }

        public void Pop(Layer layer)
        {
            Layers.Remove(layer);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var layer in Layers)
                layer.DrawFrame(spriteBatch, gameTime);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var layer in Layers)
                layer.UpdateFrame(gameTime);
        }
    }
}