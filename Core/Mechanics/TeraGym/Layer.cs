using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using LinuxMod.Core;
using System.Collections.Generic;
using System;

namespace LinuxMod.Core.Mechanics
{
    public class NetLayer
    {
        public List<Node> nodes = new List<Node>();
        public int Size => nodes.Count;

        private ActivationFunction activationFunction;

        public NetLayer(int size, ActivationFunction activationFunction)
        {
            for (int i = 0; i < size; i++)
            {
                nodes.Add(new Node());
            }

            this.activationFunction = activationFunction;
        }

        public void Map(float[] values)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                nodes[i].value = values[i];
            }
        }

        public void ComputeLayer(NetLayer previousLayer)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                nodes[j].value = 0;

                for (int k = 0; k < previousLayer.nodes.Count; k++)
                {
                    Node node = previousLayer.nodes[k];
                    nodes[j].value += node.value * node.weights[j] + node.bias;
                }

                nodes[j].value += nodes[j].bias;
                nodes[j].value = activationFunction.Compute(nodes[j].value);
            }
        }

        public float Max()
        {
            float value = float.MinValue;
            int index = 0;

            for(int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].value > value)
                {
                    value = nodes[i].value;
                    index = i;
                }
            }

            return index;
        }

        public void GenerateWeights(int nextLayerSize, Func<float> weightInitialisationFunction = null)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                float[] array = new float[nextLayerSize];
                for(int j = 0; j < nextLayerSize; j++)
                {
                    array[j] = weightInitialisationFunction?.Invoke() ?? 0;
                }
                nodes[i].weights = array;
            }
        }
    }
}