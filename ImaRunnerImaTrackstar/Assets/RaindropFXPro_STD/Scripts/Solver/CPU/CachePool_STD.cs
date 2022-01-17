using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RaindropFX;

namespace RaindropFX {
    public class CachePool_STD : MonoBehaviour {

        public int counter = 0;

        List<Raindrop_STD> raindrops = new List<Raindrop_STD>();

        public void Init() {
            counter = 0;
            raindrops.Clear();
        }

        public void Recycle(Raindrop_STD raindrop) {
            raindrops.Add(raindrop);
            counter = raindrops.Count;
        }

        public Raindrop_STD GetRaindrop() {
            if (counter > 0) {
                Raindrop_STD temp = raindrops[0];
                raindrops.RemoveAt(0);
                counter = raindrops.Count;
                return temp;
            } else return new Raindrop_STD();
        }

    }
}
