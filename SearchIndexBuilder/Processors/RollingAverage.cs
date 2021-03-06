﻿using System;
using System.Collections.Generic;

namespace SearchIndexBuilder.Processors
{

    /// <summary>
    /// Handles calculations for the rolling average timing data
    /// </summary>
    public class RollingAverage
    {
        private int _size;
        private Queue<TimeSpan> _times = new Queue<TimeSpan>();

        public RollingAverage(int size)
        {
            _size = size;
        }

        public void Record(TimeSpan time, int pauseMs = 0)
        {
            if(pauseMs > 0)
            {
                _times.Enqueue(time+ TimeSpan.FromMilliseconds(pauseMs));
            }
            else
            {
                _times.Enqueue(time);
            }

            while (_times.Count > _size)
            {
                _times.Dequeue();
            }
        }

        public TimeSpan CurrentAverage()
        {
            int length = _times.Count;
            long sum = 0;
            foreach (var timespan in _times)
            {
                sum += timespan.Ticks;
            }

            return new TimeSpan(sum / length);
        }
    }

}