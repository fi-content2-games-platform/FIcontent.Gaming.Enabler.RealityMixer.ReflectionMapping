using System;
using System.Collections.Generic;

public class ShuffleArray
{
    private static Random rand = new Random((int)System.DateTime.Now.Ticks);

    public static void Shuffle<T>(T[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            int j = rand.Next(i, array.Length);
            T tmp = array[j]; 
            array[j] = array[i];
            array[i] = tmp;
        }
    }
}

