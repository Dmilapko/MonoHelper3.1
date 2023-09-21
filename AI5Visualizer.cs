using AILib;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static AILib.AI5;

namespace MonoHelper
{
    public class AI5Visualizer : Evo0Visualiser
    { 
        public class NeuronVisualizer:Evo0NeuronVisualizer
        {
            public SpriteFont textfont;
            int texture_width;

            public override void CreateNewConnectionVisualizer(AIFamilyEvo0.ConnectionFamilyEvo0 connection)
            {
                connectionvisualizers.Add(new AI5ConnectionVisualizer(connection));
            }

            public NeuronVisualizer(AIFamilyEvo0.NeuronFamilyEvo0 neuron, int texture_width) : base(neuron)
            {
                this.texture_width = texture_width;
            }

            public override void Draw(SpriteBatch spriteBatch, Vector2 draw_position, Color textcolor)
            {
                string text = Math.Abs(Math.Round(neuron.value, 2)).ToString();
                if (text.Count() == 1) text += ".";
                if (text.Count() > 4) text = text.Substring(0, 4);
                else
                {
                    while (text.Count() != 4) text += "0";
                }
                float text_width = textfont.MeasureString(text).X;
                float text_height = textfont.MeasureString(text).Y;
                float r = (texture_width-5) / text_width;
                spriteBatch.DrawString(textfont, text, draw_position + pos.ToVector2(), textcolor, 0f, new Vector2(text_width / 2, text_height / 2), r, SpriteEffects.None, 1);
            }
        }

        public SpriteFont textfont;

        internal class AI5ConnectionVisualizer:Evo0ConnectionVisualizer
        {
            public double current_flow = 0;
            public SpriteFont textfont;

            public AI5ConnectionVisualizer(AIFamilyEvo0.ConnectionFamilyEvo0 connection) : base(connection)
            {
            }

            public override void Draw(SpriteBatch spriteBatch, Vector2 draw_position)
            {
                string text = Math.Round(current_flow, 3).ToString();
                if (text.Count() == 1) text += ".";
                if (text.Count() > 5) text = text.Substring(0, 5);
                else
                {
                    while (text.Count() != 5) text += "0";
                }
                float text_width = textfont.MeasureString(text).X;
                float text_height = textfont.MeasureString(text).Y;
                spriteBatch.DrawString(textfont, text, draw_position + pos.ToVector2(), Color.Red, 0f, new Vector2(text_width / 2, text_height / 2), 0.5f, SpriteEffects.None, 1);
            }
        }

        public AI5Visualizer(GraphicsDevice graphicsDevice, AIFamilyEvo0 ai, int widthpx, int heightpx, SpriteFont textfont) :base(graphicsDevice, ai, widthpx, heightpx)
        {
            foreach (NeuronVisualizer item in neuronvisualizers)
            {
                item.textfont = textfont;
                foreach (AI5ConnectionVisualizer convis in item.connectionvisualizers)
                {
                    convis.textfont = textfont;
                }
            }
        }

        public override void SetAI(int ai_number)
        {
            base.SetAI(ai_number);
            foreach (NeuronVisualizer item in neuronvisualizers)
            {
                item.textfont = textfont;
                foreach (AI5ConnectionVisualizer convis in item.connectionvisualizers)
                {
                    convis.textfont = textfont;
                }
            }
        }

        public AI5Visualizer(GraphicsDevice graphicsDevice, AIFamilyEvo0 ai, int widthpx, int heightpx, SpriteFont textfont, Color textcolor, Color connectioncolor) : base(graphicsDevice, ai, widthpx, heightpx)
        {
            this.textcolor= textcolor;
            this.connectioncolor= connectioncolor;
            this.textfont = textfont;
            SetAI(ai.evolution_history.Count - 1);
        }

        public override void PreProcess()
        {
            foreach (NeuronVisualizer nvis in neuronvisualizers)
            {
                nvis.color = new Color(0, 0.5f + (float)nvis.neuron.value / 2f, 1 - (0.5f + (float)nvis.neuron.value / 2f));
                //nvis.color = new Color(0, (float)nvis.neuron.value, -(float)nvis.neuron.value);
                foreach (AI5ConnectionVisualizer convis in nvis.connectionvisualizers)
                {
                    convis.current_flow = nvis.neuron.value * ((AI5.Connection)convis.connection).weight;
                }
            }
        }

        public override void CreateNewNeuronVisualizer(AIFamilyEvo0.NeuronFamilyEvo0 neuron)
        {
            neuronvisualizers.Add(new NeuronVisualizer(neuron, neurontexture.Width));
        }
    }
}
