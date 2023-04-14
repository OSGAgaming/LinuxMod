using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System.Collections.Generic;
using System;

namespace LinuxMod.Core.Mechanics
{
    public class BaseNeuralNetwork : INeuralNetwork
    {
        public NetLayer Inputs;
        public NetLayer Outputs;

        public List<NetLayer> HiddenLayers = new List<NetLayer>();
        public int Size => HiddenLayers.Count + 2;


        public BaseNeuralNetwork(int inputSize)
        {
            Inputs = new NetLayer(inputSize, null);
        }

        public NetLayer GetLayer(int index)
        {
            if (index == 0) return Inputs;
            else if (index == Size - 1) return Outputs;
            else return HiddenLayers[index - 1];
        }

        public void Draw(SpriteBatch sb, Vector2 position)
        {
            int horizontalSep = 100;
            int verticalSep = 40;

            for (int i = 0; i < Size; i++)
            {
                NetLayer layer = GetLayer(i);
                for (int j = 0; j < layer.Size; j++)
                {
                    Vector2 v1 = position + new Vector2(i * horizontalSep, j * verticalSep);
                    LinuxTechTips.DrawCircle(v1, new Vector2(4), Color.AliceBlue);
                    if (i > 0)
                    {
                        NetLayer lastLayer = GetLayer(i - 1);
                        for (int k = 0; k < lastLayer.Size; k++)
                        {
                            Vector2 v2 = position + new Vector2((i - 1) * horizontalSep, k * verticalSep);
                            float rawWeight = lastLayer.nodes[k].weights[j];
                            float weight = (rawWeight + 1) / 2f;
                            LinuxTechTips.DrawLine(v1, v2, Color.Lerp(Color.IndianRed, Color.SeaGreen, weight), Math.Abs(rawWeight)*2);
                        }
                    }
                }
            }
        }

        public void UpdateNetwork(float[] inputs)
        {
            Inputs.Map(inputs);

            for (int i = 1; i < Size; i++)
            {
                GetLayer(i).ComputeLayer(GetLayer(i - 1));
            }
        }

        public BaseNeuralNetwork AddLayer<T>(int size) where T : ActivationFunction, new()
        {
            HiddenLayers.Add(new NetLayer(size, new T()));
            return this;
        }

        public BaseNeuralNetwork SetOutput<T>(int size) where T : ActivationFunction, new()
        {
            Outputs = new NetLayer(size, new T());
            return this;
        }

        public BaseNeuralNetwork GenerateWeights(Func<float> weightInitialisationFunction = null)
        {
            for (int i = 0; i < Size - 1; i++)
            {
                GetLayer(i).GenerateWeights(GetLayer(i + 1).Size, weightInitialisationFunction);
            }
            return this;
        }
    }
}