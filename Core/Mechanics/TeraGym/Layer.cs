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
        public List<NetNode> nodes = new List<NetNode>();
        public int Size => nodes.Count;

        public ActivationFunction activationFunction;

        public NetLayer(int size, ActivationFunction activationFunction)
        {
            for (int i = 0; i < size; i++)
            {
                nodes.Add(new NetNode());
            }

            this.activationFunction = activationFunction;
        }

        public float[] GetValues()
        {
            float[] values = new float[nodes.Count];
            for(int i = 0; i <nodes.Count; i++)
            {
                values[i] = nodes[i].value;
            }
            return values;
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
                    NetNode node = previousLayer.nodes[k];
                    nodes[j].value += node.value * node.weights[j] + node.bias;
                }

                nodes[j].value += nodes[j].bias;
                nodes[j].value = activationFunction.Compute(nodes[j].value);
            }
        }

        public float[] ComputeSoftMaxedLayer()
        {
            float totalE =  0;

            foreach(NetNode n in nodes)
            {
                totalE += (float)Math.Pow(MathHelper.E, n.value);
            }

            float[] values = new float[Size];
            for(int i = 0; i < Size; i++)
            {
                values[i] = ((float)Math.Pow(MathHelper.E, nodes[i].value)) / totalE;
            }

            return values;
        }

        public static float[] ComputeSoftMaxedLayer(float[] values)
        {
            float totalE = 0;
            
            foreach (float n in values)
            {
                totalE += (float)Math.Pow(MathHelper.E, n);
            }

            float[] output = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                output[i] = ((float)Math.Pow(MathHelper.E, values[i])) / totalE;
            }

            return values;
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