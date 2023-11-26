using System.Collections.Generic;

namespace DefaultNamespace
{
    public class GaussianAnimation
    {
        public List<List<Gaussian>> Frames;

        public GaussianAnimation()
        {
            Frames = new List<List<Gaussian>>();
        }

        public void AddFrame(List<Gaussian> frame)
        {
            Frames.Add(frame);
        }
    }
}