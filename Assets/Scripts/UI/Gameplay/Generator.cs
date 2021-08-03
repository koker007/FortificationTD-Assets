using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Generator
{

    //Получить высоту для конкретной ячейки
    static public float getHeight(string keygen, int x, int y, int maxSizeMap) {
        float height = 0;
        float key = 0;
        //Получаем число ключа
        for (int kgsymbol = 0; kgsymbol < 10; kgsymbol++) {
            if (kgsymbol < keygen.Length) {
                key += keygen[kgsymbol] * keygen[0];
            }
        }

        

        float posx = key;
        float posy = key;

        //Получаем число для X
        if (1 < keygen.Length) {
            posx /= keygen[1];
        }
        //получаем число для y
        if (2 < keygen.Length) {
            posy /= keygen[2];
        }

        float perlin = Mathf.PerlinNoise(posx + ((float)x/10), posy + ((float)y /10));

        //уменьшаем в зависимости дальности от центра
        float coofx = 0;
        if (x < maxSizeMap / 2)
        {
            coofx = (float)x / (maxSizeMap / 3f);
        }
        //Если больше центра
        else {
            coofx = 1 - ((float)x - (maxSizeMap / 2f)) / (maxSizeMap / 2);
        }

        float coofy = 0;
        if (y < maxSizeMap / 2)
        {
            coofy = (float)y / (maxSizeMap / 3f);
        }
        //Если больше центра
        else
        {
            coofy = 1 - ((float)y - (maxSizeMap / 2f)) / (maxSizeMap / 2);
        }

        float perlX = perlin * coofx;
        float perlY = perlin * coofy;
        height = perlX;
        if (perlY < perlX) {
            height = perlY;
        }

        return height;
    }

    
}
