using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AStar
{
    class PriorityQueue
    {
        private List<Vertex> heap;

        public PriorityQueue()
        {
            heap = new List<Vertex>();
        }

        public void Enqueue(Vertex data)
        {
            //add the data to the end of the list
            heap.Add(data);

            //up-heap the data
            int index = heap.Count - 1;
            if (index == 0)
                return;
            int parent = (int)Math.Floor((double)((index - 1) / 2.0));
            Vertex temp;
            while (heap[index].Priority <= heap[parent].Priority)
            {
                //swap the values
                temp = heap[index];
                heap[index] = heap[parent];
                heap[parent] = temp;
                if (parent == 0)
                    return;
                index = parent;
                parent = (int)Math.Floor((double)((index - 1) / 2.0));

            }


        }

        public Vertex Dequeue()
        {
            //get the data to return
            if(heap.Count <= 0)
            {
                return null;
            }
            Vertex data = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            DownHeap(0);

            return data;
        }



        private void DownHeap(int index)
        {
            if (index >= heap.Count) return;
            int child1Index = 2 * index + 1;
            int child2Index = 2 * index + 2;
            int newindex = heap.Count;
            Vertex child1 = new Vertex(0, 0, Microsoft.Xna.Framework.Color.LavenderBlush);
            Vertex child2 = new Vertex(0, 0, Microsoft.Xna.Framework.Color.LavenderBlush);

            if (child1Index < heap.Count)
                child1 = heap[child1Index];
            else
                child1.Priority = double.MaxValue;
            if (child2Index < heap.Count)
                child2 = heap[child2Index];
            else
                child2.Priority = double.MaxValue;

            Vertex parent = heap[index];

            if (parent.Priority > child1.Priority || parent.Priority > child2.Priority)
            {
                if (child1.Priority < child2.Priority)
                {
                    Vertex temp = heap[index];
                    heap[index] = heap[child1Index];
                    heap[child1Index] = temp;
                    DownHeap(child1Index);
                }
                else
                {
                    Vertex temp = heap[index];
                    heap[index] = heap[child2Index];
                    heap[child2Index] = temp;
                    DownHeap(child2Index);
                }
            }
        }

        public Vertex Peek()
        {
            if (heap.Count <= 0)
                return null;
            return heap[0];
        }

        public bool IsEmpty()
        {
            return heap.Count == 0;
        }

        public bool Contains(Vertex search)
        {
            return heap.Contains(search);
        }

        public Vertex Remove(Vertex vertex)
        {
            if(!heap.Contains(vertex))
            {
                return null;
            }

            int index = heap.IndexOf(vertex);
            heap[index] = heap[heap.Count - 1];
            DownHeap(index);
            return vertex;
        }

    }
}
